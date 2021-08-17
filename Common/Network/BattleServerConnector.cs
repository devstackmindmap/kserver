using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public class BattleServerConnector
    {
        private bool _connect;
        private string _ip;
        private int _port;
        private int _reconnectTime;
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();
        private readonly string Name;

        private ManualResetEvent _connectionEvent = new ManualResetEvent(false);

        public event EventHandler<BaseProtocol> DataReceived;
        public event EventHandler Connected;
        public event EventHandler Closed;


        public BattleServerConnector()
        {
            Name = "Battle Server connector";
        }

        public void Initialize(string ip, int port, int tryReconnectTime)
        {
            _asyncTcpSession.Connected += AsyncTcpSession_Connected; ;
            _asyncTcpSession.DataReceived += AsyncTcpSession_DataReceived;
            _asyncTcpSession.Closed += AsyncTcpSession_Closed;
                
            _ip = ip;
            _port = port;
            _connect = true;
            _reconnectTime = tryReconnectTime;

        }

        private IPEndPoint GetEndPoint()
        {
            try
            {
                if (IPAddress.TryParse(_ip, out var ipAddress) == false)
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(_ip);
                    foreach (var address in hostEntry.AddressList)
                    {
                        return new IPEndPoint(address, _port);
                    }
                    return null;
                }
                else
                    return new IPEndPoint(ipAddress, _port);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void TryConnect()
        {
            if (_connect)
            {
                WaitConnected();
            }
        }

        public bool IsConnected()
        {
            return _asyncTcpSession.IsConnected;
        }


        public void Send(MessageType messageType, byte[] data)
        {
            Task.Factory.StartNew(() =>
            {
                if (IsConnected() == false)
                    return;

                var header = BitConverter.GetBytes((int)messageType);
                using (var stream = new MemoryStream(header.Length + sizeof(int) + data.Length))
                {
                    stream.Write(header, 0, header.Length);
                    stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                    stream.Write(data, 0, data.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    _asyncTcpSession.Send(stream.GetBuffer(), 0, (int)stream.Length);
                }
            });
        }

        private void AsyncTcpSession_DataReceived(object sender, DataEventArgs e)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            var intSize = sizeof(int);
            while (offset < e.Length)
            {
                try
                {
                    var header = e.Data.CloneRange(offset, sizeHeader);
                    var bodyLength = e.Data.CloneRange(offset + sizeHeader, intSize);
                    var realBodyLength = BitConverter.ToInt32(bodyLength, 0);
                    var body = e.Data.CloneRange(offset + intSize + sizeHeader, realBodyLength);

                    offset += (sizeHeader + intSize + realBodyLength);

                    var msgType = (MessageType)BitConverter.ToInt32(header, 0);

                    var proto = ProtocolFactory.DeserializeProtocol(msgType, body);
                    DataReceived?.Invoke(_ip, proto);
                }
                catch(Exception)
                {

                }
            }
        }

        public void Close()
        {
            try
            {
                _connect = false;
                _asyncTcpSession.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        private void AsyncTcpSession_Connected(object sender, EventArgs e)
        {
            //TODO check wait connect
            AkaLogger.Logger.Instance().Debug($"{Name} - {_ip}:{_port.ToString()} Connected");
            _connectionEvent.Set();
            Connected?.Invoke(_ip, e);
        }

        private void AsyncTcpSession_Closed(object sender, EventArgs e)
        {
            AkaLogger.Logger.Instance().Debug($"{Name} - {_ip}:{_port.ToString()} Closed");

            _connectionEvent.Reset();
            Closed?.Invoke(_ip, e);
        }

        private bool WaitConnected()
        {
            if (IsConnected() == false)
            {
                try
                {
                    var endPoint = GetEndPoint();
                    if (endPoint != null)
                        _asyncTcpSession.Connect(endPoint);
                }
                catch(Exception)
                {
                    return false;
                }
            }
            return _connectionEvent.WaitOne(_reconnectTime) && IsConnected();
        }

    }
}
