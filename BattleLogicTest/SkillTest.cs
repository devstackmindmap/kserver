using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using NUnit.Framework;
using System.Threading.Tasks;


namespace BattleLogicTest
{
    [TestFixture]
    public class SkillTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void RoundTestByInt()
        {
            for(int i = 0; i < 10000; ++i)
            {
                float floatNum = 1.9f;
                int intNum = 20;

                var num = (int)(floatNum * intNum);
            }
        }

        [Test]
        public void RoundTestByRound()
        {
            for (int i = 0; i < 10000; ++i)
            {
                float floatNum = 1.9f;
                int intNum = 20;

                var num = (int)(floatNum * intNum);
            }
        }
    }
}
