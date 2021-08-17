using AkaDB.MySql;
using NUnit.Framework;
using System.Threading.Tasks;
using AkaEnum;
using Common.Entities.Deck;
using System.Data.Common;

namespace BattleLogicTest
{
    [TestFixture]
    public class RankTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task GetSumOfUnitsRankPointTest()
        {
            using (var db = new DBContext(15))
            {
                var dbSum = new DBGetSumOfUnitRankPointInBattleDeck
                {
                    Db = db,
                    DeckNum = 0,
                    ModeType = ModeType.PVP,
                    UserId = 15
                };

                AkaLogger.Logger.Instance().Info(await dbSum.ExecuteAsync());
            }
        }

        [Test]
        public async Task GetSumOfUnitsRankPointTest2()
        {
            using (var db = new DBContext(15))
            {
                var dbSum = new DBGetSumOfUnitRankPointInBattleDeck
                {
                    Db = db,
                    DeckNum = 0,
                    ModeType = ModeType.PVP,
                    UserId = 15
                };

                AkaLogger.Logger.Instance().Info(await dbSum.ExecuteAsync());
            }
        }

        public async Task UnitsTableDataSet(DBContext db, uint userId, uint id, uint rankLevel, int rankPoint)
        {
            var query = $"UPDATE units SET maxRankLevel={rankLevel}, currentSeasonRankPoint={rankPoint} " +
                $"WHERE userId={userId} AND id={id}";
            await db.ExecuteNonQueryAsync(query);
        }

        public async Task<DbDataReader> GetRankData(DBContext db, uint userId, uint id, uint rankLevel, int rankPoint)
        {
            var strUserId = userId.ToString();
            var strId = id.ToString();

            var query = $"SELECT maxRankLevel, currentSeasonRankPoint FROM units WHERE userId={strUserId} AND id={strId};";
            var cursor = await db.ExecuteReaderAsync(query);
            cursor.Read();
            return cursor;

        }
    }
}
