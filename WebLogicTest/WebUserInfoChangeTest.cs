using AkaEnum;
using NUnit.Framework;
using CommonProtocol;
using System.Threading.Tasks;
using WebServer.Controller.User;
using System.Text;
using AkaDB.MySql;
using AkaRedisLogic;

namespace WebLogicTest
{
    class WebUserInfoChangeTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public async Task NicknameChangeTest()
        {
            var web = new WebUserAdditionalInfoChange();
            var result = await web.DoPipeline(new ProtoUserInfoChange
            {
                MessageType = MessageType.UserAdditionalInfoChange,
                UserId = 1,
                UserInfoType = UserAdditionalInfoType.NicknameChange,
                UserValue = "난닝구2"
            });
            var onProto = result as ProtoResult;
        }

        [TestCase((uint)1, "KR")]
        public async Task CountryChangeTest(uint userId, string newCountryCode)
        {
            var web = new WebUserAdditionalInfoChange();
            var result = await web.DoPipeline(new ProtoUserInfoChange
            {
                MessageType = MessageType.UserAdditionalInfoChange,
                UserId = userId,
                UserInfoType = UserAdditionalInfoType.CountryCodeChange,
                UserValue = newCountryCode
            });
            var onProto = result as ProtoResult;

            Assert.True(onProto.ResultType == ResultType.Success);
        }

        [Test]
        public async Task AddDeckTest()
        {
            var web = new WebUserAdditionalInfoChange();
            var result = await web.DoPipeline(new ProtoUserInfoChange
            {
                MessageType = MessageType.UserAdditionalInfoChange,
                UserId = 7,
                UserInfoType = UserAdditionalInfoType.AddDeck
            });
            var onProto = result as ProtoResult;

            Assert.True(onProto.ResultType == ResultType.Success);
        }
    }
}
