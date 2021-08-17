using AkaEnum;
using Common.Entities.ServerStatus;
using CommonProtocol;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebServer
{
    public class WebServerStatus : BaseController
    {
        private HttpContext _context;
        private readonly StackExchange.Redis.IDatabase _redis = AkaRedis.AkaRedis.GetDatabase();

        public WebServerStatus(HttpContext context)
        {
            _context = context;
        }

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            if (await ServerStatus.IsServerDown(_redis)
                && false == await ServerStatus.IsDeveloperIp(_redis, _context.Connection.RemoteIpAddress.ToString()))
                return new ProtoResult { ResultType = ResultType.ServerDown };

             return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
