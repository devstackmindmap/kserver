using AkaThreading;
using CommonProtocol;

namespace PubSubServer
{
    public static class S2CManager
    {
        public static void SendOne2OneMessage(MessageType messageType, NetworkSession networkSession, uint userId)
        {
            networkSession.Send(messageType,
                AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoOne2One>.
                Serialize(new CommonProtocol.PubSub.ProtoOne2One
                {
                    MessageType = messageType,
                    UserId = userId
                }));
        }

        public static void SendClanChattingMessage(MessageType messageType, NetworkSession networkSession, uint userId, string msg)
        {
            networkSession.Send(messageType,
                AkaSerializer.AkaSerializer<ProtoUserIdStringValue>.
                Serialize(new ProtoUserIdStringValue
                {
                    MessageType = messageType,
                    UserId = userId,
                    Value = msg
                }));
        }

        public static void SendUserIdTargetIdMessage(MessageType messageType, NetworkSession networkSession, uint userId, uint targetId)
        {
            networkSession.Send(messageType,
                AkaSerializer.AkaSerializer<ProtoUserIdTargetId>.
                Serialize(new ProtoUserIdTargetId
                {
                    MessageType = messageType,
                    UserId = userId,
                    TargetId = targetId
                }));
        }

        public static void SendWeb2OneMessage(MessageType messageType, NetworkSession networkSession, byte[] protoData)
        {
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.PubsubServer2ClientSocketBalancer))
            {
                networkSession.Send(messageType, protoData);
            }
        }
    }
}
