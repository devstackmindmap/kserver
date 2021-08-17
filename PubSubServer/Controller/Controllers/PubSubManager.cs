using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace PubSubServer
{
    public class PubSubManager
    {
        public string GetChannelByMessageType(MessageType messageType)
        {
            var channel = Channel.AlreadyLogin;
            switch (messageType)
            {
                case MessageType.PubSubMatchmaking:
                    channel = Channel.Matchmaking;
                    break;
                case MessageType.PubSubBattle:
                    channel = Channel.Battle;
                    break;
                case MessageType.PubSubOnline:
                    channel = Channel.Online;
                    break;
            }
            return channel;
        }

        public void Subscribe(NetworkSession session)
        {
            var redisServer = session.GetRedisServer();
            if (redisServer != null)
            {
                redisServer.SubscriptionSubscriberCountAsync(ChannelName.Get(Channel.Login, session.UserId))
                    .ContinueWith(task =>
                    {
                        if (task.Result > 0 && session.State != MessageType.PubSubOnLogin)
                        {
                            session.Close();
                            return;
                        }
                        InternalSubscribe(session);
                    });
            }
            else
                session.Close();
        }

        private void InternalSubscribe(NetworkSession session)
        {
            var subscriber = session.GetSubscriber();


            subscriber.AkaSubscribe(MessageHandlerType.Common, session, 
                ChannelName.Get(Channel.Login, session.UserId), message =>
            {
                RedisChannel ch = message.Channel;
                RedisValue val = message.Message;

                var subsLoginInfo = val.ToString().Split('|');
                var friendId = uint.Parse(subsLoginInfo[0]);
                var state = (MessageType)Enum.Parse(typeof(MessageType), subsLoginInfo[1]);
                var relationType = (RelationType)Enum.Parse(typeof(RelationType), subsLoginInfo[2]);

                if (relationType == RelationType.Friend)
                {
                    session.AddFriend(friendId);
                }
                else if (relationType == RelationType.ClanMember)
                {
                    session.AddClanMember(friendId);
                }
                else if (relationType == RelationType.Both)
                {
                    session.AddFriend(friendId);
                    session.AddClanMember(friendId);
                }
                else if (relationType == RelationType.Self)
                {
                    if (session.SessionID != subsLoginInfo[3])
                    {
                        S2CManager.SendOne2OneMessage(MessageType.PubSubOnOtherLogin, session, friendId);
                        session.Close();
                    }
                    return;
                }

                S2CManager.SendOne2OneMessage(state, session, friendId);

                var manager1 = new PubSubManager();
                var channel = manager1.GetChannelByMessageType(session.State);
                subscriber.AkaPublish(ChannelName.Get(channel, friendId), session.UserId);
            });

            DefaultSubscribe(MessageType.PubSubOnline, Channel.Online, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnLogin, Channel.AlreadyLogin, subscriber, session);
            DefaultSubscribe(MessageType.PubSubMatchmaking, Channel.Matchmaking, subscriber, session);
            DefaultSubscribe(MessageType.PubSubBattle, Channel.Battle, subscriber, session);
            DefaultSubscribe(MessageType.PubSubLogout, Channel.LoginOut, subscriber, session);

            subscriber.AkaSubscribe(MessageHandlerType.Common, session, 
                ChannelName.Get(Channel.FriendSigned, session.UserId), message =>
            {
                RedisChannel ch = message.Channel;
                RedisValue val = message.Message;

                var friendId = uint.Parse(val);
                S2CManager.SendOne2OneMessage(MessageType.PubSubOnFriendSigned, session, friendId);
                session.AddFriend(friendId);
            });

            subscriber.AkaSubscribe(MessageHandlerType.Common, session,
                ChannelName.Get(Channel.FriendRemove, session.UserId), message =>
            {
                RedisChannel ch = message.Channel;
                RedisValue val = message.Message;

                var friendId = uint.Parse(val);
                session.RemoveFriend(friendId);
                S2CManager.SendOne2OneMessage(MessageType.PubSubOnFriendRemoved, session, friendId);
            });

            session.LoginPublish();

            DefaultSubscribe(MessageType.PubSubOnFriendAsked, Channel.FriendAsked, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleInvite, Channel.FriendlyBattleInvite, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleAccept, Channel.FriendlyBattleAccept, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleCancel, Channel.FriendlyBattleCancel, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleDecline, Channel.FriendlyBattleDecline, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleReady, Channel.FriendlyBattleReady, subscriber, session);
            DefaultSubscribe(MessageType.PubSubOnFriendlyBattleReadyCancel, Channel.FriendlyBattleReadyCancel, subscriber, session);

            Web2OneMessageSubscribe(MessageType.PubSubUpdateQuest, Channel.ServerQuestUpdate, subscriber, session);

            Manage2AllMessageSubscribeForNotice(MessageType.PubSubPublicNotice, Channel.PublicNotice, subscriber, session);
            Manage2OneMessageSubscribeForNotice(MessageType.PubSubPrivateNotice, Channel.PrivateNotice, subscriber, session);


            ClanSubscribe(subscriber, session);

        }

        private void DefaultSubscribe(MessageType messageType, string channelHeader, ISubscriber subscriber, NetworkSession session)
        {
            subscriber.AkaSubscribe(MessageHandlerType.Common, session, ChannelName.Get(channelHeader, session.UserId), message =>
            {
                S2CManager.SendOne2OneMessage(messageType, session, uint.Parse(message.Message));
            });
        }

        private void Web2OneMessageSubscribe(MessageType messageType, string channelHeader, ISubscriber subscriber, NetworkSession session)
        {
            subscriber.AkaSubscribe(MessageHandlerType.Common, session, ChannelName.Get(channelHeader, session.UserId), message =>
            {
                S2CManager.SendWeb2OneMessage(messageType, session, message.Message);
            });
        }

        private void Manage2AllMessageSubscribeForNotice(MessageType messageType, string channelHeader, ISubscriber subscriber, NetworkSession session)
        {
            subscriber.AkaSubscribe(MessageHandlerType.Common, session, Channels.Manage2AllMessage.Sub(channelHeader), message =>
            {
                var protoData = new CommonProtocol.PubSub.ProtoPubsubNotice
                {
                    NoticeMessage = message.Message
                };
                S2CManager.SendWeb2OneMessage(messageType, session,
                    AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoPubsubNotice>.Serialize(protoData));
            });
        }

        private void Manage2OneMessageSubscribeForNotice(MessageType messageType, string channelHeader, ISubscriber subscriber, NetworkSession session)
        {
            subscriber.AkaSubscribe(MessageHandlerType.Common, session, ChannelName.Get(channelHeader, session.UserId), message =>
            {
                var protoData = new CommonProtocol.PubSub.ProtoPubsubNotice
                {
                    NoticeMessage = message.Message
                };
                S2CManager.SendWeb2OneMessage(messageType, session,
                    AkaSerializer.AkaSerializer<CommonProtocol.PubSub.ProtoPubsubNotice>.Serialize(protoData));
            });
        }

        public void ClanSubscribe(ISubscriber subscriber, NetworkSession session)
        {
            if (session.ClanId == 0)
                return;

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanJoin, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var clanMemberId = uint.Parse(message.Message);
                    if (clanMemberId != session.UserId)
                    {
                        S2CManager.SendOne2OneMessage(MessageType.PubSubOnClanJoin, session, clanMemberId);

                        var manager = new PubSubManager();
                        var channel = manager.GetChannelByMessageType(session.State);
                        session.AddClanMember(clanMemberId);
                        subscriber.AkaPublish(ChannelName.Get(channel, clanMemberId), session.UserId);
                    }
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanOut, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var clanMemberId = uint.Parse(message.Message);
                    S2CManager.SendOne2OneMessage(MessageType.PubSubOnClanOut, session, clanMemberId);
                    session.RemoveClanMember(clanMemberId);
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanBanish, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var subsMessage = message.Message.ToString().Split('|');
                    var userId = uint.Parse(subsMessage[0]);
                    var targetId = uint.Parse(subsMessage[1]);
                    S2CManager.SendUserIdTargetIdMessage(MessageType.PubSubOnClanBanish, session, userId, targetId);
                    if (targetId == session.UserId)
                    {
                        session.RemoveClanMembers();
                        var manager = new PubSubManager();
                        manager.UnSubscribeClanMessage(session);
                    }
                    else
                    {
                        session.RemoveClanMember(targetId);
                    }
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanModifyMemberGradeUp, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var subsMessage = message.Message.ToString().Split('|');
                    var userId = uint.Parse(subsMessage[0]);
                    var targetId = uint.Parse(subsMessage[1]);
                    S2CManager.SendUserIdTargetIdMessage(MessageType.PubSubOnClanModifyMemberGradeUp, session, userId, targetId);
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanModifyMemberGradeDown, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var subsMessage = message.Message.ToString().Split('|');
                    var userId = uint.Parse(subsMessage[0]);
                    var targetId = uint.Parse(subsMessage[1]);
                    S2CManager.SendUserIdTargetIdMessage(MessageType.PubSubOnClanModifyMemberGradeDown, session, userId, targetId);
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanProfileModify, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var clanMemberId = uint.Parse(message.Message);
                    if (clanMemberId != session.UserId)
                    {
                        S2CManager.SendOne2OneMessage(MessageType.PubSubClanProfileModify, session, clanMemberId);
                    }
                }
            });

            subscriber.AkaSubscribe(MessageHandlerType.Clan, session, ChannelName.Get(Channel.ClanChattingMessage, session.ClanId), message =>
            {
                if (session.ClanId != 0)
                {
                    var subsMessage = message.Message.ToString().Split('|');
                    var userId = uint.Parse(subsMessage[0]);
                    var msg = subsMessage[1];
                    
                    S2CManager.SendClanChattingMessage(MessageType.PubSubOnClanChatting, session, userId, msg);
                }
            });
        }

        public void UnSubscribeCommonMessage(NetworkSession session)
        {
            foreach (var msgQ in session.ChannelHandlers)
                msgQ.Unsubscribe();

            session.ChannelHandlers.Clear();
        }

        public void UnSubscribeClanMessage(NetworkSession session)
        {
            foreach (var msgQ in session.ClanMessageHandlers)
                msgQ.Unsubscribe();

            session.ClanMessageHandlers.Clear();
        }
    }
}
