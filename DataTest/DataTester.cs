using AkaData;
using AkaEnum;
using NUnit.Framework;

namespace DataTest
{
    [TestFixture]
    class DataTester
    {
        private RunMode _runMode = RunMode.Machance;

        [SetUp]
        public void Init()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void DataAnimationLengthMapTest()
        {
            var length = Data.GetAnimationLengthBullet("A10", AnimationType.Attack);
            var takeDamage = Data.GetAnimationTakeDamageLength("A10", AnimationType.Attack);

            Assert.NotZero(length);
            Assert.NotZero(takeDamage);
        }

        [Test]
        public void DataUserProduct()
        {
            var data = Data.GetDataUserProductDigital(90000);
            var data2 = Data.GetDataUserProductReal(90001);
        }
    }
}
