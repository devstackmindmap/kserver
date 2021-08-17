using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using WebServer.Controller.Coupon;

namespace WebLogicTest
{
    public class WebGetCouponRewardTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task Run()
        {
            var web = new WebGetCouponReward();
            ProtoGetCouponReward res = await web.DoPipeline(new ProtoUserIdStringValue
            {
                UserId = 1,
                Value = "Z6E7HF3PDMJUXLE9"
            }) as ProtoGetCouponReward;
        }
    }
}

