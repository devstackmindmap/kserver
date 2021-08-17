using AkaSerializer;
using System;
using System.IO;

namespace CommonProtocol
{
    public static class ProtocolFactory
    {
        // Client To WebServer
        public static BaseProtocol DeserializeProtocol(MessageType messageType, Stream stream)
        {
            return Client2Web.DeserializeProtocol(messageType, stream);
        }

        // WebServer To Client
        public static byte[] SerializeProtocol(MessageType messageType, BaseProtocol protocol)
        {
            return Web2Client.SerializeProtocol(messageType, protocol);
        }

        public static BaseProtocol DeserializeProtocol(MessageType messageType, byte[] bytes)
        {
            return Tcp2Client.DeserializeProtocol(messageType, bytes);
        }
    }
}
