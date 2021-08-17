using NUnit.Framework;

namespace BattleLogicTest
{
    class TutorialTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void NormalBattleCreateTest()
        {

        }
    }
}
