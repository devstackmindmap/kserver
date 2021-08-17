using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Reward;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebLogicTest
{
    public class RewardTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task SimpleBoxRewardCallTest()
        {
            using (DBContext db = new DBContext(1090))
            {
                var rewards = await Reward.GetRewards(db, 1090, 1001, "Test");
            }
        }

        [Test]
        public async Task MailRewardTest()
        {
            using (DBContext db = new DBContext(7))
            {
                var rewards = await Reward.GetRewards(db, 7, 85101, "Test");
            }
        }

        [Test]
        public async Task FearBoxTest()
        {
            int tryCount = 0;
            bool isGet = false;
            using (DBContext accountDb = new DBContext("AccountDBSetting"))
            {
                using (DBContext db = new DBContext(10))
                {
                    while (true)
                    {
                        tryCount++;
                        var rewards = await Reward.GetRewardsByDb(10, 221021, accountDb, db,  "Test", 0);
                        foreach (var item in rewards)
                        {
                            if (item.ItemType == ItemType.UnitPiece && item.Count == 0 && item.ClassId == 1010)
                                isGet = true;
                        }

                        if (isGet)
                            break;
                    }
                    Logger.Instance().Info(tryCount);

                    //await DeleteUnit(db, 10, 1010);
                }
            }
        }

        private async Task DeleteUnit(DBContext userDb, uint userId, uint id)
        {
            await userDb.ExecuteNonQueryAsync("DELETE FROM units WHERE userId=" + userId + " AND id=" + id + ";");
        }
    }
}
