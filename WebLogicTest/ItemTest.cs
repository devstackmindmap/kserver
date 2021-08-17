using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class ItemTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task CreateTermMaterialGetTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(7))
                {
                    var coin = MaterialFactory.CreateTermMaterial(MaterialType.EventCoin, 7, 10, accountDb, db);
                    await coin.Get("Test");
                }
            }
        }

        [Test]
        public async Task CreateTermMaterialUseTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(7))
                {
                    var coin = MaterialFactory.CreateTermMaterial(MaterialType.EventCoin, 7, 10, accountDb, db);
                    await coin.Use("Test");
                }
            }
        }

        [Test]
        public async Task CreateTermMaterialGetTest2()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(7))
                {
                    var coin = MaterialFactory.CreateTermMaterial(MaterialType.EventBoxEnergy, 7, 10, accountDb, db);
                    await coin.Get("Test");
                }
            }
        }

        [Test]
        public async Task CreateTermMaterialUseTest2()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(7))
                {
                    var coin = MaterialFactory.CreateTermMaterial(MaterialType.EventBoxEnergy, 7, 10, accountDb, db);
                    await coin.Use("Test");
                }
            }
        }
    }
}
