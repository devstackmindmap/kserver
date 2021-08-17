using AkaRedisLogic;
using CommonProtocol;
using StackExchange.Redis;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class MainServer : AppServer<NetworkSession, BinaryRequestInfo>
    {
        private int _areaIndex;
        private int _matchingLine;
        private IDatabase _redis;
        public bool EndingMainProcess = false;

        private MainServer()
        : base(new DefaultReceiveFilterFactory<CustomReceiveFilter, BinaryRequestInfo>())
        {
            NewRequestReceived += new RequestHandler<NetworkSession, BinaryRequestInfo>(RequestReceived);
            NewSessionConnected += MainServer_NewSessionConnected;
            SessionClosed += MainServer_SessionClosed;
            _redis = AkaRedis.AkaRedis.GetDatabase();
            Task.Factory.StartNew(SessionChecker);
        }

        private void MainServer_SessionClosed(NetworkSession session, CloseReason value)
        {
            //if (value == CloseReason.SocketError)
            //{
            //    AkaLogger.Logger.Instance().Info("SocketError : " + value.ToString());
            //    _ = MatchingSessionClosed.CleanRoomAsync(session.SessionID);
            //}
        }

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

        private void RequestReceived(NetworkSession session, BinaryRequestInfo requestInfo)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            var intSize = sizeof(int);

            while (offset < requestInfo.Body.Length)
            {
                MessageType msgType = MessageType.None;
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
                    AkaLogger.Log.Debug.InvalidPacket(msg, "Match");
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
                    AkaLogger.Log.Debug.InvalidProtocol(msg, "Match");
                    return;
                }

                try
                { 
                    var controller = ControllerFactory.CreateController(msgType, _areaIndex, _matchingLine);
                    controller.DoPipeline(session, proto);
                }
                catch(Exception e)
                {
                    AkaLogger.Log.Debug.Exception("Matching:" + msgType.ToString(), e);
                    AkaLogger.Logger.Instance().Error(e.ToString());
                    return;
                }

            }
        }

        private void MainServer_NewSessionConnected(NetworkSession session)
        {
        }

        public void InitMatchingLine(int areaIndex, int matchingLine)
        {
            _areaIndex = areaIndex;
            _matchingLine = matchingLine;
            var redis = AkaRedis.AkaRedis.GetDatabase();
            MatchingServerInfoJob.SetServerStateInfoAsync(redis, areaIndex, matchingLine, matchingLine).Wait();
        }

        public void RemoveMatchingLine()
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            MatchingServerInfoJob.RemoveServerStateInfo(redis, _areaIndex, _matchingLine);
        }

        private async Task SessionChecker()
        {
            var baseDelaySessionCount = AkaConfig.Config.MatchingServerConfig.BaseDelaySessionCount;

            while (true)
            {
                await Task.Delay(1000);

                if (MainServer.Instance.SessionCount > baseDelaySessionCount)
                    await MatchingServerInfoJob.SetServerStateInfoAsync(_redis, _areaIndex, _matchingLine, MainServer.Instance.SessionCount);
                else if (Instance.SessionCount <= _matchingLine && _areaIndex != 0 && EndingMainProcess == false)
                    await MatchingServerInfoJob.SetServerStateInfoAsync(_redis, _areaIndex, _matchingLine, _matchingLine);
            }
        }
    }
}
