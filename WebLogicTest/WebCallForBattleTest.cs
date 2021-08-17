using AkaEnum;
using NUnit.Framework;
using WebServer.Controller.Matching;
using CommonProtocol;
using WebServer.Controller.Battle;
using System.Threading.Tasks;
using AkaEnum.Battle;
using AkaUtility;
using System.Collections.Generic;

namespace WebLogicTest
{
    class WebCallForBattleTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void GetBattlerServerTest()
        {
            var web = new WebGetBattleServer();
            var result = web.DoPipeline(new ProtoGetBattleServer { GroupCode = 1});
            result.Wait();
            var onProto = result.Result as ProtoOnGetBattleServer;

            AkaLogger.Logger.Instance().Info(onProto.BattleServerIp + ":" + onProto.BattleServerPort);
            
        }

        [TestCase(BattleResultType.Win, BattleType.AkasicRecode_UserDeck, (uint)10101, (uint)22)]
      //  [TestCase(BattleResultType.Draw, 0, (uint)15)]
      //  [TestCase(BattleResultType.Lose, 0, (uint)15)]
        public async Task WebBattleResultTest(BattleResultType battleResultType, BattleType battleType, uint stageLevelId, uint userId)
        {
            var protoBattleResult = new ProtoBattleResultStage
            {
                PlayerInfoList = new List<ProtoBattleResultPlayerInfo>()
                    {
                        new ProtoBattleResultPlayerInfo
                        {
                            BattleResultType = battleResultType,
                            DeckNum = 0,
                            UserId = userId
                        }
                    },
                StageLevelId = stageLevelId,
                BattleType = battleType
            };

            var web = new WebGetBattleResult();
            ProtoOnBattleResult res = await web.DoPipeline(protoBattleResult) as ProtoOnBattleResult;

            
        }
    }
}
