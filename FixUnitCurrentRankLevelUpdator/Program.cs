using AkaConfig;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace FixUnitCurrentRankLevelUpdator
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
                await Initailize(args[0], dataVersion);
            }

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var fixUnitCurrentRankLevelUpdate = new FixUnitCurrentRankLevelUpdate(accountDb);

                var isUpdate = false;
                if (args.Length >= 3 && args[2] == "Update")
                    isUpdate = true;

                await fixUnitCurrentRankLevelUpdate.Run(isUpdate);
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

            AkaRedis.AkaRedis.AddServer(Config.Server, Config.GameServerConfig.GameRedisSetting.ServerSetting,
                Config.GameServerConfig.GameRedisSetting.Password);
        }
    }
}
