using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AkaConfig;
using AkaEnum;
using AkaRedisLogic;
using BattleServer;
using NUnit.Framework;
using SuperSocket.ClientEngine;

namespace BattleLogicTest
{
    [TestFixture]
    public partial class BattleRoomTest
    {
        private string sessionid = $"storymodetest-session:{KeyMaker.GetNewRoomId()}";

        private readonly int redisDbIndex = 10;

        private List<String> memidList = new List<string>();
        private const int defaultCaseEnemyid = 18;
        private const int defaultCaseMemid = 18;
        private const int defaultCaseArea = 1;
        private List<AsyncTcpSession> _asyncSessions = new List<AsyncTcpSession>();

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
            _asyncSessions.ForEach(session =>
            {
                if (session.IsConnected) session.Close();
            });

        }

        public void empty()
        {
        }

        private BattleInfo CreatePlayerInfo(uint userId, byte deckNum,int areaNum, uint stageRoundId, AkaEnum.ModeType modeType)
        {
            var battleServerIp = "127.0.0.1";
            var webServerUri = $"http://127.0.0.1:{Config.GameServerConfig.GameServerPort}/";
            return new BattleInfo
            {
                BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                UserId = userId,
                DeckNum = deckNum,
      //          StageRoundHelperId = stageRoundId,
                BattleServerIp = battleServerIp,
                SessionId = sessionid
            };
        }


        [TestCase((uint)5010101,12)]
        public void GetStageRoundBehavior(uint stageRoundId, int expect)
        {
            var deckCreator = new BattleLogic.StageDeckCreator(stageRoundId, AkaEnum.Battle.PlayerType.Player1, AkaEnum.Battle.PlayerType.Player2);
            var behavior = deckCreator.GetPatternContainer();
            Assert.That(behavior?.PatternsOfMonster?.Count, Is.EqualTo(expect));

        }

        [TestCase((uint)5010101, false)]
        public void HasPatternTimeAction(uint stageRoundId,bool expect)
        {
            var deckCreator = new BattleLogic.StageDeckCreator(stageRoundId, AkaEnum.Battle.PlayerType.Player1, AkaEnum.Battle.PlayerType.Player2);
            var behavior = deckCreator.GetPatternContainer();
            Assert.That(behavior?.HasTimeAction, Is.EqualTo(expect));
        }

        [TestCase((uint)5010101, (uint)50101, ActionPatternType.PerMyselfSkillUsingCount,6)]
        public void GetStageRoundPattern(uint stageRoundId,uint unitId, ActionPatternType expectActionType, int expectPatternValue)
        {
            var deckCreator = new BattleLogic.StageDeckCreator(stageRoundId, AkaEnum.Battle.PlayerType.Player1, AkaEnum.Battle.PlayerType.Player2);
            var behavior = deckCreator.GetPatternContainer();
            var actionType = behavior?.PatternsOfMonster?[0]?.Conditions[0].ConditionData.ActionPatternType;
            var actionValue = behavior?.PatternsOfMonster?[0]?.Conditions[0].ConditionData.ActionPatternValue;

            Assert.That( (actionType, actionValue) , Is.EqualTo( (expectActionType,expectPatternValue) ) );

        }

        [TestCase((uint)18, 0)]
        public void GetPvPDeckInfoFromWeb(uint userId, byte deckNum)
        {
            var playerInfo = CreatePlayerInfo(userId, deckNum, 1, 0, AkaEnum.ModeType.PVP);
            var guestPlayerInfo = CreatePlayerInfo(userId, deckNum, 1, 0, AkaEnum.ModeType.PVP);

            var roomId = $"BattleRoomTest-{userId.ToString()}";
            if (RoomManager.MakeRoom(roomId, playerInfo)
                && RoomManager.TryEnterGuestPlayer(roomId, guestPlayerInfo))
            {
                RoomManager.BattleInitialize(roomId);
                return;
            }

            Assert.Fail();            
        }

        [TestCase((uint)18, 0)]
        public void GetPvEDeckInfoFromWeb(uint userId, byte deckNum)
        {
            var playerInfo = CreatePlayerInfo(userId, deckNum, 1, 0, AkaEnum.ModeType.PVP);

            var roomId = $"BattleRoomTest2-{userId.ToString()}";
            if (RoomManager.MakeRoom(roomId, playerInfo)
                && RoomManager.TryEnterGuestPlayer(roomId, playerInfo))
            {
                RoomManager.BattleInitialize(roomId);
                return;
            }

            Assert.Fail();
        }

    }
}
