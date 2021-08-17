using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.BattleTest
{
    public abstract class TestBaseController
    {
        public abstract void Run(BaseProtocol proto, AsyncTcpSession session);
    }
}
