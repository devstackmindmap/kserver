using AkaDB.MySql;
using Common.Entities.Season;
using NUnit.Framework;
using System.Threading.Tasks;
using Common.Entities.Challenge;
using AkaData;
using CommonProtocol;

//SELECT* FROM knightrun.challenge_stage;
//SELECT* FROM knightrun.user_info WHERE userId=7;
//SELECT* FROM challenge_stage_first_clear_user;
//SELECT* FROM knightrun.challenge_first_clear_decks;

//DELETE FROM challenge_stage_first_clear_user;
//DELETE FROM knightrun.challenge_stage;
//DELETE FROM knightrun.challenge_first_clear_decks;
//UPDATE `knightrun`.`user_info` SET `challengeClearCount`='0' WHERE  `userid`=7;

// Test1
// Day1 Difficult 1 클리어
// Day1 Reset
// Day2 Difficult 2 클리어
// Day2 Difficult 2 클리어

// Test2
// Day1 Difficult 1 클리어
// Day2 Difficult 2 클리어
// Day1 Reset
// Day1 Difficult 1 클리어
// Day2 Difficult 2 클리어

namespace BattleLogicTest
{
    [TestFixture]
    public class ChallengeTest
    {
        private uint _userId;
        private uint _season;
        private int _day;
        private int _difficultLevel;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);

            _season = 1;
            _userId = 7;
            _day = 2;
            _difficultLevel = 1;
        }

        [Test]
        public async Task Test01_Clear()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateChallengeManager(accountDb, userDb, _userId, _season, _day, _difficultLevel);
                    var rewards = await challenge.Clear(0);
                    Assert.True(rewards != null);
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
                    var challenge = ChallengeFactory.CreateChallengeManager(accountDb, userDb, _userId, _season, _day, _difficultLevel);
                    Assert.True(await challenge.ResetReward());
                }
            }
        }

        [Test]
        public async Task Test03_GetStages()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateChallengeManager(accountDb, userDb, _userId, _season, _day, _difficultLevel);
                    var stages = await challenge.GetStageList();
                }
            }
        }

        [Test]
        public async Task Test04_GetChallengeFirstClearUser()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var manager
                    = ChallengeFactory.CreateChallengeManager
                    (accountDb, null, _userId, _season, _day, _difficultLevel);

                var userId = await manager.GetFirstClearUserId();
                if (false == userId.HasValue)
                    return;

                using (var userDb = new DBContext(userId.Value))
                {
                    var deck = new WebLogic.Deck.Deck(userDb, _userId);
                    var deckInfo = new ProtoOnGetDeckWithNickname
                    {
                        DeckElements = await deck.GetRecentDeck(await manager.GetFirstClearUser(userDb, userId.Value))
                    };
                }
            }
        }

        [Test]
        public async Task IsValidDayCheckTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var challenge = ChallengeFactory.CreateChallengeManager(accountDb, null, _userId, _season, _day, _difficultLevel);
                Assert.True(await challenge.IsValidDay());
            }
        }

        [Test]
        public async Task IsValidFlowCheckTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateChallengeManager(accountDb, userDb, _userId, _season, _day, _difficultLevel);
                    Assert.True(await challenge.IsBeforeStageClear());
                }
            }
        }

        [Test]
        public async Task SaveFirstClearTest()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var challenge = ChallengeFactory.CreateChallengeManager(accountDb, userDb, _userId, _season, _day, _difficultLevel);
                    await challenge.SaveFirstClear(0);
                }
            }
        }

        [Test]
        public async Task IsValidDateTimeForEventCoin()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    //Assert.True(await IsValidFlow(accountDb, userDb, 4, 2));
                }
            }
        }

        private async Task<ServerSeasonInfo> GetSeasonInfo(DBContext db)
        {
            var serverSeason = new ServerSeason(db);
            return await serverSeason.GetKnightLeagueSeasonInfo();
        }

        [Test]
        public void GetDataChallengeTest()
        {
            var data = Data.GetDataChallenge(1, 1);
            
        }

        [Test]
        public void GetRoundTest()
        {
            var data1 = Data.GetStageRound(90001, 0);
            var data2 = Data.GetStageRound(90001, 1);
        }
    }
}
