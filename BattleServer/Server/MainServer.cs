using AkaLogger;
using CommonProtocol;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;

namespace BattleServer
{
    public class MainServer : AppServer<NetworkSession, BinaryRequestInfo>
    {
        private MainServer()
        : base(new DefaultReceiveFilterFactory<CustomReceiveFilter, BinaryRequestInfo>())
        {
            NewRequestReceived += new RequestHandler<NetworkSession, BinaryRequestInfo>(RequestReceived);
            NewSessionConnected += MainServer_NewSessionConnected;
            SessionClosed += new SessionHandler<NetworkSession, CloseReason>(MainServer_SessionClosed);


            PerformanceMonitor.Instance.StartMonitor();
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
            PerformanceMonitor.Instance.Dispose();
            disposed = true;
        }

        private void MainServer_SessionClosed(NetworkSession session, CloseReason reason)
        {
            //Console.WriteLine(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));
        }
        
        private void MainServer_NewSessionConnected(NetworkSession session)
        {
            //Console.WriteLine("NewSessionConnected : " + session.SessionID);
        }


        private async void RequestReceived(NetworkSession session, BinaryRequestInfo requestInfo)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            var intSize = sizeof(int);

            while (offset < requestInfo.Body.Length)
            {
                BaseController controller = null;
                MessageType msgType = MessageType.None;
                var checkTime = DateTime.Now;
                byte[] body = null; 

                try
                {
                    var header = requestInfo.Body.CloneRange(offset, sizeHeader);
                    var bodyLength = requestInfo.Body.CloneRange(offset + sizeHeader, intSize);
                    var realBodyLength = BitConverter.ToInt32(bodyLength, 0);
                    body = requestInfo.Body.CloneRange(offset + intSize + sizeHeader, realBodyLength);

                    offset += (sizeHeader + intSize + realBodyLength);

                    msgType = (MessageType)BitConverter.ToInt32(header, 0);
                }
                catch(Exception)
                {
                    var msg = $"{msgType.ToString()}_{ (requestInfo.Body?.Length ?? 0).ToString()}_{session.SocketSession.RemoteEndPoint.ToString()}";
                    AkaLogger.Log.Debug.InvalidPacket(msg, "Battle");
                    return;
                }

                BaseProtocol proto = null;
                try
                {
                    proto = ProtocolFactory.DeserializeProtocol(msgType, body);
                }
                catch(Exception)
                {
                    var msg = $"{msgType.ToString()}_{ body.Length.ToString()}_{session.SocketSession.RemoteEndPoint.ToString()}";
                    AkaLogger.Log.Debug.InvalidProtocol(msg, "Battle");
                    return;
                }
                
                try
                {
                    controller = ControllerFactory.CreateController(msgType);
                    await controller.DoPipeline(session, proto);
                }
                catch(Exception ex)
                {
                    AkaLogger.Log.Debug.Exception("BattleRoot:" + msgType.ToString(), ex);
                    AkaLogger.Logger.Instance().Error(ex.ToString());
                    return;
                }

                if (controller != null && false == (controller is Controller.Controllers.SyncTime))
                {
                    var time = (DateTime.Now - checkTime).TotalMilliseconds;
                    PerformanceMonitor.Instance.AddMessage( controller.GetType().Name, (int)time);
                }
            }
        }
    }
}