using AkaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlerClient
{
    public class Run
    {
        public TcpClient client2 = new TcpClient();

        public void Execute(Object o = null)
        {
            var client = new TcpClient();
            client.ConnectToServer("127.0.0.1", Config.config.ServerSetting.BattleServerPort);
            client.Send(Convert.ToUInt16(o));
        }
        
        public void Connect()
        {
            client2.ConnectToServer("127.0.0.1", Config.config.ServerSetting.BattleServerPort);
        }

        public void Execute2(Object o = null)
        {            
            client2.Send(Convert.ToUInt16(o));
        }
    }
}
