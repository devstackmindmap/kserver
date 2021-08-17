using AkaDB.MySql;
using AkaEnum;
using Common.Entities.ServerStatus;
using Common.Entities.User;
using CommonProtocol;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer
{
    public class WebLogin : BaseController
    {
        private HttpContext _context;
        private readonly StackExchange.Redis.IDatabase _redis = AkaRedis.AkaRedis.GetDatabase();

        public WebLogin(HttpContext context)
        {
            _context = context;
        }

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            if (await ServerStatus.IsServerDown(_redis)
                && false == await ServerStatus.IsDeveloperIp(_redis, _context.Connection.RemoteIpAddress.ToString()))
                return new ProtoOnLogin { ResultType = ResultType.ServerDown };

            var req = requestInfo as ProtoLogin;
            if (req == null)
                throw new Exception("ProtoLogin Cast Fail");

            var protoOnLogin = new ProtoOnLogin();

            if (IsInvalidParam(req, out protoOnLogin.ResultType))
                return protoOnLogin;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var account = new Account(accountDb, req.SocialAccount);
                var accountInfo = await account.GetAccountInfo();

                if (null == accountInfo)
                {
                    protoOnLogin.ResultType = ResultType.NotJoined;
                    return protoOnLogin;
                }

                if (accountInfo.LimitLoginDateTime > DateTime.UtcNow)
                {
                    protoOnLogin.ResultType = ResultType.LimitUser;
                    protoOnLogin.CountryCode = accountInfo.LimitLoginReason;
                    return protoOnLogin;
                }

                using (var userDb = new DBContext(accountInfo.UserId))
                {
                    var pushKeyManager = new PushKeyManager(userDb, accountInfo.UserId);
                    if (false == await pushKeyManager.IsAgreeTerms())
                    {
                        protoOnLogin.ResultType = ResultType.DeniedTerms;
                        return protoOnLogin;
                    }

                    var clanId = await Common.Entities.Clan.ClanManager.GetClanId(accountDb, accountInfo.UserId);

                    (string countryCode, int groupCode) countryInfo;
                    if (_context == null)
                        countryInfo = ("KR", 1);
                    else
                        countryInfo = await account.GetCountryInfo(_context.Connection.RemoteIpAddress.ToString());

                    protoOnLogin.CountryCode = accountInfo.CountryCode;
                    protoOnLogin.GroupCode = countryInfo.groupCode;
                    protoOnLogin.NoticeMessage = await GetNoticeMessage(accountDb);

                    await userDb.BeginTransactionCallback(async () =>
                    {
                        var userLoginInfo = new UserLoginInfo(accountDb, userDb, _redis, accountInfo, protoOnLogin);
                        await userLoginInfo.GetProtoOnLogin(req.PlatformType, req.LanguageType);
                        return true;
                    });

                    await pushKeyManager.UpdateLoginDateTime();

                    var clan = new Common.Clan(accountInfo.UserId, accountDb);
                    protoOnLogin.ClanProfileAndMembers = await clan.GetClanProfileAndMembers(clanId);
                    protoOnLogin.Wins = accountInfo.Wins;
                }
            }

            

            return protoOnLogin;
        }

        private bool IsInvalidParam(ProtoLogin req, out ResultType resultType)
        {
            resultType = ResultType.Success;
            if (string.IsNullOrWhiteSpace(req.SocialAccount))
            {
                resultType = ResultType.InvalidParameter;
                return true;
            }

            return false;
        }

        private async Task<string> GetNoticeMessage(DBContext accountDb)
        {
            using (var cursor = await accountDb.ExecuteReaderAsync(
                "SELECT `message` FROM _notice LIMIT 1;"))
            {
                if (false == cursor.Read())
                    return "";

                return (string)cursor["message"];
            }
        }
    }
}
