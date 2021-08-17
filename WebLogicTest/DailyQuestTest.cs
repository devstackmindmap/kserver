using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using Common.Entities.Reward;
using Common.Quest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLogicTest
{
    public class DailyQuestTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        [Test]
        public void GetSpendData()
        {
            var datas = Data.GetSpendMaterials(BehaviorType.DailyQuestForcedAdd);
            System.Console.WriteLine(string.Join(",", datas.Select(data => data.Order.ToString() + " - " + data.Value)));
        }

        [TestCase(6390u, 90001u, 90001u, Description = "Added Quest")]
        [TestCase(6390u, 90001u, 90001u, Description = "FullQuest : result empty")]
        public async Task DynamicQuestRewardTest(uint userid, uint rewardId, uint itemValue)
        {
            using (DBContext db = new DBContext(7))
            {
                var rewards = await Reward.GetRewards(db, userid, rewardId, "Test", itemValue);

                System.Console.WriteLine(string.Join(",", rewards.Select(result => result.ItemType.ToString() + " - " + result.ClassId)));
            }
        }

        class InitDbData
        {
            public int refreshCount;
            public int addCount;
            public string lastRefreshDate;
            public uint quest90001;
            public uint quest90002;
        }

        /*
         * 0 empty 날짜 한참전
         * 1 꽉참 날짜 한참전
         */
        static InitDbData[] caseDatas = new InitDbData[]
        {
            new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-10).ToTimeString(), quest90001 =0, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-10).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-10).ToTimeString(), quest90001 =91001, quest90002 = 91002}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =0, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =91001, quest90002 = 91002}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-2).ToTimeString(), quest90001 =0, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-1).ToTimeString(), quest90001 =0, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-2).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-1).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-2).ToTimeString(), quest90001 =91001, quest90002 = 91002}
            ,new InitDbData{ refreshCount = 10, addCount = 20, lastRefreshDate = DateTime.UtcNow.AddDays(-1).ToTimeString(), quest90001 =91001, quest90002 = 91002}

            ,new InitDbData{ refreshCount = 0, addCount = 0, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =0, quest90002 = 0}
            ,new InitDbData{ refreshCount = 0, addCount = 1, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 0, addCount = 0, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =91001, quest90002 = 91002}
            ,new InitDbData{ refreshCount = 1, addCount = 0, lastRefreshDate = DateTime.UtcNow.AddDays(0).ToTimeString(), quest90001 =91001, quest90002 = 0}
            ,new InitDbData{ refreshCount = 0, addCount = 1, lastRefreshDate = (new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2, 1, 0, DateTimeKind.Utc).AddHours(31)) .ToTimeString(), quest90001 =91001, quest90002 = 0}
        ,};


        //   [TestCase(6390u, 90001u, Description = "Added Quest")]
        [TestCase("9days ago need pay 2 hole", 6390u, 0, 0u)] //full success
        [TestCase("9days ago need pay 1 hole", 6390u, 1, 0u)] // 
        [TestCase("9days ago need pay 0 hole", 6390u, 2, 0u)] //already 
        [TestCase("0day need pay 2 hole", 6390u, 3, 0u)] //already 
        [TestCase("0day need pay 1 hole", 6390u, 4, 0u)] //already 
        [TestCase("0day need pay 0 hole", 6390u, 5, 0u)] //already 
        [TestCase("2days ago need pay 2 hole", 6390u, 6, 0u)] //already 
        [TestCase("1days ago need pay 2 hole", 6390u, 7, 0u)] //already 
        [TestCase("2days ago need pay 1 hole", 6390u, 8, 0u)] //already 
        [TestCase("1days ago need pay 1 hole", 6390u, 9, 0u)] //already 
        [TestCase("2days ago need pay 0 hole", 6390u, 10, 0u)] //already 
        [TestCase("1days ago need pay 0 hole", 6390u, 11, 0u)] //already 

        [TestCase("r9days ago need pay 2 hole", 6390u, 0, 90001u)] //full success
        [TestCase("r9days ago need pay 1 hole", 6390u, 1, 90001u)] // 
        [TestCase("r9days ago need pay 0 hole", 6390u, 2, 90001u)] //already 
        [TestCase("r0day need pay 2 hole", 6390u, 3, 90001u)] //already 
        [TestCase("r0day need pay 1 hole", 6390u, 4, 90001u)] //already 
        [TestCase("r0day need pay 0 hole", 6390u, 5, 90001u)] //already 
        [TestCase("r2days ago need pay 2 hole", 6390u, 6, 90001u)] //already 
        [TestCase("r1days ago need pay 2 hole", 6390u, 7, 90001u)] //already 
        [TestCase("r2days ago need pay 1 hole", 6390u, 8, 90001u)] //already 
        [TestCase("r1days ago need pay 1 hole", 6390u, 9, 90001u)] //already 
        [TestCase("r2days ago need pay 0 hole", 6390u, 10, 90001u)] //already 
        [TestCase("r1days ago need pay 1 hole", 6390u, 11, 90001u)] //already 

        [TestCase("n0days 0 0 none need pay 0 hole", 6390u, 12, 0u)] //already 
        [TestCase("n0days 0 1 need pay 1 hole", 6390u, 13, 0u)] //already 
        [TestCase("nr0days 0 0 none need pay 0 hole", 6390u, 14, 90001u)] //already 
        [TestCase("nr0days 1 0 need pay 0 hole", 6390u, 15, 90001u)] //already 

        [TestCase("issue 1 0 need pay 0 hole", 6390u, 16, 0u)] //already 

        public async Task NewQuestTest(string casename, uint userId, int initIndex, uint targetGroupId)
        {
            using (DBContext db = new DBContext(7))
            {
                await db.BeginTransactionCallback(async () =>
                {
                    Console.WriteLine(casename);

                    List<string> updateQuery = new List<string>();
                    updateQuery.Add("UPDATE users SET gem = 10000 , gold = 10000 WHERE userId =" + userId + ";");
                    updateQuery.Add("UPDATE quests SET dynamicQuestId = " + caseDatas[initIndex].quest90001 + " WHERE userId = " + userId +
                                       " AND id = 90001;");
                    updateQuery.Add("UPDATE quests SET dynamicQuestId = " + caseDatas[initIndex].quest90002 + " WHERE userId = " + userId +
                                       " AND id = 90002;");
                    updateQuery.Add("UPDATE user_info SET dailyQuestRefreshCount = " + caseDatas[initIndex].refreshCount +
                                    ", dailyQuestAddCount =" + caseDatas[initIndex].addCount +
                                    ", lastRefreshDate ='" + caseDatas[initIndex].lastRefreshDate +
                                    "' WHERE userId = " + userId + ";");
                    foreach (var query in updateQuery)
                        await db.ExecuteNonQueryAsync(query);

                    var questItem = new DailyQuest(userId, targetGroupId, db);
                    var res = await questItem.CreateNew();

                    System.Console.WriteLine("M.Type:" + res.MaterialType.ToString()
                                        + "\nM.RemainedValue:" + res.RemainedMaterials
                                        + "\nTarget:" + (res.QuestGroupAndDynamicList == null ? "None" : string.Join("|", res.QuestGroupAndDynamicList.Select(keyvalue => keyvalue.Key.ToString() + ":" + keyvalue.Value.ToString())))
                                        + "\nAddCount:" + res.AddCount
                                        + "\nRefreshCount:" + res.RefreshCount
                                        + "\nResult:" + res.ResultType.ToString()
                                        );

                    Console.WriteLine("\n updated\n" +
                                       "refCount:" + caseDatas[initIndex].refreshCount + "  addCount:" + caseDatas[initIndex].addCount + " refDate:" + caseDatas[initIndex].lastRefreshDate
                                       + "quests:90001-" + caseDatas[initIndex].quest90001 + ",  90002-" + caseDatas[initIndex].quest90002);

                    var resultQuery = "SELECT  dailyQuestRefreshCount, dailyQuestAddCount, lastRefreshDate FROM user_info WHERE userId =" + userId + ";";
                    using (var cursor = await db.ExecuteReaderAsync(resultQuery))
                    {
                        cursor.Read();
                        Console.WriteLine("\n refCount:" + cursor.GetInt32(0) + "  addCount:" + cursor.GetInt32(1) + " refDate:" + cursor.GetDateTime(2).ToTimeString());
                    }

                    resultQuery = "SELECT  id, dynamicQuestId  FROM quests WHERE questType = 400 AND userId =" + userId + ";";
                    using (var cursor = await db.ExecuteReaderAsync(resultQuery))
                    {
                        while (cursor.Read())
                            Console.WriteLine("id:" + cursor.GetInt32(0) + "  dynid:" + cursor.GetInt32(1));
                    }


                    return true;
                });

            }

        }



        class InitQuestDbData
        {
            public string casename;
            public int performCount;
            public int receivedOrder;
            public int completedOrder;
            public string activeTime;
            public DateTime now;
        }

        static InitQuestDbData[] caseQuestDatas = new InitQuestDbData[]
        {
            new InitQuestDbData { casename = "3 days ago", performCount =3, receivedOrder = 0, completedOrder = 0 , activeTime = DateTime.UtcNow.AddDays(-3).ToTimeString() }
            ,new InitQuestDbData { casename = "2 days ago", performCount =3, receivedOrder = 0, completedOrder = 0 , activeTime = DateTime.UtcNow.AddDays(-2).ToTimeString() }
            ,new InitQuestDbData { casename = "1 days ago", performCount =3, receivedOrder = 0, completedOrder = 0 , activeTime = DateTime.UtcNow.AddDays(-1).ToTimeString() }
            ,new InitQuestDbData { casename = "today", performCount =3, receivedOrder = 0, completedOrder = 0 , activeTime = DateTime.UtcNow.AddDays(0).ToTimeString() }
            ,new InitQuestDbData { casename = "3 days ago not getting reward", performCount =4, receivedOrder = 0, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-3).ToTimeString() }
            ,new InitQuestDbData { casename = "2 days ago not getting reward", performCount =4, receivedOrder = 0, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-2).ToTimeString() }
            ,new InitQuestDbData { casename = "1 days ago not getting reward", performCount =4, receivedOrder = 0, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-1).ToTimeString() }
            ,new InitQuestDbData { casename = "today not getting reward", performCount =4, receivedOrder = 0, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(0).ToTimeString() }
            ,new InitQuestDbData { casename = "3 days ago getting reward", performCount =4, receivedOrder = 1, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-3).ToTimeString() }
            ,new InitQuestDbData { casename = "2 days ago getting reward", performCount =4, receivedOrder = 1, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-2).ToTimeString() }
            ,new InitQuestDbData { casename = "1 days ago getting reward", performCount =4, receivedOrder = 1, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(-1).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =4, receivedOrder = 1, completedOrder = 1 , activeTime = DateTime.UtcNow.AddDays(0).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =3, receivedOrder = 0, completedOrder = 1 , activeTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2,1,0).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =3, receivedOrder = 0, completedOrder = 1 , activeTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 1,0,0).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =3, receivedOrder = 0, completedOrder = 1 , activeTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day -1, 3,0,0).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =3, receivedOrder = 0, completedOrder = 1 , activeTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day +1, 2,1,0).ToTimeString() }
            ,new InitQuestDbData { casename = "today getting reward", performCount =3, receivedOrder = 0, completedOrder = 1 , activeTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day +1, 1,0,0).ToTimeString() }
        };

        [TestCase("3 days ago", 6390u, 0, 999999u)]
        [TestCase("2 days ago", 6390u, 1, 999999u)]
        [TestCase("1 days ago", 6390u, 2, 999999u)]
        [TestCase("today", 6390u, 3, 999999u)]
        [TestCase("3 days ago not getting reward", 6390u, 4, 999999u)]
        [TestCase("2 days ago not getting reward", 6390u, 5, 999999u)]
        [TestCase("1 days ago not getting reward", 6390u, 6, 999999u)]
        [TestCase("today not getting reward", 6390u, 7, 999999u)]
        [TestCase("3 days ago getting reward", 6390u, 8, 999999u)]
        [TestCase("2 days ago getting reward", 6390u, 9, 999999u)]
        [TestCase("1 days ago getting reward", 6390u, 10, 999999u)]
        [TestCase("today getting reward", 6390u, 11, 999999u)]

        [TestCase("base over 1min ", 6390u, 12, 999999u)]
        [TestCase("base less 1hour ", 6390u, 13, 999999u)]
        [TestCase("base less 23hour", 6390u, 14, 999999u)]
        [TestCase("base over 1day", 6390u, 15, 999999u)]
        [TestCase("base over 23hour", 6390u, 16, 999999u)]
        public async Task DailyRangeQuestTest(string casename, uint userId, int initIndex, uint targetGroupId)
        {
            using (DBContext db = new DBContext(7))
            {
                var query = "UPDATE quests SET performCount = " + caseQuestDatas[initIndex].performCount +
                                ", receivedOrder=" + caseQuestDatas[initIndex].receivedOrder +
                                ", completedOrder=" + caseQuestDatas[initIndex].completedOrder +
                                ", activeTime='" + caseQuestDatas[initIndex].activeTime +
                                "' WHERE userId = " + userId + " AND id = " + targetGroupId + ";";
                await db.ExecuteNonQueryAsync(query);

          //      query = "SELECT * FROM quests WHERE userId =" + userId + " AND id = " + targetGroupId + ";";
          //      using (var cursor = await db.ExecuteReaderAsync(query))
                {
                    var qio = new QuestIO(userId, db);
                    var result = await qio.GetQuestWithType(QuestType.ChallengeConditioner);//qio.Select(cursor);

                    var sresult = result.Select(quest => "" + quest.PerformCount + "  " + quest.ReceivedOrder + "    " + quest.CompleteOrder + "  " + quest.ActiveTime);

                    Console.WriteLine(caseQuestDatas[initIndex].activeTime);
                    foreach (var str in sresult)
                        Console.WriteLine(str);
                }




            }
        }
    }

    static class DateTimeExtension
    {
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
