using AkaConfig;
using AkaDB.MySql;
using AkaEnum;
using System.Threading.Tasks;

namespace ProductUpdator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Initailize(args[0]);
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var product = new ProductUpdate(accountDb);
                await product.Run();
            }
        }

        private static async Task Initailize(string runMode)
        {
            Config.GameServerInitConfig(Server.GameServer, runMode);
            AkaDB.DBEnv.AllSetUp();
        }
    }
}
