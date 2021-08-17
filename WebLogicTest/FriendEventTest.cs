using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServer.Controller.Event;
using WebServer.Controller.Friend;

namespace WebLogicTest
{
    public class FriendEventTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task WebEventAddFriendByCodeTest()
        {
            var web = new WebEventAddFriendByCode();
            var res = await web.DoPipeline(new ProtoUserIdTargetId {
                UserId = 1,
                TargetId = 2
            });
        }

        [Test]
        public async Task WebEventFriendGetRewardTest()
        {
            var web = new WebEventFriendGetReward();
            var res = await web.DoPipeline(new ProtoUserIdTargetId
            {
                UserId = 1,
                TargetId = 1
            });
        }

        [Test]
        public async Task WebEventFriendCheckTest()
        {
            var web = new WebEventFriendCheck();
            var res = await web.DoPipeline(new ProtoUserId { UserId = 1 });
        }
    }
}
