using AkaConfig;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace FixRewardedRankReward
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await Initailize("Dylan2", 0);
            }
            else
            {
                var dataVersion = args.Length >= 2 ? Int32.Parse(args[1]) : 0;
                if (args[0] != "Live")
                    return;
                 
                await Initailize(args[0], dataVersion);
            }
            
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var fixUnitCurrentRankLevelUpdate = new FixRewardedRankReward(accountDb, 
                    args.Length == 0 ? "Dylan2" : args[0],
                    args.Length == 3 ? UInt32.Parse(args[2]) : 0);
                await fixUnitCurrentRankLevelUpdate.Run();
            }
        }

        private static async Task Initailize(string runMode, int dataVersion)
        {
            Config.GameServerInitConfig(Server.GameServer, runMode);
            AkaDB.DBEnv.AllSetUp();

            var loader = new FileLoader(FileType.Table, runMode, dataVersion);
            var fileList = await loader.GetFileLists();
            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);

            Console.WriteLine(Config.Server);
            Console.WriteLine(Config.GameServerConfig.GameRedisSetting.ServerSetting.ip);
            Console.WriteLine(Config.GameServerConfig.GameRedisSetting.ServerSetting.port);
            AkaRedis.AkaRedis.AddServer(Config.Server, Config.GameServerConfig.GameRedisSetting.ServerSetting,
                Config.GameServerConfig.GameRedisSetting.Password);
        }
    }
}
