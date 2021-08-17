using System.Collections.Generic;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaUtility;
using Common.Entities.Battle;
using Common.Entities.Clan;
using Common.Entities.Season;
using CommonProtocol;
using WebLogic.Friend;

namespace WebServer.Controller.Battle
{
    public class WebGetBattleResultRank : WebGetBattleResultCommon
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBattleResult;
            var protoOnBattleResultList = new ProtoOnBattleResultRankList();

            _battleType = req.BattleType;

            protoOnBattleResultList.BattleResult = await GetEachUserJob(req.PlayerInfoList);

            return protoOnBattleResultList;
        }

        private async Task<Dictionary<uint, ProtoOnBattleResultRank>> GetEachUserJob(List<ProtoBattleResultPlayerInfo> playerInfoList)
        {
            Dictionary<uint, ProtoOnBattleResultRank> battleResult = new Dictionary<uint, ProtoOnBattleResultRank>();

            if (playerInfoList.Count == 2 )
            {
                var player1Result = await GetSeasonInfoWithTeamRankPoint(playerInfoList[0]);
                var player2Result = await GetSeasonInfoWithTeamRankPoint(playerInfoList[1]);

                battleResult.Add(playerInfoList[0].UserId,
                await GetBattleResult(player1Result.battleResult, player2Result.teamRankPoint, playerInfoList[0], playerInfoList[1].UserId));

                battleResult.Add(playerInfoList[1].UserId,
                await GetBattleResult(player2Result.battleResult, player1Result.teamRankPoint, playerInfoList[1], playerInfoList[0].UserId));
            }
            else if (playerInfoList.Count == 1)
            {
                var player1Result = await GetSeasonInfoWithTeamRankPoint(playerInfoList[0]);
                battleResult.Add(playerInfoList[0].UserId,
                await GetBattleResult(player1Result.battleResult, -1, playerInfoList[0], 0));
            }
            else
            {
                throw new System.Exception();
            }
            return battleResult;
        }


        private async Task<(ProtoOnBattleResultRank battleResult, int teamRankPoint)> GetSeasonInfoWithTeamRankPoint(ProtoBattleResultPlayerInfo playerInfo)
        {
            ProtoOnBattleResultRank protoOnBattleResult = new ProtoOnBattleResultRank();
            uint serverCurrentSeason = 0;
            int teamRankPoint = 0;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(playerInfo.UserId))
                {
                    IBattleResultManager resultManager = CreateBattleResultManager(accountDb, userDb, playerInfo, 0, 0, protoOnBattleResult);

                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            if (_battleType == BattleType.LeagueBattle || _battleType == BattleType.LeagueBattleAi)
                            {
                                serverCurrentSeason = await resultManager.SeasonJob();
                                protoOnBattleResult.CurrentSeason = serverCurrentSeason;
                            }
                            else
                            {
                                ServerSeason seasonManager = new ServerSeason(accountDb);
                                var seasonInfo = await seasonManager.GetKnightLeagueSeasonInfo();
                                protoOnBattleResult.CurrentSeason = seasonInfo.CurrentSeason;
                            }
                            return true;

                        });
                        return true;
                    });

                    accountDb.Commit();
                    userDb.Commit();

                    teamRankPoint = await resultManager.GetSumOfUnitsRankPoint();
                }
            }

            return (protoOnBattleResult, teamRankPoint);
        }

        private async Task<ProtoOnBattleResultRank> GetBattleResult(ProtoOnBattleResultRank protoOnBattleResult, int enemyTeamRankPoint, ProtoBattleResultPlayerInfo playerInfo, uint enemyUserId)
        {
            var serverCurrentSeason = protoOnBattleResult.CurrentSeason;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(playerInfo.UserId))
                {
                    IBattleResultManager resultManager = CreateBattleResultManager(accountDb, userDb, playerInfo, enemyTeamRankPoint, 0, protoOnBattleResult);
                    IBattleResultManager userExpInfoManager = CreateUserExpInfoManager(userDb, playerInfo, protoOnBattleResult);

                    await userDb.BeginTransactionCallback(async () =>
                    {
                        await accountDb.BeginTransactionCallback(async () =>
                        {
                            await UpdateSeasonPass(playerInfo.UserId, accountDb, userDb);
                            return true;
                        });
                        await resultManager.BattleResultJob(playerInfo.BattleResultType);
                        await userExpInfoManager.BattleResultJob(playerInfo.BattleResultType);

                        protoOnBattleResult.NewInfusionBox
                        = await resultManager.InfusionBoxJob(playerInfo.BattleResultType);

                        return true;
                    });

                    if (resultManager.HasRedisJob())
                    {
                        var clanId = await ClanManager.GetClanId(accountDb, playerInfo.UserId);
                        var clanCountryCode = await ClanManager.GetClanInfo(accountDb, clanId);
                        if (false == await resultManager.RedisJob(serverCurrentSeason, clanId, clanCountryCode))
                            AkaLogger.Log.Battle.BattleResultRedisFail.Log(playerInfo.UserId, protoOnBattleResult.ChangedRankPoint);
                    }

                    protoOnBattleResult.RecommendFriend = await AddRecommendFriend(playerInfo.UserId, enemyUserId, accountDb, userDb);
                }
            }

            return protoOnBattleResult;
        }

        private IBattleResultManager CreateUserExpInfoManager(DBContext db, ProtoBattleResultPlayerInfo playerInfo, ProtoOnBattleResult protoOnBattleResult)
        {
            return new UserLevel(db, _battleType, playerInfo.UserId, protoOnBattleResult);
        }

        private async Task<ProtoFriendInfo> AddRecommendFriend(uint userId, uint friendId, DBContext accountDB, DBContext db)
        {
            if (friendId == 0)
                return null;

            var friendManager = new FriendManager();

            if (await friendManager.IsAlreadyFriend(userId, friendId, db))
                return null;

            await friendManager.AddRecommendFriend(userId, friendId, db);
            return await friendManager.GetUserInfo(friendId, accountDB);
        }
    }
}
