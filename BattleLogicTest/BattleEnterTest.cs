using BattleServer.Controller.Controllers;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;

namespace BattleLogicTest
{
    [TestFixture]
    public partial class BattleEnterTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task BattleEnterTest01()
        {
            var battleEnter = new EnterPvERoom();
            await battleEnter.DoPipeline(new BattleServer.NetworkSession
            {
                UserId = 1
            }, new ProtoEnterPveRoom
            {
                BattleType = AkaEnum.Battle.BattleType.AkasicRecode_RogueLike,
                DeckNum = 0,
                UserId = 1,
                StageLevelId = 80101,
                StageRoundId = 0
            });
        }
    }
}
