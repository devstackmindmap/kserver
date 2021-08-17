using CommonProtocol;
using SuperSocket.ClientEngine;
using System;

namespace TestClient.BattleTest
{
    public class TestBeforeBattleStart : TestBaseController
    {
        public override void Run(BaseProtocol proto, AsyncTcpSession session)
        {
            Console.WriteLine("Receive BeforeBattleStart");

            var receivePacket = proto as ProtoBeforeBattleStart;
            foreach (var unit in receivePacket.EnemyPlayer.Units)
            {
                Console.WriteLine("EnemyUnitId :" + unit.UnitId);
            }
        }
    }
}
