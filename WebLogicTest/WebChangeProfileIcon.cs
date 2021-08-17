using AkaDB.MySql;
using Common.Entities.User;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using WebServer.Controller.User;

namespace WebLogicTest
{
    public class WebChangeProfileIcon
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task Run()
        {
            var web = new WebServer.Controller.User.WebChangeProfileIcon();
            ProtoUserId res = await web.DoPipeline(new ProtoChangeProfileIcon { 
                UserId = 129,
                ProfileIconId = 2,
                ProfileIconType = AkaEnum.ProfileIconType.User
            }) as ProtoUserId;
        }
    }
}
