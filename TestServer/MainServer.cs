using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class MainServer : AppServer<NetworkSession, BinaryRequestInfo>
    {
        private MainServer()
        : base(new DefaultReceiveFilterFactory<CustomReceiveFilter, BinaryRequestInfo>())
        {
            NewRequestReceived += new RequestHandler<NetworkSession, BinaryRequestInfo>(RequestReceived);
            NewSessionConnected += MainServer_NewSessionConnected;
            SessionClosed += new SessionHandler<NetworkSession, CloseReason>(MainServer_SessionClosed);

        }

        private bool disposed = false;
        private static MainServer _instance = null;
        public static MainServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainServer();
                }
                return _instance;
            }
        }

        public new void Dispose()
        {
            base.Dispose();
        }

        private void MainServer_SessionClosed(NetworkSession session, CloseReason reason)
        {
            Console.WriteLine(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));
        }

        private void MainServer_NewSessionConnected(NetworkSession session)
        {
            Console.WriteLine("NewSessionConnected : " + session.SessionID);
        }

        private void RequestReceived(NetworkSession session, BinaryRequestInfo requestInfo)
        {
            Console.WriteLine($"Received:ST:{session.SessionID}");
            int offset = 0;
            var sizeHeader = sizeof(int);
            var intSize = sizeof(int);

            int msgType = 0;
            string text = "";
            while (offset < requestInfo.Body.Length)
            {
                var header = requestInfo.Body.CloneRange(offset, sizeHeader);
                var bodyLength = requestInfo.Body.CloneRange(offset + sizeHeader, intSize);
                var realBodyLength = BitConverter.ToInt32(bodyLength, 0);
                var body = requestInfo.Body.CloneRange(offset + intSize + sizeHeader, realBodyLength);

                offset += (sizeHeader + intSize + realBodyLength);

                msgType = (int)BitConverter.ToInt32(header, 0);
                text = Encoding.Default.GetString(body);
            }
            Console.WriteLine($"Received:ED:{session.SessionID}");
            //var toDateTime = DateTime.UtcNow.AddSeconds(1);
            //var strMsgType = msgType.ToString();
            //var run = new Run();
            //for (int i = 0; i < 100; i++)
            //{
            //    run.Go(i, strMsgType, text);
            //}
        }
    }

    class Run
    {
        public async Task Go(int i, string strMsgType, string text)
        {
            await Task.Delay(1);
            if (i % 10 == 0)
                Console.WriteLine($"{strMsgType}:{text}");
        }
    }
}