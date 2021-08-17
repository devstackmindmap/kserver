using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoModifyMemberGrade : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint TargetId;

        [Key(3)]
        public ClanMemberGrade ClanMemberGrade;
    }
}
