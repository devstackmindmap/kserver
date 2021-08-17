using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class TestEnterRoomFail : TestBaseController
    {
        public override void Run(BaseProtocol proto, AsyncTcpSession session)
        {
            var receivePacket = proto as ProtoResult;
            Console.WriteLine(receivePacket.ResultType.ToString());
        }
    }
}
