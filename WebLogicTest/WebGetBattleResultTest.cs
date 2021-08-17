using AkaConfig;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using Common.Entities.Challenge;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Battle;

namespace WebLogicTest
{
    public class WebGetBattleResultTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [TestCase((uint)1, (uint)4, 40, BattleType.LeagueBattle)]
        [TestCase((uint)2, (uint)3, 40, BattleType.LeagueBattle)]
        [TestCase((uint)1, (uint)2, 1, BattleType.LeagueBattle)]
        [TestCase((uint)46, (uint)1, 1, BattleType.LeagueBattle)]
        [TestCase((uint)1, (uint)46, 1, BattleType.LeagueBattle)]
        [TestCase((uint)1, (uint)7, 1, BattleType.LeagueBattle)]
        [TestCase((uint)7, (uint)1, 1, BattleType.LeagueBattleAi)]
        public async Task RankResultTest(uint winUserId, uint loseUserId, int GameRepeat, BattleType battleType)
        {
            Network.PubSubConnector.Instance
                .Connect(
                Config.GameServerConfig.WebPubServer.ip,
                Config.GameServerConfig.WebPubServer.port,
                Config.GameServerConfig.WebPubServer.tryReconnectTime);

            var battleResultPlayerInfoList = new List<ProtoBattleResultPlayerInfo>();
            battleResultPlayerInfoList.Add(new ProtoBattleResultPlayerInfo
            {
                BattleResultType = BattleResultType.Win,
                DeckNum = 0,
                UserId = winUserId,
                ActionStatusLog = new List<ProtoActionStatus>()
            }) ;
            battleResultPlayerInfoList.Add(new ProtoBattleResultPlayerInfo
            {
                BattleResultType = BattleResultType.Lose,
                DeckNum = 0,
                UserId = loseUserId,
                ActionStatusLog = new List<ProtoActionStatus>()
            });
            var web = new WebGetBattleResultRank();

            for (int i = 0; i < GameRepeat; i++)
            {
                var res = await web.DoPipeline(new ProtoBattleResult
                {
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                    MessageType = MessageType.GetBattleResultKnightLeague,
                    PlayerInfoList = battleResultPlayerInfoList
                });
            }
        }

        [Test]
        public async Task TodayKnightLeagueWinCount()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(7))
                {
                    var manager = ChallengeFactory.CreateChallengeManager(accountDb, userDb, 7, 1, 1, 1);
                    var res = await manager.GetTodayKnightLeagueWinCount();
                }
            }
        }

        [TestCase((uint)9, (uint)0, 1)]
        public async Task AiRankResultTest(uint winUserId, uint loseUserId, int GameRepeat)
        {
            Network.PubSubConnector.Instance
                .Connect(
                Config.GameServerConfig.WebPubServer.ip,
                Config.GameServerConfig.WebPubServer.port,
                Config.GameServerConfig.WebPubServer.tryReconnectTime);

            var battleResultPlayerInfoList = new List<ProtoBattleResultPlayerInfo>();
            battleResultPlayerInfoList.Add(new ProtoBattleResultPlayerInfo
            {
                BattleResultType = BattleResultType.Win,
                DeckNum = 0,
                UserId = winUserId
            });
            var web = new WebGetBattleResultRank();

            for (int i = 0; i < GameRepeat; i++)
            {
                var res = await web.DoPipeline(new ProtoBattleResult
                {
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattleAi,
                    MessageType = MessageType.GetBattleResultKnightLeague,
                    PlayerInfoList = battleResultPlayerInfoList
                });
            }
        }

        [Test]
        public void SendBattleResultToClientTest()
        {
            
        }

        [Test]
        public void GetUserCurrentRankLevelTest()
        {
            var level = Data.GetUserRankLevelByPoint(2400);
        }


        [Test]
        public void GetUserCurrentRankLevelTest2()
        {
            Data.GetUserRankLevelIdFromPoint(2000, out int nextPoint, out int minPoint);
        }
    }
}
