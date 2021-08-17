using AkaConfig;
using AkaEnum;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Battle;
using WebServer.Controller.Rank;

namespace WebLogicTest
{
    public class WebGetRankingBoardTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task RankResultTest()
        {
            var web1 = new WebGetRankingBoard();
            var res1 = await web1.DoPipeline(new ProtoRankingBoard
            {
                MessageType = MessageType.GetRankingBoard,
                UserId = 126
            });

            var web2 = new WebGetRankingBoardClan();
            var res2 = await web2.DoPipeline(new ProtoRankingBoard
            {
                MessageType = MessageType.GetRankingBoard,
                UserId = 126
            });

            var web3 = new WebGetRankingBoardUnit();
            var res3 = await web3.DoPipeline(new ProtoRankingBoardUnit
            {
                MessageType = MessageType.GetRankingBoard,
                UserId = 126,
                UnitId = 1001
            });

        }
    }
}
