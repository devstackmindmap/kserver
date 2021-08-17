using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;

namespace TestClient
{
    public class Connector
    {
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();
        
        public Connector()
        {

        }

        private static Connector _instance = null;
        public static Connector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Connector();
                }
                return _instance;
            }
        }

        public void Connect(string ip, int port)
        {
            if (!_asyncTcpSession.IsConnected)
            {
                _asyncTcpSession.Connected += AsyncTcpSession_Connected; ;
                _asyncTcpSession.DataReceived += AsyncTcpSession_DataReceived;
                _asyncTcpSession.Closed += AsyncTcpSession_Closed;
                _asyncTcpSession.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
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

                if(msgType == MessageType.PubSubOnLogin)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 접속 했습니다.");
                }
                else if (msgType == MessageType.PubSubLogout)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 게임을 종료했습니다.");
                }

                else if (msgType == MessageType.PubSubMatchmaking)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 매치 메이킹 중 입니다.");
                }

                else if (msgType == MessageType.PubSubBattle)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 전투 중 입니다.");
                }

                else if (msgType == MessageType.PubSubOnline)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 온라인 입니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleInvite)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전 초대하였습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleAccept)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전 초대 수락하였습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleDecline)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전 초대 거절하였습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleCancel)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전이 취소되었습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleReady)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전 준비완료 되었습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendlyBattleReadyCancel)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"친구 {res.UserId} 가 친선전 준비해제 되었습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendAsked)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"유저 {res.UserId} 가 친구 신청하였습니다.");
                }
                else if (msgType == MessageType.PubSubOnFriendSigned)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"유저 {res.UserId} 와 친구가 되었습니다.");
                }

                else if (msgType == MessageType.PubSubOnFriendRemoved)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"유저 {res.UserId} 와 친구가 해제 되었습니다");
                }
                else if (msgType == MessageType.PubSubPublicNotice)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"공지");
                }
                else if (msgType == MessageType.PubSubClanJoin)
                {
                    var res = proto as CommonProtocol.PubSub.ProtoOne2One;

                    Console.WriteLine($"유저 {res.UserId} 가 클랜에 가입했습니다.");
                }

            }
        }

        private void AsyncTcpSession_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("connected");
        }

        private void AsyncTcpSession_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("closed");
        }

        public bool IsConnected()
        {
            return _asyncTcpSession.IsConnected;
        }


    }
}
