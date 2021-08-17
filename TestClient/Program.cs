using AkaConfig;
using AkaEnum;
using AkaEnum.Battle;
using CommonProtocol;
using CommonProtocol.PubSub;
using MySql.Data.MySqlClient;
using Network;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TestClient.BattleTest;
using TestClient.MatchingTest;
using TestClient.PubSubTest;

namespace TestClient
{
    class Program
    {
        public static uint UserId;
        static void Main(string[] args)
        {
            BattleClient.Run();
            //MatchingClient.Run();
            //PubSubClient.Run();
            //StartUWPLogging();
            //Test2();
            //RedisTest();
            //   GetBattleRecords();
            //Test3();
        }

        private static void Test3()
        {
            var configurationOptions = new ConfigurationOptions
            {
                DefaultDatabase = 0,
                Password = "rkrufhdi34!"
            };

            configurationOptions.EndPoints.Add("172.30.1.222", 6379);
            
            var con = ConnectionMultiplexer.Connect(configurationOptions);
            var subscriber = con.GetSubscriber();

            subscriber.Subscribe("a", (RedisChannel ch, RedisValue val) =>
            {
                Console.WriteLine((string)val);
            });

            Console.ReadLine();
        }

        private static void RedisTest()
        {
            Connector.Instance.Connect("172.30.1.224", 40554);
            while (Connector.Instance.IsConnected() == false) ;

            Connector.Instance.Send(MessageType.TryMatching, AkaSerializer.AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching
            {
                BattleServerIp = "172.30.1.224",
                BattleType = BattleType.LeagueBattle,
                DeckNum = 0,
                MessageType = MessageType.TryMatching,
                UserId = 61

            }));

            Console.ReadLine();
        }

        private static void Test2()
        {
            //List<Connector> connectors = new List<Connector>();
            //for (int i = 0; i < 10; i++)
            //{
            //    connectors.Add(new Connector());
            //    connectors[i].Connect("172.30.1.222", 50507);
            //}

            //while (true)
            //{
            //    bool allConnected = true;
            //    for(int i = 0; i < 10; i++)
            //    {
            //        if(connectors[i].IsConnected() == false)
            //        {
            //            allConnected = false;
            //        }
            //    }

            //    if (allConnected)
            //        break;
            //}

            //for (int i = 0; i < 10; i++)
            //{
            //    connectors[i].Send(1, Encoding.UTF8.GetBytes("Hi"));
            //}

            //Console.ReadLine();
        }
        //*/


        async static void GetBattleRecords()
        {
            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Port = (uint)3306;
            stringBuilder.Server = "172.30.1.224";
            stringBuilder.Database = "knightrun";
            stringBuilder.UserID = "root";
            stringBuilder.Password = "aka10";
            stringBuilder.Pooling = true;
            stringBuilder.MinimumPoolSize = 10;
            stringBuilder.MaximumPoolSize = 100;
            stringBuilder.SslMode = MySqlSslMode.None;
            stringBuilder.CharacterSet = "utf8";


            var connection = new MySqlConnection(stringBuilder.ConnectionString);

            connection.Open();

            var query = "SELECT behaviorsId, behaviors FROM battle_record_behaviors ORDER BY behaviorsId;";
            var Command = new MySqlCommand(query, connection);

            var Cursor = await Command.ExecuteReaderAsync();
            int count = 1;
            while (Cursor.Read())
            {
                var readByte = (byte[])Cursor["behaviors"];
                var record = AkaSerializer.AkaSerializer<ProtoBattleRecord>.Deserialize(readByte);

                var units1 = string.Join(",", record.BattleInfo.BattleStartInfo[PlayerType.Player1].Units.Select(unit => unit.UnitId) );
                var units2 = string.Join(",", record.BattleInfo.BattleStartInfo[PlayerType.Player2].Units.Select(unit => unit.UnitId));
                var userid1 =  record.BattleInfo.BattleStartInfo[PlayerType.Player1].UserId;
                var userid2 =  record.BattleInfo.BattleStartInfo[PlayerType.Player2].UserId;
                Console.WriteLine($"play{count++}  winner:{record.BattleInfo.Winner}, 1 - {userid1} units:{units1} ,   2 - {userid2} units:{units2}");
            }

        }
    }
}
