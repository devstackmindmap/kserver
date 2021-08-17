using AkaDB.MySql;
using Common.Entities.Charger;
using Common.Entities.Item;
using NUnit.Framework;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;
using AkaData;
using Common.Entities.InfusionBox;
using AkaEnum;
using Common;
using System.Collections.Generic;
using CommonProtocol;
using Common.Pass;
using Common.Entities.Season;

namespace WebLogicTest
{
    public class ChargetTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [TestCase((uint)7, 3, -20, -10, false, false, 0, 7, 0)]
        [TestCase((uint)7, 3, -20, -10, true, false, 3, 4, 0)]
        [TestCase((uint)7, 3, -10, -10, false, true, 0, 3,0)]
        [TestCase((uint)7, 3, -27, -27, false, true, 0, 10, 0)]
        [TestCase((uint)7, 3, -27, -27, true, false, 0, 10, 600)]
        public async Task ChargetUpdateTest(
            uint userId,
            int currentSeason,
            int recentUpdateDateTimeForAddHour,
            int currentSeasonStartDateTimeForAddHour,
            bool purchasedBeforeSeasonPass,
            bool purchasedCurrentSeasonPass,
            int beforeCount,
            int currentCount,
            int setUserEnergy)
        {
            await TestDataSet(userId, currentSeason, recentUpdateDateTimeForAddHour, currentSeasonStartDateTimeForAddHour,
                purchasedBeforeSeasonPass, purchasedCurrentSeasonPass, setUserEnergy);

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(7))
                {
                    var serverSeason = new ServerSeason(accountDb);
                    var seasonInfo = await serverSeason.GetSeasonPassInfo();
                    var purchasedSeasons
                        = await (new SeasonPassManager(userId, seasonInfo.CurrentSeason, db))
                        .GetBeforeAndCurrentPurchasedSeasonPassList();
                    var charger = ChargerFactory.CreateCharger(userId, InfusionBoxType.LeagueBox, db, accountDb, seasonInfo, purchasedSeasons);
                    await charger.Update();
                }
            }

            var resultEnergy = await GetUserEnergy(userId);
            if (purchasedBeforeSeasonPass == false && purchasedCurrentSeasonPass == false)
            {
                Assert.True(resultEnergy ==
                ((int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET).Value * beforeCount
                + (int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET).Value * currentCount));
            }
            else if (purchasedBeforeSeasonPass == true && purchasedCurrentSeasonPass == false)
            {
                if (setUserEnergy >= (int)Data.GetConstant(AkaEnum.DataConstantType.MAX_ENERGY).Value)
                {
                    Assert.True(resultEnergy == (int)Data.GetConstant(AkaEnum.DataConstantType.MAX_ENERGY).Value);
                }
                else
                {
                    Assert.True(resultEnergy ==
                ((int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET_PREMIUM).Value * beforeCount
                + (int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET).Value * currentCount));
                }
            }
            else if (purchasedBeforeSeasonPass == false && purchasedCurrentSeasonPass == true)
            {
                Assert.True(resultEnergy ==
                ((int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET).Value * beforeCount
                + (int)Data.GetConstant(AkaEnum.DataConstantType.ENERGY_QUANTITY_EACH_GET_PREMIUM).Value * currentCount));
            }
        }

        [Test]
        public async Task InsertSeasonPassTest()
        {
            using (var db = new DBContext(7))
            {
                var seasonPass = ItemFactory.CreateItem(AkaEnum.ItemType.SeasonPass, 7, 1002, db);
                await seasonPass.Get("GetSeasonPassTest");
            }
        }

        private async Task TestDataSet(
            uint userId,
            int currentSeason, 
            int recentUpdateDateTimeForAddHour, 
            int currentSeasonStartDateTimeForAddHour,
            bool purchasedBeforeSeasonPass,
            bool purchasedCurrentSeasonPass,
            int setUserEnergy)
        {
            var utcNow = DateTime.UtcNow;
            var recentUpdateDateTime = utcNow.AddHours(recentUpdateDateTimeForAddHour);
            var currentSeasonStartDateTime = utcNow.AddHours(currentSeasonStartDateTimeForAddHour);
            var beforeSeasonStartDateTime = currentSeasonStartDateTime.AddDays(-15);
            var nextSeasonStartDateTime = currentSeasonStartDateTime.AddDays(15);

            using (var userDb = new DBContext(userId))
            {
                var query = new StringBuilder();
                query.Append("UPDATE infusion_boxes SET userEnergy =").Append(setUserEnergy).Append(", userEnergyRecentUpdateDatetime = '")
                    .Append(recentUpdateDateTime.ToTimeString()).Append("' WHERE userId=").Append(userId).Append(";");

                await userDb.ExecuteNonQueryAsync(query.ToString());

                query.Clear();
                query.Append("DELETE FROM get_seasonpass WHERE userId=").Append(userId).Append(";");
                await userDb.ExecuteNonQueryAsync(query.ToString());

                if (purchasedBeforeSeasonPass)
                {
                    query.Clear();
                    query.Append("INSERT INTO get_seasonpass (userId, seasonPassType, season) VALUES (")
                        .Append(userId).Append(",").Append(2).Append(",").Append(currentSeason - 1).Append(");");
                    await userDb.ExecuteNonQueryAsync(query.ToString());
                }

                if (purchasedCurrentSeasonPass)
                {
                    query.Clear();
                    query.Append("INSERT INTO get_seasonpass (userId, seasonPassType, season) VALUES (")
                        .Append(userId).Append(",").Append(2).Append(",").Append(currentSeason).Append(");");
                    await userDb.ExecuteNonQueryAsync(query.ToString());
                }
            }

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var query = new StringBuilder();
                query.Append("UPDATE _common SET commonValue=").Append(currentSeason)
                    .Append(", commonValue3=").Append(currentSeason).Append(", commonValue6=").Append(currentSeason - 1)
                    .Append(", commonBeforeStartDateTime='").Append(beforeSeasonStartDateTime.ToTimeString())
                    .Append("', commonStartDateTime='").Append(currentSeasonStartDateTime.ToTimeString())
                    .Append("', commonNextStartDateTime='").Append(nextSeasonStartDateTime.ToTimeString())
                    .Append("' WHERE commonId=4;");

                await accountDb.ExecuteNonQueryAsync(query.ToString());
            }
        }

        private async Task<int> GetUserEnergy(uint userId)
        {
            var query = new StringBuilder();
            query.Append("SELECT userEnergy FROM infusion_boxes WHERE userId=").Append(userId).Append(";");

            using (var userDb = new DBContext(userId))
            {
                using (var cursor = await userDb.ExecuteReaderAsync(query.ToString()))
                {
                    if (false == cursor.Read())
                        Assert.Fail();

                    return (int)cursor["userEnergy"];
                }
            }
        }

        [TestCase((uint)7, 3, -27, -27, true, false, 0, 10, 600, BattleResultType.Win)]
        public async Task InfusionTest(uint userId,
            int currentSeason,
            int recentUpdateDateTimeForAddHour,
            int currentSeasonStartDateTimeForAddHour,
            bool purchasedBeforeSeasonPass,
            bool purchasedCurrentSeasonPass,
            int beforeCount,
            int currentCount,
            int setUserEnergy,
            BattleResultType battleResultType)
        {

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(userId))
                {
                    var eventManager = new EventManager(accountDb);
                    var infusion = await InfusionBoxInfusionFactory.CreateInfusionBoxInfusion(userId, InfusionBoxType.LeagueBox, userDb, accountDb);

                    var additionalEnergy = GetAdditionalInfusionDefiniteEnergy();
                    var isEnergyDoubleEvent = await eventManager.IsInEventProgress(EventType.DoubleEnergy);
                    var newInfusionInfo
                        = await infusion.GetNewInfusionBox(
                            battleResultType == BattleResultType.Win ? true : false,
                            additionalEnergy,
                            await eventManager.IsInEventProgress(EventType.DoubleEnergy));
                    await infusion.SetNewInfusionBox(newInfusionInfo);
                }
            }  
        }

        private int GetAdditionalInfusionDefiniteEnergy()
        {
            int sum = 0;
            Dictionary<RewardCategoryType, List<ProtoItemResult>> ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>();
            foreach (var items in ItemResults)
            {
                foreach (var item in items.Value)
                {
                    if (item.ItemType == ItemType.Energy)
                        sum += item.Count;
                }
            }
            return sum;
        }

        [Test]
        public async Task BattleWinInfusionTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var eventManager = new EventManager(accountDb);
                    var infusion = await InfusionBoxInfusionFactory.CreateInfusionBoxInfusion(7, InfusionBoxType.LeagueBox, userDb, accountDb);

                    var additionalEnergy = GetAdditionalInfusionDefiniteEnergy();
                    var isEnergyDoubleEvent = await eventManager.IsInEventProgress(EventType.DoubleEnergy);
                    var newInfusionInfo
                        = await infusion.GetNewInfusionBox(
                            true,
                            additionalEnergy,
                            await eventManager.IsInEventProgress(EventType.DoubleEnergy));
                    await infusion.SetNewInfusionBox(newInfusionInfo);
                }
            }
        }

        [Test]
        public async Task InfusionBoxRecentlyTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var serverSeason = new ServerSeason(accountDb);
                    var seasonInfo = await serverSeason.GetSeasonPassInfo();
                    var purchasedSeasons
                        = await (new SeasonPassManager(7, seasonInfo.CurrentSeason, userDb))
                        .GetBeforeAndCurrentPurchasedSeasonPassList();

                    var charger 
                        = ChargerFactory.CreateCharger(7, InfusionBoxType.LeagueBox, userDb, accountDb, seasonInfo, purchasedSeasons);
                    await charger.Update();
                    await charger.UpdateChargerDataNowDateTime();
                }
            }
        }

        [Test]
        public async Task InfusionBoxUpdateTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var serverSeason = new ServerSeason(accountDb);
                    var seasonInfo = await serverSeason.GetSeasonPassInfo();
                    var purchasedSeasons
                        = await (new SeasonPassManager(7, seasonInfo.CurrentSeason, userDb))
                        .GetBeforeAndCurrentPurchasedSeasonPassList();

                    var charger = ChargerFactory.CreateCharger(7, InfusionBoxType.LeagueBox, userDb, accountDb, seasonInfo, purchasedSeasons);
                    await charger.Update();
                }
            }
        }

    }
}
