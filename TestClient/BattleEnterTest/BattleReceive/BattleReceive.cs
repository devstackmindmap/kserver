using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class BattleReceive
    {
        public static void Run(BaseProtocol proto, MessageType msgType, AsyncTcpSession session)
        {
            switch(msgType)
            {
                case MessageType.BeforeBattleStart:
                    (new TestBeforeBattleStart()).Run(proto, session);
                    break;
                case MessageType.EnterRoomFail:
                    (new TestEnterRoomFail()).Run(proto, session);
                    break;
                case MessageType.BattleChallengeRoundResult:
                    (new TestChallengeRoundResult()).Run(proto, session);
                    break;
                case MessageType.GetBattleResultChallenge:
                    (new TestGetBattleResult()).Run(proto, session);
                    break;
                case MessageType.BattleEventChallengeRoundResult:
                    (new TestEventChallengeRoundResult()).Run(proto, session);
                    break;
                default:
                    Console.WriteLine(msgType.ToString());
                    break;
            }
        }
    }
}
