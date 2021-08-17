using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class TestGetBattleResult : TestBaseController
    {
        public override void Run(BaseProtocol proto, AsyncTcpSession session)
        {
            var receivePacket = proto as ProtoOnBattleResult;
            Console.WriteLine("Receive GetBattleResult : " + receivePacket.BattleResultType.ToString());
        }
    }
}
