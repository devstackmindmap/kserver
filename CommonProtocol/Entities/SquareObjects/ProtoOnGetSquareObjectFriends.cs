
using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetSquareObjectFriends : BaseProtocol
    {
        [Key(1)]
        public List<ProtoFriendInfo> NeedHelpFriends;

        [Key(2)]
        public List<ProtoFriendInfo> DonatedFriends;

        [Key(3)]
        public List<uint> HelpedFriends;

        [Key(4)]
        public List<uint> EnableHelpFriendsForDonated;
    }
}
