using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class WebGetProfileTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task UserProfile()
        {
            var web = new WebServer.Controller.User.WebGetUserProfile();
            ProtoUserProfile res = await web.DoPipeline(new ProtoUserId { UserId = 129 }) as ProtoUserProfile;
        }

        [Test]
        public async Task UnitProfile()
        {
            var web = new WebServer.Controller.User.WebGetUnitProfile();
            ProtoOnUnitProfile res = await web.DoPipeline(new ProtoUserId { 
                MessageType = MessageType.GetUnitProfile, 
                UserId = 129 }) as ProtoOnUnitProfile;
        }

        [Test]
        public async Task CardProfile()
        {
            var web = new WebServer.Controller.User.WebGetCardProfile();
            ProtoOnCardProfile res = await web.DoPipeline(new ProtoUserIdAndId
            {
                MessageType = MessageType.GetCardProfile,
                UserId = 129,
                Id = 1001
            }) as ProtoOnCardProfile;
        }
    }
}
