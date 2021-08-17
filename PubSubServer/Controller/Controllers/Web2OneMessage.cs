using CommonProtocol;
using CommonProtocol.PubSub;
using System.Threading.Tasks;

namespace PubSubServer
{
    public class Web2OneMessage : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoWeb2One;

            switch(req.MessageType)
            {
                case MessageType.PubSubUpdateQuest:
                    Publish(req as ProtoWeb2OneUpdateQuest, Channel.ServerQuestUpdate);
                    break;
                default:
                    throw new System.Exception();

            }
        }

        private void Publish<TProtoRequst>(TProtoRequst req, string channelHeader) where TProtoRequst : ProtoWeb2One
        {
            if (req != null)
            {
                var subscriber = AkaRedis.AkaRedis.GetSubscriber();
                byte[] protoData = AkaSerializer.AkaSerializer<TProtoRequst>.Serialize(req);
                subscriber.AkaPublish(ChannelName.Get(channelHeader, req.UserId), protoData);
            }
        }

    }
}
