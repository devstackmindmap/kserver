using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaConfig;
using AkaRedisLogic;
using CommonProtocol;
using NUnit.Framework;



namespace BattleLogicTest
{
    [TestFixture]
    [Description("Area = 1, UserId = 15 - Default case ")]
    public partial class StoryModeTest
    {
        private string sessionid = $"storymodetest-session:{KeyMaker.GetNewRoomId()}";

        private readonly int redisDbIndex = 10;

        private List<String> memidList = new List<string>();
        private const int defaultCaseEnemyid = 18;
        private const int defaultCaseMemid = 18;
        private const int defaultCaseArea = 1;

        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [OneTimeTearDown]
        [Ignore("--")]
        public void CleanUp()
        {
            var redis = AkaRedis.AkaRedis.Connector.GetDatabase(redisDbIndex);
        //    redis.KeyDelete(RedisKeyType.HRoomIdList.ToString());

        //    memidList.ForEach(memberid => redis.HashDelete(RedisKeyType.HRoomIdList.ToString(), memberid));

            AkaRedis.AkaRedis.Connector.Close();
            _asyncSessions.ForEach(session =>
            {
                if (session.IsConnected) session.Close();
            });

        }

        public void empty()
        {
            //TODO 지향점
            /*
             * Order 제거 
             * 구성 구현 -> 캡슐 -> 낮은 결합
             */

            //XXX 
            /*
             * RoomID 생성 주체
             * DoPipeline request 구성
             * BattleServer에서 룸생성
             * 동시접속
             * 파기정책
             */
        }

        

        [Order(1)]
        [Test, Description("UserId = 15 - Set Redis for Pve Room")]
        public void a01_CreateSetRedisPveRoomTest()
        {
            CreateSetRedisPveRoomTest(defaultCaseMemid);
        }

        [Order(2)]
        [Test, Description("UserId = 15 - Get Redis for Pve Room")]
        public void a02_GetRedisPveRoomTest()
        {
            GetRedisPveRoomTest(defaultCaseMemid);
        }

        [Order(3)]
        [Test, Description("Area = 1, UserId = 15 - BattleServer Create EnterRoom")]
        public async Task a03_MakeRoomTest()
        {
            var controller = BattleServer.ControllerFactory.CreateController(MessageType.EnterRoom);
            var dummyRoom = await CreateDummyEnterRoomMessage();
            var session = new BattleServer.NetworkSession();


            await controller.DoPipeline(session, dummyRoom);
        }


        [Order(4)]
        [Test, Description("Area = 1, UserId = 15 - BattleServer Create EnterRoom")]
        public async Task a04_EnemyRoomEnterTest()
        {
            var controller = BattleServer.ControllerFactory.CreateController(MessageType.EnterRoom);
            var dummyRoom = await CreateEnemyEnterRoomMessage();
            var session = new BattleServer.NetworkSession();
            
            await controller.DoPipeline(session, dummyRoom);
        }

        [Test, Description("BattleServer Create EnterRoom")]
        public void a00_CreateEnterRoomTest()
        {
            var controller = BattleServer.ControllerFactory.CreateController(MessageType.EnterRoom);
            Assert.That(controller.GetType().Name, Is.EqualTo("EnterRoom"));
        }

        [Test, Description("Connect PVE MatchingServer ")]
        public void a00_ConnectPVEMatchingServerTest()
        {
            //TODO get webserver
            string ip = "127.0.0.1";
            ConnectPVEServer("MatchingServer",ip, Config.MatchingServerConfig.MatchingServerList[1][1].port);
        }

        [Test, Description("Connect PVE BattleServer ")]
        public void a00_ConnectPVEBattleServerTest()
        {
            //TODO get webserver
            string ip = "127.0.0.1";
            ConnectPVEServer("BattleServer", ip,  Config.BattleServerConfig.BattleServerPort);
        }

        [Description("Enter Pve Room BattleServer ")]
        [TestCase(defaultCaseArea, defaultCaseMemid, defaultCaseEnemyid, 0,0 , true)]
        public void a00_EnterPveRoomTest(int area, int userid, int enemyid, byte deckNum, byte enemyDeckNum, bool expect)
        {
            //TODO get webserver
            string webServerIp = "127.0.0.1";
            string battleServerIp = "127.0.0.1";


            var userTask = EnterPveRoomOnBattleServer(area, userid,  deckNum, enemyid, enemyDeckNum, battleServerIp, webServerIp);
            
            var r1 = userTask.Wait(11000);
            Console.WriteLine("Battle Server 1");

            Assert.That(() => r1 , Is.EqualTo(expect));
        }


        [Description(" Check Matching Possiblity ")]
        [TestCase(defaultCaseMemid, defaultCaseEnemyid, true)]
        [TestCase(defaultCaseMemid, defaultCaseEnemyid + 1, false)]
        public void a00_CheckPossibleMatching(int userid, int enemyid, bool expect)
        {
            CheckPossibleMatching((uint)userid, (uint)enemyid , expect);
        }


        [Order(99)]
        [Test , Description("UserId = 15 - Destroy Pve Room")]
        [Ignore("Room 삭제점검")]
        public void a99_DeleteRedisPveRoomTest()
        {
            DeleteRedisPveRoomTest(defaultCaseMemid);
        }




    }
}
