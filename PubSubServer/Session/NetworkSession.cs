using AkaUtility;
using CommonProtocol;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace PubSubServer
{
    public enum MessageHandlerType
    {
        Common,
        Clan
    }

    public class NetworkSession : AppSession<NetworkSession, BinaryRequestInfo>
    {
        public List<ChannelMessageQueue> ChannelHandlers = new List<ChannelMessageQueue>();
        public List<ChannelMessageQueue> ClanMessageHandlers = new List<ChannelMessageQueue>();
        public SessionRelation _relation = new SessionRelation();

        ISubscriber _subscriber;
        IServer _redisServer;
        public uint UserId;
        public uint ClanId;

        public MessageType State { get; set; }

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
                Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        public void SessionOut()
        {
            foreach (var friendId in _relation.RelationBoth)
            {
                GetSubscriber().AkaPublish(ChannelName.Get(Channel.LoginOut, friendId), UserId);
            }

            var manager = new PubSubManager();
            manager.UnSubscribeCommonMessage(this);
            manager.UnSubscribeClanMessage(this);
        }

        public ISubscriber GetSubscriber()
        {
            if (_subscriber == null)
            {
                _subscriber = AkaRedis.AkaRedis.GetSubscriber();
                return _subscriber;
            }

            return _subscriber;
        }

        public IServer GetRedisServer()
        {
            if (_redisServer == null)
            {
                _redisServer = AkaRedis.AkaRedis.GetServer();
                return _redisServer;
            }

            return _redisServer;            
        }

        public void Init(uint userId, uint clanId, MessageType stateMessageType, List<uint> friendIds, List<uint> clanMemberIds)
        {
            UserId = userId;
            ClanId = clanId;
            StatInit(stateMessageType);
            RelationInit(friendIds, clanMemberIds);
        }

        private void StatInit(MessageType stateMessageType)
        {
            State = stateMessageType.In(MessageType.PubSubBattle, MessageType.PubSubOnline, MessageType.PubSubMatchmaking) ?
                                   stateMessageType : MessageType.PubSubOnLogin;
        }

        private void RelationInit(List<uint> friendIds, List<uint>clanMemberIds)
        {
            _relation.Init(friendIds, clanMemberIds);
        }

        public void LoginPublish()
        {
            //networkSession.FriendIds.ExceptWith(networkSession.ClanMemberIds);
            var onlyFriendIds = _relation.GetOnlyFriendIds();
            var onlyClanMemberIds = _relation.GetOnlyClanMemberIds();
            var friendClanMemberIntersectionIds = _relation.GetFriendClanMemberIntersectionIds();

            string loginInfo = UserId.ToString() + "|" + State.ToString() + "|";
            string friendMessage = loginInfo + RelationType.Friend.ToString();
            string clanMemberMessage = loginInfo + RelationType.ClanMember.ToString();
            string friendClanMemberMessage = loginInfo + RelationType.Both.ToString();
            string selfMessage = loginInfo + RelationType.Self.ToString() + "|" + SessionID;

            var subscriber = GetSubscriber();
            foreach (var friendId in onlyFriendIds)
            {
                subscriber.AkaPublish(ChannelName.Get(Channel.Login, friendId), friendMessage);
            }

            foreach (var friendId in onlyClanMemberIds)
            {
                subscriber.AkaPublish(ChannelName.Get(Channel.Login, friendId), clanMemberMessage);
            }

            foreach (var friendId in friendClanMemberIntersectionIds)
            {
                subscriber.AkaPublish(ChannelName.Get(Channel.Login, friendId), friendClanMemberMessage);
            }

            subscriber.AkaPublish(ChannelName.Get(Channel.Login, UserId), selfMessage);
        }

        public void StatusMessagePublish(string channelHeader, uint userId)
        {
            foreach (var friendId in _relation.RelationBoth)
            {
                GetSubscriber().AkaPublish(ChannelName.Get(channelHeader, friendId), userId);
            }
        }

        public void ClanJoin(uint clanId, List<uint> clanMemberIds)
        {
            ClanId = clanId;
            _relation.AddClanMembers(clanMemberIds);

            PubSubManager manager = new PubSubManager();
            manager.ClanSubscribe(GetSubscriber(), this);
            var channel = manager.GetChannelByMessageType(State);

            foreach (var friendId in _relation.ClanMemberIds)
                GetSubscriber().AkaPublish(ChannelName.Get(channel, friendId), UserId);
        }

        public void ClanOut()
        {
            ClanId = 0;
            _relation.RemoveClanMembers();
        }

        public void AddFriend(uint friendId)
        {
            _relation.AddFriend(friendId);
        }

        public void AddClanMember(uint clanMemberId)
        {
            _relation.AddClanMember(clanMemberId);
        }

        public void RemoveFriend(uint friendId)
        {
            _relation.RemoveFriend(friendId);
        }

        public void RemoveClanMember(uint friendId)
        {
            _relation.RemoveClanMember(friendId);
        }

        public void RemoveClanMembers()
        {
            _relation.RemoveClanMembers();
        }
    }
}

