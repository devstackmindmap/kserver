using AkaConfig;
using AkaData;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace ClearTrashDataTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // args[0] : RunMode
            // args[1] : DataVersion
            // args[2] : ClearType (PastRanking, Ranking, ClearMatch)
            // args[3] : SeasonNum (if args[2] == Ranking)
            if (args.Length == 0)
            {
                await Initailize("Dylan2", 0);
            }
            else
            {
                var dataVersion = args.Length >= 2 ? Int32.Parse(args[1]) : 0;
                await Initailize(args[0], dataVersion);
            }

            var run = new ClearTrashData();
            await run.Run(args);
        }

        private static async Task Initailize(string runMode, int dataVersion)
        {
            Config.GameServerInitConfig(Server.GameServer, runMode);
            AkaRedis.AkaRedis.AddServer(Server.GameServer, Config.GameServerConfig.GameRedisSetting.ServerSetting,
                Config.GameServerConfig.GameRedisSetting.Password);

            Config.MatchingServerInitConfig(Server.MatchingServer, runMode);
            AkaRedis.AkaRedis.AddServer(Server.MatchingServer, Config.MatchingServerConfig.MatchingRedisSetting.ServerSetting,
                Config.MatchingServerConfig.MatchingRedisSetting.Password);

            AkaDB.DBEnv.AllSetUp();

            var loader = new FileLoader(FileType.Table, runMode, dataVersion);
            var fileList = await loader.GetFileLists();
            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);

        }
    }
}
