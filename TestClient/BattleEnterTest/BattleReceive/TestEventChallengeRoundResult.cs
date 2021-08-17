using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class TestEventChallengeRoundResult : TestBaseController
    {
        public override void Run(BaseProtocol proto, AsyncTcpSession session)
        {
            Console.WriteLine("Receive TestBattleRoundResult");

            BattleServerConnector.Instance.Send(MessageType.EnterEventChallengeRoom, AkaSerializer.AkaSerializer<CommonProtocol.ProtoEnterEventChallengeRoom>.
                Serialize(new ProtoEnterEventChallengeRoom
                {
                    MessageType = MessageType.EnterEventChallengeRoom,
                    UserId = Program.UserId,
                    DeckNum = 0,
                    BattleServerIp = BattleClient.ip,
                    BattleType = AkaEnum.Battle.BattleType.EventChallenge,
                    ChallengeEventId = 1,
                    DifficultLevel = 1,
                    IsStart = false
                }));
        }
    }
}
