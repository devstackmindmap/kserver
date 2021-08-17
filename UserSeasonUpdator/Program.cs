using AkaConfig;
using AkaData;
using AkaEnum;
using AkaThreading;
using System;
using System.Threading.Tasks;

namespace UserSeasonUpdator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AntiDuplicator.AppRunning();

            if (args.Length == 0)
            {
               await Initailize("Dylan2", 0);
            }
            else
            {
                var dataVersion = args.Length >= 2 ? Int32.Parse(args[1]) : 0;
                await Initailize(args[0], dataVersion);
            }

            UserSeasonUpdate clanRankUpdate = new UserSeasonUpdate();
            await clanRankUpdate.Run();
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


// Accounts 순환
// 서버시즌 유저시즌 차이가 있으면 작업진행
    //SeasonUpdate 참조
// 작업완료되면 체크
// 모든 순환 완료 되면 배치 완료
