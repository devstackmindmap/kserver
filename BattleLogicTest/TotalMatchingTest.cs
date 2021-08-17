using AkaConfig;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaLogger;
using BattleLogic;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelper;
using TestHelper.BattleData;
using TestHelper.Matching;
using AkaUtility;
using BattleServer;
using BattleServer.Controller.Controllers;
using BattleServer.Controller;

namespace BattleLogicTest
{
    class TotalMatchingTest
    {
        [SetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        public void ShowProcessTime(string section)
        {
            //var process = System.Diagnostics.Process.GetCurrentProcess();
            Console.WriteLine($"{section} Total Elapsed Time(sec) { (DateTime.UtcNow - _elapsedCounter).TotalSeconds.ToString()}");
            _elapsedCounter = DateTime.UtcNow;
        }

        DateTime _elapsedCounter = DateTime.UtcNow;


        [TestCase(1, "172.30.1.224", "172.30.1.224", 1, TestName = "Dylan2 1", Category = Category.Overhead)]
   /*     [TestCase(100, "47.75.127.95", "47.75.102.249", 0, TestName = "Dev2 500", Category = Category.Overhead)]
        [TestCase(1000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 1000")]
        [TestCase(2000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 2000")]
        [TestCase(2000, "172.30.1.222", "172.30.1.222", 20, TestName = "Dylan2 2000 Repeat 20")]
        [TestCase(3000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 3000")]
        [TestCase(4000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 4000")]
        [TestCase(5000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 5000")]
        [TestCase(5000, "172.30.1.222", "172.30.1.222", 20, TestName = "Dylan2 5000 Repeat 20")]
        [TestCase(6000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 6000")]
        [TestCase(7000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 7000")]
        [TestCase(8000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 8000")]
        [TestCase(9000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 9000")]
        [TestCase(10000, "172.30.1.222", "172.30.1.222", 1, TestName = "Dylan2 10000")]
        [TestCase(10000, "172.30.1.222", "172.30.1.222", 3, TestName = "Dylan2 10000 Repeat 3")]
        */
        public async Task MatchingOverHeadTest(int userCount, string ip, string webip, int repeat)
        {
            int port = 40554;

            var webServerUri = $"http://{webip}:{Config.MatchingServerConfig.GameServer.port}/";

            for (int i = 0; i < repeat; i++)
            {
                await Matching(userCount, ip, webip, webServerUri, port);
                if (i + 1 == repeat)
                    break;
            }

            //Assert.That(() => userControllers.Count(), Is.EqualTo(250));
        }

        private async Task Matching(int userCount, string ip, string webip, string webServerUri, int port)
        {
            using (MatchingTestHelper helper = new MatchingTestHelper(ip, port))
            {

                ShowProcessTime("Start");
                helper.MakeUserAndConnection(userCount);
                ShowProcessTime("Make User");
                await helper.TryMatching(webServerUri);

                var results = helper.WaitForSendResult();
                ShowProcessTime("TryMatching");

                var res = results.All(result => result.receivedMessage == MessageType.MatchingSuccess);
                Assert.That(res, "Not Matched result in connection");
                var userMatched = results.Count(result => result.RoomId != null);

                Console.WriteLine($"All user:{helper.Count} Matched:{results.Count()} WithUser:{userMatched} WithAi:{results.Count() - userMatched}");
                // Assert.That(userMatched == helper.Count >> 1, "Not Matched result in connection");

                return;
            }
        }

        [TestCase(BattleType.LeagueBattleAi, 22, 5010101, 0, true, TestName = "나이트리크 정상", Category = Category.Validation)]
        [TestCase(BattleType.LeagueBattleAi, 22, 5011101, 0, false, TestName = "나이트리크 랭크 비정상", Category = Category.Validation)]
        [TestCase(BattleType.LeagueBattleAi, 22, 1010101, 0, false, TestName = "나이트리크 라운드ID 비정상", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 5011101, 0, false, TestName = "아카식 로그라이크 파라미터 비정상", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 0, 1010101, false, TestName = "아카식 로그라이크 레벨 비정상", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 0, 10101, true, TestName = "아카식 로그라이크 정상", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 0, 10103, false, TestName = "아카식 로그라이크 잠금레벨 접근", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_UserDeck, 22, 0, 50101, true, TestName = "아카식 유저덱 정상", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_UserDeck, 22, 0, 50103, false, TestName = "아카식 유저덱 잠금레벨 접근", Category = Category.Validation)]
        [TestCase(BattleType.AkasicRecode_UserDeck, 22, 0, 10101, true, TestName = "아카식 레벨 접근 (none db)", Category = Category.Validation)]
        public void ValidateEnterPvERoom(BattleType battleType, int userId, int stageRoundId, int stageLevelId, bool assertResult)
        {

            var playerInfo = new BattleInfo
            {
                BattleType = battleType,
                UserId = (uint)userId,
                DeckNum = 0,
                StageRoundId = (uint)stageRoundId,
                StageLevelId = (uint)stageLevelId,
            };

            var tempEnterPveRoom = new TestEnterPveRoom();
            var result = tempEnterPveRoom.ValidEnterRoom(playerInfo);

            Assert.That(result, Is.EqualTo(assertResult));
        }

        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 10101, 0, true, TestName = "로그라이크 첫진행 stage Id", Category = Category.Validation_Roguelike)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 10102, 0, false, TestName = "로그라이크 첫진행(불가능) stage Id", Category = Category.Validation_Roguelike)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 0, 0, false, TestName = "진행중 로그라이크 가져오기 (없음)", Category = Category.Validation_Roguelike)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 0, 10101, true, TestName = "진행중 로그라이크 가져오기 (있음)", Category = Category.Validation_Roguelike)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 10101, 10101, true, TestName = "진행중 로그라이크 일치 (있음)", Category = Category.Validation_Roguelike)]
        [TestCase(BattleType.AkasicRecode_RogueLike, 22, 50101, 10101, false, TestName = "진행중 로그라이크 불일치 (있음)", Category = Category.Validation_Roguelike)]
        public async Task ValidateEnterRoguelike(BattleType battleType, int userId, int stageLevelId, int setDbLevelid, bool assertResult)
        {
            using (BattleRoomTestHelper helper = new BattleRoomTestHelper())
            {
                await helper.SetRoguelikeStageRound((uint)userId, (uint)setDbLevelid, 0);

                var playerInfo = new BattleInfo
                {
                    BattleType = battleType,
                    UserId = (uint)userId,
                    DeckNum = 0,
                    StageRoundId = (uint)0,
                    StageLevelId = (uint)stageLevelId,
                };

                var tempEnterPveRoom = new TestEnterPveRoom();
                var result = tempEnterPveRoom.ValidEnterRoom(playerInfo);
                Assert.That(result, Is.EqualTo(assertResult));
            }
        }

        [TestCase(TestName = "DB - 로그인시 진행중 로그라이크 스테이지 정보", Category = Category.Login_Info)]
        public async Task DB_GetInprogressStageList()
        {
            using (BattleRoomTestHelper helper = new BattleRoomTestHelper())
            {
                uint userId = 99999;
                (uint stageLevelId, uint clearRound, BattleType battleType, List<uint> cardStatIdList, List<uint> replaceCardStatIdList)[] cases =
                {
                    ( stageLevelId: 88888 , clearRound: 0, battleType:BattleType.AkasicRecode_RogueLike, cardStatIdList: new List<uint>(), replaceCardStatIdList: new List<uint>() ),
                    ( stageLevelId: 77777 , clearRound: 2, battleType:BattleType.AkasicRecode_UserDeck, cardStatIdList: new List<uint>(), replaceCardStatIdList: new List<uint>() )
                };

                foreach (var tuple in cases)
                {
                    await helper.InsertRoguelikeStageRound(userId, tuple.stageLevelId, tuple.clearRound, tuple.battleType, tuple.cardStatIdList, tuple.replaceCardStatIdList);
                }


                var stageInfo = new Common.Entities.Stage.StageInfo(helper.DB, userId);
                var inprogressList = await stageInfo.GetInProgressStageList();

                Assert.That(() =>
                    cases.All((tuple) => inprogressList.SafeGet(tuple.battleType)?.StageLevelId == tuple.stageLevelId)
                , Is.EqualTo(true));
            }

        }


        [TestCase(TestName = "Join, All 샘플", Category = Category.Dotnet)]
        public void TestJoin()
        {
            var nullList1 = new List<int>();
            var nullList2 = new List<int>();
            Console.WriteLine(nullList1.All(nullList2.Contains));
            Console.WriteLine(nullList1.Any());

            var sample1 = new List<int>() { 1};
            var excepted1 = sample1.Except(nullList1);
            var excepted2 = sample1.Except(excepted1);
            


            Console.WriteLine(nullList1.All(excepted1.Contains));

            Console.WriteLine(true && true || false);
            return;
            var str = string.Join("/", new List<int>());
            Console.WriteLine(str.Length);

            var take = new List<int>() { 1, 2, 3, 4, 5 };
            var treasureIdList = take.Take(6);
            var cont = new List<int>() { 6 };


            Console.WriteLine(cont.All(take.Contains));
            cont = null;
            Console.WriteLine(false == cont?.Any());


            var take2 = new List<int>() { 2, 3, 7 };
            var takeList = new List<List<int>>() { new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 6 } };
            var find = new List<int> { 1, 2, 7 };

            var cardStatIdList = takeList.Take(2).SelectMany(proposalCardList => proposalCardList)
                                                                   .Union(take2);

            Console.WriteLine(find.All(cardStatIdList.Contains));
            List<int> emptyList = new List<int>();
            Console.WriteLine(find.All(emptyList.Contains));

            Console.WriteLine(emptyList.All(cardStatIdList.Contains));

        }


        [TestCase(TestName = "타입변환 샘플", Category = Category.Dotnet)]
        public void TypeConvertTest()
        {
            double dd = 3.1;
            int a = Convert.ToInt32(dd);
            Console.WriteLine(a == dd);
            var convertedVar = Convert.ChangeType("0", typeof(System.UInt32));

            int.Parse("ee", System.Globalization.NumberStyles.AllowHexSpecifier);
            //System.Drawing.Color()
        }

        class te : IDisposable
        {
            public void Dispose()
            {
                TestContext.WriteLine("Dispose");
            }
        }

        void TaskInfoOut(string message)
        {
            TestContext.WriteLine($"{message} :  TaskId -{Task.CurrentId}  ThreadId - {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        [Test]
        public async Task TaskAndThreadTest()
        {
            using (te t = new te())
            {
                TaskInfoOut("0");
                await Task.Delay(1000);
                TaskInfoOut("1");
                await Task.Delay(1000);
                TaskInfoOut("2");
                await Task.Yield();
                TaskInfoOut("3");
                await Task.Run(() => { });
                TaskInfoOut("4");
                await Task.Delay(1000);
                TaskInfoOut("5");
                await Task.Factory.StartNew(() => { TaskInfoOut("5 -1"); });
                TaskInfoOut("6");

            }
            TaskInfoOut("7");
        }

        public class TestEnterPveRoom : EnterPvERoom
        {
            public bool ValidEnterRoom(BattleInfo playerInfo)
            {
                var task = PveInfoFactory.SetBattleInfo(playerInfo);
                task.Wait();
                return task.Result == ResultType.Success ? true : false;
            }
        }
    }
}
