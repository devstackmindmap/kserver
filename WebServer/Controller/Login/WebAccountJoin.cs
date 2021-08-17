using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using AkaUtility;
using Common.Entities.Season;
using Common.Entities.User;
using CommonProtocol;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer
{
    public class WebAccountJoin : BaseController
    {
        private HttpContext _context;
        private readonly StackExchange.Redis.IDatabase _redis = AkaRedis.AkaRedis.GetDatabase();

        public WebAccountJoin(HttpContext context)
        {
            _context = context;
        }

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoAccountJoin;
            if (req == null)
                throw new Exception("ProtoLogin Cast Fail");

            var protoOnLogin = new ProtoOnLogin();

            if (IsInvalidParam(req, out protoOnLogin.ResultType))
                return protoOnLogin;

            var slangFilter = new SlangFilter();
            if (slangFilter.IsFiltered(req.NickName))
            {
                protoOnLogin.ResultType = ResultType.Slang;
                return protoOnLogin;
            }

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                ServerSeason seasonManager = new ServerSeason(accountDb);
                var seasonInfo = await seasonManager.GetKnightLeagueSeasonInfo();


                await accountDb.BeginTransactionCallback(async () =>
                {
                    var account = new Account(accountDb, req.SocialAccount, req.NickName);
                    
                    (string countryCode, int groupCode) countryInfo;
                    if (_context == null)
                        countryInfo = ("KR", 1);
                    else
                        countryInfo = await account.GetCountryInfo(_context.Connection.RemoteIpAddress.ToString());

                    var accountInfo = await account.GetAccountInfo(seasonInfo.CurrentSeason, countryInfo.countryCode);
                    var clanId = await Common.Entities.Clan.ClanManager.GetClanId(accountDb, accountInfo.UserId);

                    if (accountInfo.IsNicknameDuplicate)
                    {
                        protoOnLogin.ResultType = ResultType.NicknameDuplicate;
                        return false;
                    }

                    protoOnLogin.CountryCode = countryInfo.countryCode;
                    protoOnLogin.GroupCode = countryInfo.groupCode;

                    using (var userDb = new DBContext(accountInfo.UserId))
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            FirstMemberInitializer firstMemberInitializer
                            = new FirstMemberInitializer(userDb, accountInfo.UserId, GetCheatLevel(req.NickName, req.SocialAccount));
                            await firstMemberInitializer.FirstMemberInit();

                            var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, accountInfo.UserId,
                                UserAdditionalInfoType.RewardedRankSeason);
                            await userInfoChanger.Change(new RequestValue { StringValue = (seasonInfo.CurrentSeason).ToString() });

                            var userLoginInfo = new UserLoginInfo(accountDb, userDb, _redis, accountInfo, protoOnLogin);
                            await userLoginInfo.GetProtoOnLogin(req.PlatformType, req.LanguageType);

                            return true;
                        });

                        var clan = new Common.Clan(protoOnLogin.UserId, accountDb);
                        protoOnLogin.ClanProfileAndMembers = await clan.GetClanProfileAndMembers(clanId);

                        var pushKeyManager = new PushKeyManager(userDb, accountInfo.UserId, req.PushKey, 
                            req.PushAgree, req.NightPushAgree);
                        await pushKeyManager.PushKeyInit();
                    }
                    return true;
                });
            }

            Log.User.Join.Log(protoOnLogin.UserId, req.NickName, req.PushKey, req.PushAgree, req.NightPushAgree, req.TermsAgree);

            return protoOnLogin;
        }

        private bool IsInvalidParam(ProtoAccountJoin req, out ResultType resultType)
        {
            resultType = ResultType.Success;
            if (string.IsNullOrWhiteSpace(req.SocialAccount) || string.IsNullOrWhiteSpace(req.NickName))
            {
                resultType = ResultType.InvalidParameter;
                return true;
            }
            else if (req.TermsAgree != 1)
            {
                resultType = ResultType.DeniedTerms;
                return true;
            }

            return false;
        }

        private int GetCheatLevel(string nickName, string socialAccount)
        {
#if DEBUG
            if (AkaConfig.Config.RunMode != RunMode.Live.ToString() && nickName.Length > 5)             
            {
                var cheatName = "cheat";
                for(int i = 1; i <= 20; i++)
                {
                    var cheatLv = cheatName + i.ToString();
                    if (nickName.LastIndexOf(cheatLv) == nickName.Length - cheatLv.Length)
                        return i;
                }
            }
                
#endif
            return 0;
        }
    }
}
