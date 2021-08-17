using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebServer.Controller.Friend;

namespace WebLogicTest
{
    public class DBAsyncRedisTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task Test()
        {
            using (var db = new DBContext("AccountDBSetting"))
            {
                var query = new StringBuilder();
                for (int i = 0; i < 10000; i++)
                {
                    var redis = AkaRedis.AkaRedis.GetDatabase();
                    query.Clear();
                    query.Append($"INSERT INTO test (seq, count, num) VALUES ({i}, {i}, {i}) " +
                        $"ON DUPLICATE KEY UPDATE count = {i}, num = {i};");
                    await db.ExecuteNonQueryAsync(query.ToString());
                    await redis.SortedSetAddAsync("test", i, 100);
                }
            }
            
        }
    }
}
