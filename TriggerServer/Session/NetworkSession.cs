using AkaSerializer;
using CommonProtocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.IO;

namespace TriggerServer
{
    public class NetworkSession : AppSession<NetworkSession, BinaryRequestInfo>
    {
        public void Send(MessageType messageType, byte[] data)
        {
            var header = BitConverter.GetBytes((int)messageType);
            StreamSend(header, data);
        }

        private void StreamSend(byte[] header, byte[] body)
        {
            using (var stream = new MemoryStream(header.Length + sizeof(int) + body.Length))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(BitConverter.GetBytes(body.Length), 0, sizeof(int));
                stream.Write(body, 0, body.Length);
                stream.Seek(0, SeekOrigin.Begin);
                //TrySend(stream.GetBuffer(), 0, (int)stream.Length);
                Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }
    }
}
