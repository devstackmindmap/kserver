using AkaDB.MySql;
using Common.Entities.User;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WebLogicTest
{
    class PushTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task PushKeyInitTest()
        {
            using (var userDb = new DBContext(1))
            {
                var manager = new PushKeyManager(userDb, 1, "fdsafdsa", 1, 0);
                await manager.PushKeyInit();
            }
        }

        [Test]
        public async Task UpdateLoginDateTimeTest()
        {
            using (var userDb = new DBContext(1))
            {
                var manager = new PushKeyManager(userDb, 1);
                await manager.UpdateLoginDateTime();
            }
        }

        [Test]
        public async Task UpdatePushAgreeTest()
        {
            using (var userDb = new DBContext(1))
            {
                var manager = new PushKeyManager(userDb, 1, 0);
                await manager.UpdatePushAgree();
            }
        }

        [Test]
        public async Task UpdateNightPushAgreeTest()
        {
            using (var userDb = new DBContext(1))
            {
                var manager = new PushKeyManager(userDb, 1, 1);
                await manager.UpdateNightPushAgree();
            }
        }
    }
}
