using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetStageLevelRoomInfo : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint StageLevelId;

        [Key(3)]
        public BattleType BattleType;
    }
}
