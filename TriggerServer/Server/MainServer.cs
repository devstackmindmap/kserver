using CommonProtocol;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;

namespace TriggerServer
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
            //Console.WriteLine(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));
            
        }

        private void MainServer_NewSessionConnected(NetworkSession session)
        {
            //Console.WriteLine("NewSessionConnected : " + session.SessionID);
        }

        private void RequestReceived(NetworkSession session, BinaryRequestInfo requestInfo)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            var intSize = sizeof(int);
            while (offset < requestInfo.Body.Length)
            {
                MessageType msgType = MessageType.None;
                try
                {
                    var header = requestInfo.Body.CloneRange(offset, sizeHeader);
                    var bodyLength = requestInfo.Body.CloneRange(offset + sizeHeader, intSize);
                    var realBodyLength = BitConverter.ToInt32(bodyLength, 0);
                    var body = requestInfo.Body.CloneRange(offset + intSize + sizeHeader, realBodyLength);

                    offset += (sizeHeader + intSize + realBodyLength);

                    msgType = (MessageType)BitConverter.ToInt32(header, 0);
                    var proto = ProtocolFactory.DeserializeProtocol(msgType, body);

                    var controller = ControllerFactory.CreateController(msgType);
                    controller.DoPipeline(session, proto, msgType);
                }
                catch(Exception e)
                {
                    AkaLogger.Log.Debug.Exception("Trigger:" + msgType.ToString(), e);
                    AkaLogger.Logger.Instance().Error(e.ToString());
                    return;
                }
            }
        }
    }
}