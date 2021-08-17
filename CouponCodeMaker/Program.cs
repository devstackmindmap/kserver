using AkaConfig;
using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace CouponCodeMaker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Initailize(args[0]);
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var make = new CouponCodeMake(accountDb, Int32.Parse(args[1]), args[2]
                    , UInt32.Parse(args[3]), Int32.Parse(args[4]), Int32.Parse(args[5]));
                await make.Run();
            }
        }

        private static async Task Initailize(string runMode)
        {
            Config.GameServerInitConfig(Server.GameServer, runMode);
            AkaDB.DBEnv.AllSetUp();
        }
    }
}
