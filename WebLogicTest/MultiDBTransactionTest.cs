using AkaDB.MySql;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class MultiDBTransactionTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task Run()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                await accountDb.BeginTransactionCallback(async () =>
                {
                    await accountDb.ExecuteNonQueryAsync("INSERT INTO test (seq, count, num) VALUES (1, 1, 1);");

                    using (var userDb = new DBContext(1))
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            await userDb.ExecuteNonQueryAsync("INSERT INTO test (testId, num) VALUES (2, 2);");
                            return true;
                        });
                    }

                    return true;
                });
            }
        }
    }
}

