using NUnit.Framework;
using System.Diagnostics;
using WebServer;
using AkaEnum;
using CommonProtocol;
using AkaUtility;
using TestHelper;
using System.Threading.Tasks;
using System.Text;
using AkaDB.MySql;

namespace WebLogicTest
{
    public class LoginTester
    {
        private string _nickName;
        private RunMode _runMode;
        public ProtoOnLogin ProtoOnLogin { get; private set; }

        public LoginTester()
        {
            _nickName = "1_20190603";
            string runMode = TestContext.Parameters["runMode"];
            _runMode = runMode.CastToEnum<RunMode>();
        }

        public LoginTester(string nickName, RunMode runMode)
        {
            _nickName = nickName;
            _runMode = runMode;
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        public void Login()
        {
            Login(_nickName, _nickName);
        }

        [TestCase("k1", "k1")]
        [TestCase("k6", "k6")]
        [TestCase("k7", "k7")]
        [TestCase("k8", "k8")]
        [TestCase("k9", "k9")]
        [TestCase("k10", "k10")]
        [TestCase("EnergyTest", "EnergyTest")]
        [TestCase("±èµ¿¿µ", "123213a")]
        [TestCase("±èµ¿¿µ2", "123213adf")]
        [TestCase("±èµ¿¿µ5", "±èµ¿¿µ5")]
        public async Task Login(string socialAccount, string nickName)
        {
            var protoLogin = new ProtoLogin
            {
                MessageType = MessageType.Login,
                SocialAccount = socialAccount,
                LanguageType = "KR",
                PlatformType = PlatformType.Google
            };

            var webLogin = new WebLogin(null);
            var resultTask = webLogin.DoPipeline(protoLogin);
            resultTask.Wait();

            ProtoOnLogin = resultTask.Result as ProtoOnLogin;

            if (ProtoOnLogin.ResultType == ResultType.InvalidParameter)
                Assert.Fail("Invalid Parameter");

            if (ProtoOnLogin.ResultType == ResultType.NotJoined)
            {
                var protoAccountJoin = new ProtoAccountJoin
                {
                    MessageType = MessageType.AccountJoin,
                    NickName = nickName,
                    SocialAccount = socialAccount,
                    LanguageType = "KR",
                    PlatformType = PlatformType.Google,
                    NightPushAgree = 1,
                    PushAgree = 1,
                    TermsAgree = 1,
                    PushKey = "fdsa"
                };

                var webAccountJoin = new WebAccountJoin(null);
                resultTask = webAccountJoin.DoPipeline(protoAccountJoin);
                resultTask.Wait();
                ProtoOnLogin = resultTask.Result as ProtoOnLogin;

                if (ProtoOnLogin.ResultType == ResultType.NicknameDuplicate)
                    Debug.WriteLine("NicknameDuplicate");
            }

            Debug.WriteLine("[[ protoOnLogin ]]");
            Debug.WriteLine("userId : " + ProtoOnLogin.UserId);

            Debug.WriteLine("*** materailInfoList ***");
            foreach (var materialInfo in ProtoOnLogin.UserInfo.MaterialInfoList)
            {
                Debug.WriteLine("materialType: " + materialInfo.MaterialType);
                Debug.WriteLine("count: " + materialInfo.Count);
            }

            Debug.WriteLine("*** cardInfoList ***");
            foreach (var cardInfo in ProtoOnLogin.CardInfoList)
            {
                Debug.WriteLine("cardId: " + cardInfo.Id);
                Debug.WriteLine("level: " + cardInfo.Level);
            }

            //await DefaultDeckSet(ProtoOnLogin.UserId);
        }

        [TestCase((uint)46)]
        public async Task DefaultDeckSet(uint userId)
        {
            var query = new StringBuilder();
            query.Append($"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES ({userId}, 2, 1, 0, 0, 1001);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 1, 0, 1, 1002);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 1, 0, 2, 1003);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 0, 102);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 1, 103);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 2, 103);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 3, 202);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 4, 203);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 5, 203);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 6, 302);" +
                $"INSERT INTO `decks` (`userId`, `modeType`, `slotType`, `deckNum`, `orderNum`, `classId`) VALUES({userId}, 2, 2, 0, 7, 303); ");
            using (var db = new DBContext(userId))
            {
                await db.ExecuteNonQueryAsync(query.ToString());
            }
        }

        [TestCase(100)]
        public void MultiLogin(int count)
        {
            MultiUserController multiUserController = new MultiUserController();
            multiUserController.MakeUserList(count);
        }
    }
}