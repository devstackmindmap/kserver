using AkaDB.MySql;
using Common.Entities.Season;
using NUnit.Framework;
using System.Threading.Tasks;
using Common.Entities.Challenge;
using Common.CommonType;


//SELECT* FROM knightrun.challenge_event_stage;
//SELECT* FROM knightrun.user_info WHERE userId=7;
//SELECT* FROM challenge_event_stage_first_clear_user;
//SELECT* FROM knightrun.challenge_first_clear_decks_event;

//DELETE FROM challenge_event_stage_first_clear_user;
//DELETE FROM knightrun.challenge_event_stage;
//DELETE FROM knightrun.challenge_first_clear_decks_event;
//UPDATE `knightrun`.`user_info` SET `challengeClearCount`='0' WHERE  `userid`=7;

namespace BattleLogicTest
{
    [TestFixture]
    public class ChallengeEventTest
    {
        private uint _userId;
        private uint _challengeEventId;
        private int _difficultLevel;
        private int _challengeEventNum;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);

            _userId = 7;
            _challengeEventId = 1;
            _difficultLevel = 1;
        }

        [Test]
        public async Task Test01_ClearTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateEventChallengeManager(accountDb, userDb, _userId, _challengeEventId, _difficultLevel);
                    var rewards = await challenge.Clear(0);
                }
            }
        }

        [Test]
        public async Task Test02_ResetTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateEventChallengeManager(accountDb, userDb, _userId, _challengeEventId, _difficultLevel);
                    Assert.True(await challenge.ResetReward());
                }
            }
        }

        [Test]
        public async Task Test03_IsValidFlowCheckTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateEventChallengeManager(accountDb, userDb, _userId, _challengeEventId, _difficultLevel);
                    Assert.True(await challenge.IsValidFlow());
                }
            }
        }

        [Test]
        public async Task Test04_IsInEventCheckTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var challenge = ChallengeFactory.CreateEventChallengeManager(accountDb, null, _userId, _challengeEventId, _difficultLevel);
                Assert.True(await challenge.IsInEvent(true));
            }
        }

        [Test]
        public async Task SaveFirstClearTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateEventChallengeManager(accountDb, userDb, _userId, _challengeEventId, _difficultLevel);
                    await challenge.SaveFirstClear(0);
                }
            }
        }

        //[Test]
        //public async Task IsValidDateTimeForEventCoin()
        //{
        //    using (var accountDb = new DBContext("AccountDBSetting"))
        //    {
        //        using (var userDb = new DBContext(7))
        //        {
        //            //Assert.True(await IsValidFlow(accountDb, userDb, 4, 2));
        //        }
        //    }
        //}

        //private async Task<ServerSeasonInfo> GetSeasonInfo(DBContext db)
        //{
        //    var serverSeason = new ServerSeason(db);
        //    return await serverSeason.GetKnightLeagueSeasonInfo();
        //}
    }
}
