using AkaConfig;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using AkaRedisLogic;
using AkaUtility;
using Common;
using Common.Entities.Challenge;
using Common.Entities.Mail;
using Common.Entities.PayRewardedCheck;
using Common.Entities.Product;
using Common.Entities.Season;
using Common.Entities.ServerStatus;
using Common.Entities.SquareObject;
using Common.Pass;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLogic.Friend;
using WebLogic.User.DataUpdator;

namespace WebLogic.User
{
    public class UserLoginInfo
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private ProtoOnLogin _protoOnLogin;
        private AccountInfo _accountInfo;
        private IDatabase _redis;
        private ServerSeasonInfo _seasonInfo;

        public UserLoginInfo(DBContext accountDb, DBContext userDb, IDatabase redis, AccountInfo accountInfo, ProtoOnLogin protoOnLogin)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _redis = redis;
            _accountInfo = accountInfo;
            _protoOnLogin = protoOnLogin;
        }

        public async Task GetProtoOnLogin(PlatformType platformType, string languageType)
        {
            _protoOnLogin.Nickname = _accountInfo.Nickname;
            _protoOnLogin.ProfileIconId = _accountInfo.ProfileIconId;
            
            await UserInitDataUpdate();
            _seasonInfo = await SetSeasonInfo();
            await UserUpdatorRun();
            await UserGetSetUp();
            ProductManager productManager = new ProductManager(_accountDb, _userDb, _protoOnLogin.UserId);
            await productManager.GetStoreProducts(platformType, languageType, _protoOnLogin.Products);

            await GetEvents();

            _protoOnLogin.BattlePlayingInfo = await GetBattlePlayingInfo(_protoOnLogin.UserId);
            _protoOnLogin.NowServerDateTime = DateTime.UtcNow.Ticks;

            PayRewarded payRewarded = new PayRewarded(_accountDb, _protoOnLogin.UserId);
            _protoOnLogin.PendingStoreInfos = await payRewarded.GetPendingStoreInfos();

            await SetFriendInfo(_accountDb, _userDb, _accountInfo.UserId);


            Log.User.Login.Log(_accountInfo.UserId);

            await UpdateLoginDateTime();

            await SetPubSubServerInfo();

            var mail = MailFactory.CreateMail(MailType.Common, _accountDb, _userDb, _protoOnLogin.UserId);
            _protoOnLogin.MailInfo = await mail.GetMailInfo(languageType);
            
        }

        private async Task UserUpdatorRun()
        {
            var userUpdator = new UserUpdator(_accountInfo.UserId, _protoOnLogin, _userDb, _accountDb, _seasonInfo);
            await userUpdator.Run();
        }

        private async Task UserGetSetUp()
        {
            UserGet user = new UserGet(_accountInfo.UserId, _protoOnLogin, _userDb, _protoOnLogin.ChallengeStageList.CurrentSeason);
            await user.SetUp();

            var squareObjectIo = new SquareObjectIO(_userDb, _accountDb);
            _protoOnLogin.SquareObjectDonationCount = await squareObjectIo.GetDonationCount(_accountInfo.UserId);
        }

        private async Task GetEvents()
        {
            EventManager eventManager = new EventManager(_accountDb);
            _protoOnLogin.Events.AddRange(await eventManager.GetEventList());

            var challengeEventManager
                = ChallengeFactory.CreateEventChallengeManager(_accountDb, _userDb, _protoOnLogin.UserId, 0, 0);
            _protoOnLogin.EventsChallenge.AddRange(await challengeEventManager.GetEventList());
            _protoOnLogin.ChallengeStageList.TodayKnightLeagueWinCount = await challengeEventManager.GetTodayKnightLeagueWinCount();

            ServerSeason seasonManager = new ServerSeason(_accountDb);
            var challengeSeasonInfo = await seasonManager.GetChallengeSeasonInfo();
            _protoOnLogin.ChallengeStageList.NextChallengeStartDateTime = challengeSeasonInfo.NextSeasonStartDateTime.Ticks;
        }

        private async Task<ProtoBattlePlayingInfo> GetBattlePlayingInfo(uint userId)
        {
            var member = KeyMaker.GetMemberKey(userId);
            var redis = AkaRedis.AkaRedis.Connectors[AkaEnum.Server.GameServer].GetDatabase(0);
            var battlePlayingInfo = await GameBattleRedisJob.GetBattlePlayingInfoAsync(redis, member);

            if (battlePlayingInfo != null)
            {
                return new ProtoBattlePlayingInfo
                {
                    BattleServerIp = battlePlayingInfo.BattleServerIp,
                    RoomId = battlePlayingInfo.RoomId,
                    BattleServerPort = battlePlayingInfo.BattleServerPort.ToInt()
                };
            }
            return null;
        }

        private async Task SetFriendInfo(DBContext accountDb, DBContext db, uint userId)
        {
            var friendManager = new FriendManager();
            _protoOnLogin.Friends = await friendManager.GetFriendList(userId, accountDb, db);
            _protoOnLogin.RequestedFriends = await friendManager.GetRequestedFriendList(userId, accountDb, db);
            _protoOnLogin.RecommendFriends = await friendManager.GetRecommendFriendList(userId, accountDb, db);
        }

        private async Task UserInitDataUpdate()
        {
            if (Data.GetUserInitDatasList().LastEx().Version > _accountInfo.InitDataVersion)
            {
                var updateDatasList = Data.GetUserInitDatasList().SkipWhile(datas => datas.Version <= _accountInfo.InitDataVersion);

                var updatorList = updateDatasList.GroupBy(updateDatas => updateDatas.UserInitDataType)
                    .Select(dataTypeGroup =>
                    UserInitDataUpdator
                    .CreateUpdator(_accountInfo.UserId, _userDb, _accountDb, dataTypeGroup.Key, dataTypeGroup
                    .Select(updateDatas => updateDatas.TargetId).Distinct()));

                foreach (var updator in updatorList)
                    await updator.Run();
            }

            var lastUserInitData = Data.GetUserInitDatasList().LastEx();
            if (lastUserInitData.Version > _accountInfo.InitDataVersion)
                await SetUserInitDataVersion(_accountDb, _accountInfo.UserId, lastUserInitData.Version);
        }

        private async Task SetUserInitDataVersion(DBContext accountDb, uint userId, uint version)
        {
            var query = new StringBuilder();
            query.Append("UPDATE accounts SET initDataVersion = ")
                 .Append(version)
                 .Append(" WHERE userId = ")
                 .Append(userId)
                 .Append(";");

            await accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        private async Task<ServerSeasonInfo> SetSeasonInfo()
        {
            ServerSeason seasonManager = new ServerSeason(_accountDb);

            var knightleagueSeasonInfo = await seasonManager.GetKnightLeagbueSeasonInfoWithSeasonYearNum();
            _protoOnLogin.CurrentSeason = knightleagueSeasonInfo.CurrentSeason;
            _protoOnLogin.NextSeasonStartDateTime = knightleagueSeasonInfo.NextSeasonStartDateTime.Ticks;
            _protoOnLogin.SeasonYear = knightleagueSeasonInfo.SeasonYear;
            _protoOnLogin.SeasonYearNum = knightleagueSeasonInfo.SeasonYearNum;

            var seasonPassInfo = await seasonManager.GetSeasonPassInfo();
            _protoOnLogin.CurrentSeasonPass = seasonPassInfo.CurrentSeason;
            _protoOnLogin.CurrentSeasonPassStartDateTime = seasonPassInfo.CurrentSeasonStartDateTime.Ticks;
            _protoOnLogin.NextSeasonPassStartDateTime = seasonPassInfo.NextSeasonStartDateTime.Ticks;
            _protoOnLogin.PurchasedSeasons = await (new SeasonPassManager(_accountInfo.UserId, seasonPassInfo.CurrentSeason, _userDb))
                .GetBeforeAndCurrentPurchasedSeasonPassList();


            var seasonChallengeInfo = await seasonManager.GetChallengeSeasonInfo();
            _protoOnLogin.ChallengeStageList.CurrentSeason = seasonChallengeInfo.CurrentSeason;

            return seasonPassInfo;
        }

        private async Task UpdateLoginDateTime()
        {
            var query = new StringBuilder();
            query.Append("UPDATE accounts SET loginDateTime = '").Append(DateTime.UtcNow.ToTimeString())
                .Append("' WHERE userId = ").Append(_accountInfo.UserId).Append(";");
            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        private async Task SetPubSubServerInfo()
        {
            _protoOnLogin.PubSubServerIp
                = await ServerStatus.GetServerStatusInfo(_redis, RedisKeyType.ZPubSubServerState, _protoOnLogin.GroupCode);
            _protoOnLogin.PubSubServerPort = Config.GameServerConfig.PubSubServerPort;
        }
    }
}
