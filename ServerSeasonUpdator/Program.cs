using AkaConfig;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaThreading;
using System;
using System.Threading.Tasks;

namespace ServerSeasonUpdator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AntiDuplicator.AppRunning();

            var dataVersion = args.Length >= 2 ? Int32.Parse(args[1]) : 0;
            await Initailize(args[0], dataVersion);

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var season = new ServerSeasonUpdate(accountDb);
                await season.Run();
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
        }
    }
}
