using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PubSubServer
{
    public class FriendMessage : BaseController
    {
        private NetworkSession _session;
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as CommonProtocol.PubSub.ProtoOne2N;
            _session = session;

            switch (req.MessageType)
            {
                case MessageType.PubSubFriendAsked:
                    Publish(req.UserId, req.FriendId, Channel.FriendAsked); break;
                case MessageType.PubSubFriendlyBattleAccept:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleAccept); break;
                case MessageType.PubSubFriendlyBattleCancel:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleCancel); break;
                case MessageType.PubSubFriendlyBattleDecline:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleDecline); break;
                case MessageType.PubSubFriendlyBattleInvite:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleInvite); break;
                case MessageType.PubSubFriendlyBattleReady:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleReady); break;
                case MessageType.PubSubFriendlyBattleReadyCancel:
                    Publish(req.UserId, req.FriendId, Channel.FriendlyBattleReadyCancel); break;
                case MessageType.PubSubFriendSigned:
                    Publish(req.UserId, req.FriendId, Channel.FriendSigned);
                    _session.AddFriend(req.FriendId);
                    break;
                case MessageType.PubSubFriendRemoved:
                    Publish(req.UserId, req.FriendId, Channel.FriendRemove);
                    _session.RemoveFriend(req.FriendId);
                    break;
                default:
                    throw new System.Exception();
            }
        }

        private void Publish(uint userId, uint friendId, string channelHeader)
        {
            var subscriber = _session.GetSubscriber();
            subscriber.AkaPublish(ChannelName.Get(channelHeader, friendId), userId);
        }
    }
}
