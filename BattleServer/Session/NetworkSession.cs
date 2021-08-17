using CommonProtocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;

namespace BattleServer
{
    public class NetworkSession : AppSession<NetworkSession, BinaryRequestInfo>
    {
        public uint UserId;
        public void Send(MessageType messageType, byte[] data)
        {
            var header = BitConverter.GetBytes((int)messageType);
            StreamSend(header, data);
        }

        private void StreamSend(byte[] header, byte[] body)
        {
            using (var stream = new MemoryStream(header.Length + sizeof(int) + body.Length))
            {
                try
                {
                    stream.Write(header, 0, header.Length);
                    stream.Write(BitConverter.GetBytes(body.Length), 0, sizeof(int));
                    stream.Write(body, 0, body.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    //TrySend(stream.GetBuffer(), 0, (int)stream.Length);
                    Send(stream.GetBuffer(), 0, (int)stream.Length);
                }
                catch (Exception ex)
                {
                    AkaLogger.Log.Debug.Exception("BattleServer.StreamSend", ex);
                    AkaLogger.Logger.Instance().Error(UserId.ToString() + ex.ToString());
                }
            }
        }
    }
}
