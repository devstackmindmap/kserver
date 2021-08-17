using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;

namespace BattlerClient
{
    public class TcpClient
    {
        public AsyncTcpSession asyncTcpSession = new AsyncTcpSession();
        public void ConnectToServer(string host, int port)
        {
            asyncTcpSession.Connect(new IPEndPoint(IPAddress.Parse(host), port));
        }

        public void Send(uint o)
        {
           
        }
    }
}
