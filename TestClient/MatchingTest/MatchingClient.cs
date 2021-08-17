using AkaSerializer;
using CommonProtocol;
using System;
using System.Collections.Generic;

namespace TestClient.MatchingTest
{
    public class MatchingClient
    {
        public static void Run()
        {
            MatchigServerConnector.Instance.Connect("172.30.1.222", 40554);
            while (MatchigServerConnector.Instance.IsConnected() == false) ;
                       
            Console.Write("UserId(end:q):");
            uint userId = uint.Parse(Console.ReadLine());

            MatchigServerConnector.Instance.Send(MessageType.TryMatching, AkaSerializer<ProtoTryMatching>.
                Serialize(new ProtoTryMatching
                {
                    MessageType = MessageType.TryMatching,
                    BattleServerIp = "172.30.1.222",
                    BattleType = AkaEnum.Battle.BattleType.LeagueBattle,
                    DeckNum = 0,
                    GroupCode = 1,
                    UserId = userId
                }));

            MatchingSend.Run(userId);
        }
    }
}
