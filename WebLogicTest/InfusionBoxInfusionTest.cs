using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common;
using Common.Entities.InfusionBox;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using WebServer.Controller.Box;

namespace WebLogicTest
{
    [TestFixture]
    class InfusionBoxInfusionTest
    {
        private uint _userId = 16;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task InfutionToBoxTest1()
        {
            var groups = Data.GetProbabilityGroup(ConstValue.NORMAL_LEAGUE_PROBABILITY_GROUP_ID);
            var pickedBoxIndex = AkaRandom.Random.ChooseIndexRandomlyInSumOfProbability(groups);

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(_userId))
                {
                    var boxEnergy = 0;
                    var userEnergy = 40;
                    var userBonusEnergy = 0;
                    var dateTime = DateTime.UtcNow;
                    await InfusionTestInit(groups[pickedBoxIndex].ElementId, boxEnergy, userEnergy, userBonusEnergy, dateTime);
                    var infusion = await GetInfusionBoxInfusion(_userId, db, accountDb);
                    var newInfusionBox = await infusion.GetNewInfusionBox(true, 0, false);
                    await infusion.SetNewInfusionBox(newInfusionBox);
                    Assert.True(
                        newInfusionBox.NewTotalBoxEnergy == boxEnergy + (int)Data.GetConstant(DataConstantType.WIN_INFUSION_ENERGY).Value &&
                        newInfusionBox.NewTotalUserBonusEnergy == userBonusEnergy &&
                        newInfusionBox.NewTotalUserEnergy == userEnergy - (int)Data.GetConstant(DataConstantType.WIN_INFUSION_ENERGY).Value &&
                        newInfusionBox.UseUserEnergy == (int)Data.GetConstant(DataConstantType.WIN_INFUSION_ENERGY).Value &&
                        newInfusionBox.UseUserBonusEnergy == userBonusEnergy
                        );
                }
            }
        }

        public async Task<IInfusionBoxInfusion> GetInfusionBoxInfusion(uint userId, DBContext db, DBContext accountDb)
        {
            return await InfusionBoxInfusionFactory.CreateInfusionBoxInfusion(userId, InfusionBoxType.LeagueBox, db, accountDb);
        }

        private async Task InfusionTestInit(uint boxId, int boxEnergy, int userEnergy, int userBonusEnergy, DateTime dateTime)
        {
            using (var db = new DBContext(_userId))
            {
                var query = "INSERT INTO {0} (userId, type, id, {1}, {2}, {3}, {4}) VALUES ({5}, {6}, {7}, {8}, {9}, {10}, '{11}') ON DUPLICATE KEY UPDATE id={7}, {1}={8}, {2}={9}, {3}={10}, {4}='{11}';"
                    .FormatWith(TableName.INFUSION_BOX,
                    ColumnName.BOX_ENERGY, ColumnName.USER_ENERGY, ColumnName.USER_BONUS_ENERGY, ColumnName.USER_ENERGY_RECENT_UPDATE_DATETIME,
                    _userId, (byte)InfusionBoxType.LeagueBox, boxId, boxEnergy, userEnergy, userBonusEnergy, dateTime.ToTimeString());

                await db.ExecuteNonQueryAsync(query);
            }
        }

        [Test]
        public async Task InfutionToBoxOpenTest()
        {
            var web = new WebInfusionBoxOpen();
            var res = await web.DoPipeline(new ProtoInfusionBoxOpen
            {
                Type = InfusionBoxType.LeagueBox,
                UserId = 7
            });
        }
    }
}
