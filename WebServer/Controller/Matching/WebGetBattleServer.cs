using System.Threading.Tasks;
using AkaConfig;
using AkaEnum;
using AkaRedisLogic;
using Common;
using Common.Entities.ServerStatus;
using CommonProtocol;
using StackExchange.Redis;

namespace WebServer.Controller.Matching
{
    public class WebGetBattleServer : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetBattleServer;
            var redis = AkaRedis.AkaRedis.GetDatabase();

            var res = new ProtoOnGetBattleServer 
            {
                BattleServerPort = Config.GameServerConfig.BattleServerPort
            };

            var matchingServerInfo = await GetMatchingServerInfo(redis, req.GroupCode);

            res.MatchingServerIp = matchingServerInfo.ip;
            res.MatchingServerPort = matchingServerInfo.port;

            System.Console.WriteLine(res.MatchingServerIp);
            System.Console.WriteLine(res.MatchingServerPort);

            res.BattleServerIp = await ServerStatus.GetServerStatusInfo(redis, RedisKeyType.ZBattleServerState, req.GroupCode);

            System.Console.WriteLine(req.GroupCode);
            System.Console.WriteLine(res.BattleServerIp);
            System.Console.WriteLine(res.BattleServerPort);

            return res;
        }
        private async Task<(string ip, int port)> GetMatchingServerInfo(IDatabase redis, int groupCode)
        {
            var result = await MatchingServerInfoJob.GetServerStateInfo(redis, groupCode);

            if (result.Length > 0)
            {
                int matchingLine = (int)result[0].Element;

                if (Config.GameServerConfig.MatchingServerList.ContainsKey(groupCode))
                {
                    return
                        (Config.GameServerConfig.MatchingServerList[groupCode][matchingLine].ip,
                        Config.GameServerConfig.MatchingServerList[groupCode][matchingLine].port);
                }
            }
            return ("", 0);
        }
    }
}
