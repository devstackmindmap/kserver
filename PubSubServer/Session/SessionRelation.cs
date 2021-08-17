using CommonProtocol;
using StackExchange.Redis;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace PubSubServer
{
    public class SessionRelation
    {
        public SortedSet<uint> FriendIds = new SortedSet<uint>();
        public SortedSet<uint> ClanMemberIds = new SortedSet<uint>();
        public SortedSet<uint> RelationBoth = new SortedSet<uint>();

        public void Init(List<uint> friendIds, List<uint> clanMemberIds)
        {
            FriendIds.UnionWith(friendIds);
            ClanMemberIds.UnionWith(clanMemberIds);
            RelationBoth.UnionWith(friendIds);
            RelationBoth.UnionWith(clanMemberIds);
        }

        public void AddFriend(uint friendId)
        {
            if (ClanMemberIds.Contains(friendId))
            {
                FriendIds.Add(friendId);
            }
            else
            {
                FriendIds.Add(friendId);
                RelationBoth.Add(friendId);
            }
        }

        public void AddClanMember(uint clanMemberId)
        {
            if (FriendIds.Contains(clanMemberId))
            {
                ClanMemberIds.Add(clanMemberId);
            }
            else
            {
                ClanMemberIds.Add(clanMemberId);
                RelationBoth.Add(clanMemberId);
            }
        }

        public void AddClanMembers(List<uint> clanMemberIds)
        {
            foreach (var clanMemberId in clanMemberIds)
            {
                if (false == FriendIds.Contains(clanMemberId))
                    RelationBoth.Add(clanMemberId);

                ClanMemberIds.Add(clanMemberId);
            }
        }

        public void RemoveFriend(uint friendId)
        {
            FriendIds.Remove(friendId);

            if (false == ClanMemberIds.Contains(friendId))
                RelationBoth.Remove(friendId);
        }

        public void RemoveClanMember(uint friendId)
        {
            ClanMemberIds.Remove(friendId);

            if (false == FriendIds.Contains(friendId))
                RelationBoth.Remove(friendId);
        }

        public void RemoveClanMembers()
        {
            foreach (var clanMemberId in ClanMemberIds)
            {
                if (false == FriendIds.Contains(clanMemberId))
                    RelationBoth.Remove(clanMemberId);
            }
            ClanMemberIds.Clear();
        }

        public SortedSet<uint> GetOnlyFriendIds()
        {
            SortedSet<uint> onlyFriendIds = new SortedSet<uint>();
            onlyFriendIds.UnionWith(FriendIds);
            onlyFriendIds.ExceptWith(ClanMemberIds);
            return onlyFriendIds;
        }

        public SortedSet<uint> GetOnlyClanMemberIds()
        {
            SortedSet<uint> onlyClanMemberIds = new SortedSet<uint>();
            onlyClanMemberIds.UnionWith(ClanMemberIds);
            onlyClanMemberIds.ExceptWith(FriendIds);
            return onlyClanMemberIds;
        }

        public SortedSet<uint> GetFriendClanMemberIntersectionIds()
        {
            SortedSet<uint> friendClanMemberIntersection = new SortedSet<uint>();
            friendClanMemberIntersection.UnionWith(FriendIds);
            friendClanMemberIntersection.IntersectWith(ClanMemberIds);
            return friendClanMemberIntersection;
        }
    }
}

