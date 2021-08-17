using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Season;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Friend;

namespace WebLogicTest
{
    public class SeasonTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task TestTest()
        {
            using (var db = new DBContext("AccountDBSetting"))
            {
                ServerSeason seasonManager = new ServerSeason(db);
                var seasonInfo = await seasonManager.GetKnightLeagueSeasonInfo();
            }
        }
    }
}
