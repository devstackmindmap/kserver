using AkaRedisLogic;
using AkaSerializer;
using CommonProtocol;
using PubSubServer.Channels;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace PubSubServer
{
    public class ClanMessage : BaseController
    {
        ISubscriber _subscriber;
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            _subscriber = session.GetSubscriber();
            
            switch (requestInfo.MessageType)
            {
                case MessageType.PubSubClanJoin:
                    ClanJoin(session, requestInfo);
                    break;
                case MessageType.PubSubClanOut:
                    ClanOut(session, requestInfo);
                    break;
                case MessageType.PubSubClanBanish:
                    Publish(requestInfo as ProtoUserIdTargetIdClanId, Channel.ClanBanish);
                    break;
                case MessageType.PubSubClanModifyMemberGradeUp:
                    Publish(requestInfo as ProtoUserIdTargetIdClanId, Channel.ClanModifyMemberGradeUp);
                    break;
                case MessageType.PubSubClanModifyMemberGradeDown:
                    Publish(requestInfo as ProtoUserIdTargetIdClanId, Channel.ClanModifyMemberGradeDown);
                    break;
                case MessageType.PubSubClanProfileModify:
                    Publish(requestInfo as CommonProtocol.PubSub.ProtoClanId, Channel.ClanProfileModify);
                    break;
                case MessageType.PubSubClanChatting:
                    Publish(requestInfo as CommonProtocol.PubSub.ProtoClanChatting, Channel.ClanChattingMessage);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void ClanJoin(NetworkSession session, BaseProtocol requestInfo)
        {;
            var req = requestInfo as CommonProtocol.PubSub.ProtoClanJoin;
            session.ClanJoin(req.ClanId, req.ClanMemberIds);
            Publish(req, Channel.ClanJoin);
        }

        private void ClanOut(NetworkSession session, BaseProtocol requestInfo)
        {
            var manager = new PubSubManager();
            session.ClanOut();
            Publish(requestInfo as ProtoUserIdTargetId, Channel.ClanOut);
            manager.UnSubscribeClanMessage(session);
        }

        private void Publish(CommonProtocol.PubSub.ProtoClanJoin req, string channelName)
        {
            _subscriber.AkaPublish(ChannelName.Get(channelName, req.ClanId), req.UserId);
        }

        private void Publish(ProtoUserIdTargetId req, string channelName)
        {
            _subscriber.AkaPublish(ChannelName.Get(channelName, req.TargetId), req.UserId);
        }

        private void Publish(ProtoUserIdTargetIdClanId req, string channelName)
        {
            _subscriber.AkaPublish(ChannelName.Get(channelName, req.ClanId), GetMessage(req));
        }

        private void Publish(CommonProtocol.PubSub.ProtoClanId req, string channelName)
        {
            _subscriber.AkaPublish(ChannelName.Get(channelName, req.ClanId), req.ClanId);
        }

        private void Publish(CommonProtocol.PubSub.ProtoClanChatting req, string channelName)
        {
            _subscriber.AkaPublish(ChannelName.Get(channelName, req.ClanId), GetMessage(req));
        }

        private string GetMessage(ProtoUserIdTargetIdClanId req)
        {
            return $"{req.UserId}|{req.TargetId}";
        }

        private string GetMessage(CommonProtocol.PubSub.ProtoClanChatting req)
        {
            return $"{req.UserId}|{req.Message}";
        }
    }
}
