using AkaDB.MySql;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using WebLogic.Friend;
using WebServer.Controller.Friend;

namespace WebLogicTest
{
    public class FriendTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task RequestFriendByIdTest()
        {
            var web = new WebRequestFriendById();
            var res = await web.DoPipeline(new ProtoRequestFriend {
                UserId = 4,
                FriendId = 3,
                MessageType = MessageType.RequestFriendById
            });
        }

        [Test]
        public async Task RequestFriendByNicknameTest()
        {
            var web = new WebRequestFriendByNickname();
            var res = await web.DoPipeline(new ProtoAddInvite
            {
                UserId = 4,
                InviteCode = "kdy1",
                MessageType = MessageType.RequestFriendByNickname
            });
        }

        [Test]
        public async Task AddFriendByRequestedTest()
        {
            var web = new WebAddFriendByRequested();
            var res = await web.DoPipeline(new ProtoUserIdTargetId
            {
                UserId = 3,
                TargetId = 4,
                MessageType = MessageType.AddFriendByRequested
            });
        }

        [Test]
        public async Task AddFriendTest()
        {
            var friend = new FriendManager();
            await friend.AddFriend(1, 2);
            await friend.AddFriend(2, 1);
        }

        [Test]
        public async Task RemoveFriendTest()
        {
            using (var userDb = new DBContext(1))
            {
                var friend = new FriendManager();
                await friend.RemoveFriendJobs(1, 2, userDb);
            }

            using (var userDb = new DBContext(2))
            {
                var friend = new FriendManager();
                await friend.RemoveFriendJobs(2, 1, userDb);
            }
        }
    }
}
