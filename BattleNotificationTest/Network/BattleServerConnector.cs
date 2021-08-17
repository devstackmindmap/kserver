using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class BattleServerConnector
    {
        private BattleNotificationForm _battleNotificationForm;
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();

        private BattleServerConnector()
        {

        }

        private static BattleServerConnector _instance = null;
        public static BattleServerConnector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BattleServerConnector();
                }
                return _instance;
            }
        }
        public void SetFrom(BattleNotificationForm form)
        {
            _battleNotificationForm = form;
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
            _asyncTcpSession.Close();
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

                var controller = ControllerFactory.CreateController(msgType);
                controller.DoPipeline(proto, _battleNotificationForm);
            }
        }

        private void AsyncTcpSession_Connected(object sender, EventArgs e)
        {
            if (_battleNotificationForm != null)
                _battleNotificationForm.Notice("배틀 서버 접속 성공");
        }

        private void AsyncTcpSession_Closed(object sender, EventArgs e)
        {
            if (_battleNotificationForm != null)
                _battleNotificationForm.Notice("배틀 서버 접속 해제");
        }

        public bool IsConnected()
        {
            return _asyncTcpSession.IsConnected;
        }
    }
}
