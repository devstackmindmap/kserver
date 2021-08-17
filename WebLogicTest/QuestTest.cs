using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common;
using CommonProtocol;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class QuestTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [TestCase((uint)6390, 1)]
        [TestCase((uint)6390, 2)]
        [TestCase((uint)6390, 3)]
        public async Task CreateClanTest(uint userId, int season)
        {
            var webServerUri = $"http://127.0.0.1:{AkaConfig.Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.SkipQuest, AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(new ProtoUserId
            {
                MessageType = MessageType.SkipQuest,
                UserId = userId
            }), webServerUri);

            var protoResult = AkaSerializer.AkaSerializer<ProtoOnSkipCurrentQuest>.Deserialize(result);

            var strResult = $"Season:{season} - {protoResult.ResultType.ToString()} \n{ string.Join(",", protoResult.RemainedMaterial.Select(material => "Material:" + material.Key.ToString() + "   Value:" + material.Value))}"
                        +$"   { string.Join(",", protoResult.QuestInfoList.Select(questData => $"{questData.QuestGroupId}:{questData.PerformCount} - {questData.CompleteOrder}") )}";
            Console.WriteLine(strResult);
        }

        [TestCase(2020, 6, 12, 23, 0, 0, 2020, 6, 11, 11, 0, 0)]
        [TestCase(2020, 6, 12, 23, 0, 0, 2020, 6, 12, 10, 0, 0)]
        [TestCase(2020, 6, 12, 23, 0, 0, 2020, 6, 12, 12, 0, 0)]
        [TestCase(2020, 6, 13, 1, 0, 0, 2020, 6, 11, 11, 0, 0)]
        [TestCase(2020, 6, 13, 1, 0, 0, 2020, 6, 12, 10, 0, 0)]
        [TestCase(2020, 6, 13, 1, 0, 0, 2020, 6, 12, 12, 0, 0)]
        [TestCase(2020, 6, 13, 11, 0, 0, 2020, 6, 11, 11, 0, 0)]
        [TestCase(2020, 6, 13, 11, 0, 0, 2020, 6, 12, 10, 0, 0)]
        [TestCase(2020, 6, 13, 11, 0, 0, 2020, 6, 12, 12, 0, 0)]
        public void Check(int year, int month, int day, int hour, int minute, int second,
            int rYear, int rMonth, int rDay, int rHour, int rMinute, int rSecond)
        {
            //var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var refreshBaseHour = 11;
            var refreshDateTime = new DateTime(rYear, rMonth, rDay, rHour, rMinute, rSecond);
            var utcnow = new DateTime(year, month, day, hour, minute, second).AddHours(-refreshBaseHour);
            var baseDate = new DateTime(utcnow.Year, utcnow.Month, utcnow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);
            var lastRefreshDate = refreshDateTime;
            if (lastRefreshDate < baseDate)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }
    }
}
