using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TestHelper.Matching
{
    public class MatchingResultState
    {
        public MessageType receivedMessage;
        public string RoomId;
    };

    public class MatchingServerConnector
    {
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();
        private ManualResetEvent _connectionEvent = new ManualResetEvent(false);
        private ManualResetEvent _receiveEvent = new ManualResetEvent(false);

        private string _ip;
        private int _port;

        private MatchingResultState _resultState;

        public MatchingServerConnector(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Connect()
        {
            if (!_asyncTcpSession.IsConnected)
            {
                _asyncTcpSession.Connected += AsyncTcpSession_Connected; ;
                _asyncTcpSession.DataReceived += AsyncTcpSession_DataReceived;
                _asyncTcpSession.Closed += AsyncTcpSession_Closed;
                _asyncTcpSession.Connect(new IPEndPoint(IPAddress.Parse(_ip), _port));
            }
        }

        public bool WaitConnected()
        {
            _connectionEvent.WaitOne();
            return IsConnected();
        }

        public bool IsConnected()
        {
            return _asyncTcpSession.IsConnected;
        }


        public void Send(MessageType messageType, byte[] data)
        {
            var header = BitConverter.GetBytes((int)messageType);
            using (var stream = new MemoryStream(header.Length + sizeof(int) + data.Length))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                _asyncTcpSession.Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }


        public void Close()
        {
            try
            {
                _asyncTcpSession.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

        }

        private void AsyncTcpSession_DataReceived(object sender, DataEventArgs e)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            var intSize = sizeof(int);
            while (offset < e.Length)
            {
                var header = e.Data.CloneRange(offset, sizeHeader);
                var bodyLength = e.Data.CloneRange(offset + sizeHeader, intSize);
                var realBodyLength = BitConverter.ToInt32(bodyLength, 0);
                var body = e.Data.CloneRange(offset + intSize + sizeHeader, realBodyLength);

                offset += (sizeHeader + intSize + realBodyLength);

                var msgType = (MessageType)BitConverter.ToInt32(header, 0);
                var proto = ProtocolFactory.DeserializeProtocol(msgType, body);

                SetMatchingState(msgType, proto);
            }
        }

        private void SetMatchingState(MessageType msgType, BaseProtocol res)
        {
            string roomId = null;
            switch (msgType)
            {
                case MessageType.MatchingSuccess:
                    var proto = res as ProtoMatchingSuccess;
                    roomId = proto.RoomId;
                    break;
                case MessageType.MatchingCancelFail:
                    break;
                case MessageType.MatchingCancelSuccess:
                    break;
                case MessageType.MatchingFail:
                    break;
                default:
                    return;
            }
            _resultState = new MatchingResultState
            {
                receivedMessage = msgType,
                RoomId = roomId
            };
            _receiveEvent.Set();
        }

        public MatchingResultState WaitForReceiveResult()
        {
            _receiveEvent.WaitOne();
            return _resultState;
        }



        private void AsyncTcpSession_Closed(object sender, EventArgs e)
        {
            _connectionEvent.Set();

        }

        private void AsyncTcpSession_Connected(object sender, EventArgs e)
        {
            _connectionEvent.Set();
        }

    }
}
