using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class TestChallengeRoundResult : TestBaseController
    {
        public override void Run(BaseProtocol proto, AsyncTcpSession session)
        {
            Console.WriteLine("Receive TestBattleRoundResult");

            BattleServerConnector.Instance.Send(MessageType.EnterChallengeRoom, AkaSerializer.AkaSerializer<CommonProtocol.ProtoEnterChallengeRoom>.
                Serialize(new ProtoEnterChallengeRoom
                {
                    MessageType = MessageType.EnterChallengeRoom,
                    UserId = Program.UserId,
                    DeckNum = 0,
                    BattleServerIp = BattleClient.ip,
                    BattleType = AkaEnum.Battle.BattleType.Challenge,
                    Season = 1,
                    Day = 1,
                    DifficultLevel = 1,
                    IsStart = false
                }));
        }
    }
}
