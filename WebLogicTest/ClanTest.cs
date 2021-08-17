using AkaData;
using AkaDB.MySql;
using Common;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class ClanTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task CreateClanTest()
        {
            using (var db = new DBContext("AccountDBSetting"))
            {
                var createClan = new ClanCreate(153, db);
                await createClan.DBCreateClan("fff", 1, AkaEnum.ClanPublicType.Public, 1000,"KR", "HI");
            }
        }
    }
}
