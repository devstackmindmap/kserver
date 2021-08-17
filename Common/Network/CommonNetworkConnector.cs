using CommonProtocol;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public class CommonNetworkConnector
    {
        private bool _connect;
        private string _ip;
        private int _port;
        private int _reconnectTime;
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();
        private readonly string Name;

        private ManualResetEvent _connectionEvent = new ManualResetEvent(false);

        public CommonNetworkConnector(string name)
        {
            Name = name;
        }

        private void InitSession()
        {
            _asyncTcpSession?.Close();
            _asyncTcpSession = new AsyncTcpSession();
            _asyncTcpSession.Connected += AsyncTcpSession_Connected; ;
            _asyncTcpSession.Closed += AsyncTcpSession_Closed;
            _asyncTcpSession.Error += AsyncTcpSession_Error;
        }

        public void Connect(string ip, int port, int tryReconnectTime)
        {
            if (!IsConnected())
            {
                InitSession();

                _ip = ip;
                _port = port;
                _connect = true;
                _reconnectTime = tryReconnectTime;

                TryConnect();
            }
        }

        private void AsyncTcpSession_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            AkaLogger.Logger.Instance().Debug($"{Name} - Error {e.ToString()}");
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

        private void TryConnect()
        {
            Task.Factory.StartNew(async () =>
            {
                while (_connect && IsConnected() == false)
                {
                    Reconnect();
                    await Task.Delay(_reconnectTime);
                }
            });
        }


        public bool IsConnected()
        {
            return _asyncTcpSession.IsConnected;
        }

        private void Reconnect()
        {
            var endPoint = GetEndPoint();

            try
            {
                if (endPoint != null)
                    _asyncTcpSession.Connect(endPoint);

            }
            catch (Exception ex)
            {
                AkaLogger.Logger.Instance().Error($"{Name} - Reconnect{ex.ToString()}");
                AkaLogger.Log.Debug.Exception("Reconnect-"+Name, ex);
                InitSession();
            }
        }


        public void Send(MessageType messageType, byte[] data)
        {
            Task.Factory.StartNew(() =>
            {
                if (WaitConnected() == false)
                {
                    AkaLogger.Logger.Instance().Debug($"{Name} - Send Failed ");
                    return;
                }

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
        }

        private void AsyncTcpSession_Closed(object sender, EventArgs e)
        {
            AkaLogger.Logger.Instance().Debug($"{Name} - {_ip}:{_port.ToString()} Closed");
            _connectionEvent.Reset();
            TryConnect();
        }

        private bool WaitConnected()
        {
            return _connectionEvent.WaitOne(_reconnectTime * 2) && IsConnected();
        }

    }
}
