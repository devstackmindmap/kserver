using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaSerializer;
using Common.Entities.Item;
using CommonProtocol;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using AkaConfig;
using AkaUtility;
using System;
using System.Linq;
using TestHelper;
using System.IO.Compression;
using System.IO;

namespace WebLogicTest
{
    public class BattleRecordTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelper.TestHelper.SetUp(TestContext.Parameters["runMode"]);
        }

        public ProtoOnGetBattleRecordList GetRecordListTest(int userid)
        {

            var webServerUri = $"http://127.0.0.1:{Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.GetBattleRecordList, AkaSerializer<ProtoGetBattleRecordList>.Serialize(new ProtoGetBattleRecordList
            {
                UserId = (uint)userid,
                MessageType = MessageType.GetBattleRecordList,
                BattleTypeList = new List<BattleType> { BattleType.LeagueBattle, BattleType.LeagueBattleAi },

            }), webServerUri);

            var protoResult = AkaSerializer<ProtoOnGetBattleRecordList>.Deserialize(result);
            return protoResult;
        }


        private ProtoBattleRecord GetRecordTest(string recordKey)
        {
            var webServerUri = $"http://127.0.0.1:{Config.GameServerConfig.GameServerPort}/";

            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.GetBattleRecord, AkaSerializer<ProtoGetBattleRecord>.Serialize(new ProtoGetBattleRecord
            {
                MessageType = MessageType.GetBattleRecord,
                RecordKey = recordKey

            }), webServerUri);


            var protoResult = AkaSerializer<ProtoOnGetBattleRecord>.Deserialize(result);
            if (protoResult.ArchivedData?.Length > 0)
            {
                using (var ms = new MemoryStream(protoResult.ArchivedData, false))
                {
                    using (var outMs = new MemoryStream())
                    {
                        using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                        {
                            ds.CopyTo(outMs);
                        }
                        var recordData = outMs.ToArray();
                        return AkaSerializer<ProtoBattleRecord>.Deserialize(recordData);
                    }
                }

            }
            return new ProtoBattleRecord();
        }


     //   [TestCase(99999, 0, TestName = "없는 userId", Category = Category.BattleRecord)]
        [TestCase(6338, 2, TestName = "있는 userId", Category = Category.BattleRecord)]
        public void GetRecordList(int userId, int matchingCount)
        {
            var list = GetRecordListTest(userId);
            Console.WriteLine(list.BattleRecordList.Count);
          //  Assert.That(list.BattleRecordList.Count == matchingCount);
        }



        [TestCase(424, TestName = "전투기록 목록 열람", Category = Category.BattleRecord)]
        public void GetRecordofOne(int userid)
        {
            var list = GetRecordListTest(userid);
            foreach (var battleRecord in list.BattleRecordList)
            {
                Console.WriteLine($"Db info");
                Console.WriteLine($"UserId:{battleRecord.UserId}");
                Console.WriteLine($"EnemyId:{battleRecord.EnemyUserId}");
                Console.WriteLine($"BattleType:{battleRecord.BattleType}");
                Console.WriteLine($"RecordKey:{battleRecord.RecordKey}");
                Console.WriteLine($"Returned MessageType:{battleRecord.MessageType}");
                Console.WriteLine($"Start:{new DateTime(battleRecord.BattleStartTime)}");
                Console.WriteLine($"End:{new DateTime(battleRecord.BattleEndTime)}");
                Console.WriteLine($"Behaviors Is Null:{battleRecord.Behaviors == null}");
                Console.WriteLine($"Winner:{battleRecord.BattleInfo.Winner}");

                var player1 = battleRecord.BattleInfo.BattleStartInfo[PlayerType.Player1];
                var player2 = battleRecord.BattleInfo.BattleStartInfo[PlayerType.Player2];

                Console.WriteLine("Player1 Info");
                Console.WriteLine($"\tUserId:{player1.UserId}");
                Console.WriteLine($"\tNickName:{player1.NickName }");
                Console.WriteLine($"\tScore:{player1.Score }");
                Console.WriteLine($"\tGetting RankingPoint:{player1.GettingRankPoint }");
                Console.WriteLine($"\tUser RankingPoint:{player1.UserRankPoint }");
                Console.WriteLine($"\tTeam RankingPoint:{player1.TeamRankPoint }");
                Console.WriteLine($"\tCardList:{ string.Join(",", player1.CardStatIds) }");
                Console.WriteLine($"\tUnit List:{ string.Join(",", player1.Units.Select(unit => unit.UnitId)) }");

                Console.WriteLine("Player2 Info");
                Console.WriteLine($"\tUserId:{player2.UserId}");
                Console.WriteLine($"\tNickName:{player2.NickName }");
                Console.WriteLine($"\tScore:{player2.Score }");
                Console.WriteLine($"\tGetting RankingPoint:{player2.GettingRankPoint }");
                Console.WriteLine($"\tUser RankingPoint:{player2.UserRankPoint }");
                Console.WriteLine($"\tTeam RankingPoint:{player2.TeamRankPoint }");
                Console.WriteLine($"\tCardList:{ string.Join(",", player2.CardStatIds == null ? new uint[] { 0 } : player2.CardStatIds.ToArray()) }");
                Console.WriteLine($"\tUnit List:{ string.Join(",", player2.Units.Select(unit => unit.UnitId)) }");
            }


            var webServerUri = $"http://127.0.0.1:{Config.GameServerConfig.GameServerPort}/";
            /*
            WebServerRequestor webServer = new WebServerRequestor();
            var result = webServer.Request(MessageType.GetBattleRecord, AkaSerializer<ProtoGetBattleRecord>.Serialize(new ProtoGetBattleRecord
            {
                UserId = (uint)userid,
                MessageType = MessageType.GetBattleRecord,
                BehaviorsId = behaviorsId,
            }), webServerUri);

            var protoResult = AkaSerializer<ProtoBattleRecord>.Deserialize(result);
            */
        }


        [TestCase(2, TestName = "전투기록 재생", Category = Category.BattleRecord)]
        public void GetRecordPlay(int userid)
        {
            var list = GetRecordListTest(userid);
            foreach (var battleRecord in list.BattleRecordList)
            {
                var recordData = GetRecordTest( battleRecord.RecordKey);

                if (recordData.UserId == 0)
                    Console.WriteLine($"{battleRecord.RecordKey} File Not Exists");
                else
                    Console.WriteLine($"{battleRecord.RecordKey}  history count is {recordData.Behaviors.Count}");
                
            }

        }

    }
}
