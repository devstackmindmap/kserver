using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaConfig;
using AkaData;
using AkaEnum;
using AkaRedisLogic;
using NUnit.Framework;



namespace BattleLogicTest
{
    [TestFixture]
    [Description("Area = 1, UserId = 15 - Default case ")]
    public partial class RoguelikeTest
    {
        private string sessionid = $"storymodetest-session:{KeyMaker.GetNewRoomId()}";

        private readonly int redisDbIndex = 10;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [OneTimeTearDown]
        [Ignore("--")]
        public void CleanUp()
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
        //    redis.KeyDelete(RedisKeyType.HRoomIdList.ToString());

        //    memidList.ForEach(memberid => redis.HashDelete(RedisKeyType.HRoomIdList.ToString(), memberid));

            AkaRedis.AkaRedis.Connector.Close();
        }


        [Order(1)]
        [TestCase((uint)10101, (uint)4)]
        public void GetStageRoundListTest(uint stageLevelId, uint result)
        {
            var stageLevelList = Data.GetStageRoundList(stageLevelId)
                                .OrderBy(stageRound => stageRound.Round);
         
            Assert.That(() => stageLevelList.Count(), Is.EqualTo(result));
        }

        [Order(2)]
        [TestCase((uint)1, (uint)1)]
        public void GetStageRogueLikeDeckTest(uint roguelikeSaveDeclId, uint roguelikedeckId)
        {
            var rogueLikedeck = Data.GetRoguelikeSaveDeck(roguelikeSaveDeclId);

            Assert.That(() => rogueLikedeck.RoguelikeSaveDeckId, Is.EqualTo(roguelikedeckId));
        }

        [Order(3)]
        [TestCase((uint)10101, (uint)22)]
        public void SetRoguelikeRoundInfoTest(uint stageLevelId, uint userId)
        {
            var modeType = ModeType.PVE;


        }


    }
}
