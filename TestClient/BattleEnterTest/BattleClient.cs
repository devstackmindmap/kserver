using AkaSerializer;
using CommonProtocol;
using System;

namespace TestClient.BattleTest
{
    public class BattleClient
    {
        public static string ip = "172.30.1.222";
        public static void Run()
        {
            BattleServerConnector.Instance.Connect(ip, 30654);
            while (BattleServerConnector.Instance.IsConnected() == false) ;
                       
            Console.Write("UserId(end:q):");
            uint userId = uint.Parse(Console.ReadLine());

            Program.UserId = userId;

            //BattleServerConnector.Instance.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.
            //    Serialize(new ProtoTryMatching
            //    {
            //        MessageType = MessageType.TryMatching,
            //        BattleServerIp = "172.30.1.222",
            //        BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
            //        DeckNum = 0,
            //        GroupCode = 1,
            //        UserId = userId
            //    }));

            var battleSend = new BattleSend(userId);
            battleSend.Run(ip);
        }
    }
}
