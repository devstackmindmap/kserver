using CommonProtocol;
using Microsoft.Toolkit.Uwp.Helpers;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Network
{
    public class ClientNetworkConnector
    {
        private bool _connect;
        private string _ip;
        private int _port;
        private int _reconnectTime;
        private AsyncTcpSession _asyncTcpSession = new AsyncTcpSession();
        private ManualResetEvent _connectionEvent = new ManualResetEvent(false);
        private ManualResetEvent _closeEvent = new ManualResetEvent(true);

        public readonly string Name;

        public ClientNetworkConnector(string name, bool tryRecoonect = false)
        {
            Name = name;
            if (tryRecoonect)
                _reconnectTime = 3000;
        }

        public void Connect<TContext>(string ip, int port, Action<TContext,bool> connectedAction, Action<TContext, DataEventArgs> action, TContext context)
        {
            if (ip != null && !_asyncTcpSession.IsConnected)
            {
                _asyncTcpSession.Connected += delegate {
                    _connectionEvent.Set();
                    _closeEvent.Reset();
                    connectedAction(context, true);
                };
                _asyncTcpSession.Closed += delegate {
                    _connectionEvent.Reset();
                    _closeEvent.Set();
                    connectedAction(context, false);
                };
                _asyncTcpSession.DataReceived += delegate (object o, DataEventArgs args) {
                    action(context, args);
                };

                _ip = ip;
                _port = port;
                _connect = true;
                TryConnect();
            }
        }

        public bool IsConnected => _asyncTcpSession.IsConnected;

        public bool WaitConnect(int maxWaitMilSec = 3000)
        {
            return _connectionEvent.WaitOne(maxWaitMilSec) && IsConnected;
        }

        public bool WaitClose(int waittime)
        {
            return _closeEvent.WaitOne(waittime) && IsConnected == false;
        }

        private void TryConnect()
        {
            Task.Factory.StartNew(() =>
            {
                do
                {
                    Reconnect();                    
                } while (false == WaitConnect(1000));
            });
        }

        private void Reconnect()
        {
            try
            {
                if (IPAddress.TryParse(_ip, out var ipAddress) == false)
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(_ip);
                    ipAddress = hostEntry.AddressList[0];
                }
                _asyncTcpSession.Connect(new IPEndPoint(ipAddress, _port));
            }
            catch (Exception e)
            {

            }
        }

        public void Send(MessageType messageType, byte[] data)
        {
            if (IsConnected == false)
                return;

            var header = BitConverter.GetBytes((int)messageType);

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(header.Length + sizeof(int) + data.Length);
                stream.Write(header, 0, header.Length);
                stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int));
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                _asyncTcpSession.Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
            catch(Exception ex)
            {

            }
            finally
            {
                stream?.Dispose();
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

    }
}