using CommonProtocol;
using System.Threading.Tasks;

namespace PubSubServer
{
    public class StatusMessage : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            var req = requestInfo as CommonProtocol.PubSub.ProtoOne2One;

            networkSession.State = req.MessageType;
            switch (req.MessageType)
            {
                case MessageType.PubSubMatchmaking: Publish(networkSession, req.UserId, Channel.Matchmaking); break;
                case MessageType.PubSubBattle: Publish(networkSession, req.UserId, Channel.Battle); break;
                case MessageType.PubSubOnline: Publish(networkSession, req.UserId, Channel.Online); break;
                case MessageType.PubSubLogout:
                    networkSession.Close();
                    break;
                default:
                    networkSession.State = MessageType.PubSubOnline;
                    throw new System.Exception();

            }
        }

        private void Publish(NetworkSession networkSession, uint userId, string channelHeader)
        {
            networkSession.StatusMessagePublish(channelHeader, userId);
        }
    }
}
