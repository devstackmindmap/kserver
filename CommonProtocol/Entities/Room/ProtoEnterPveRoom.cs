using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEnterPveRoom : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte DeckNum;

        [Key(3)]
        public uint StageRoundId;

        [Key(4)]
        public uint StageLevelId;

        [Key(5)]
        public AkaEnum.Battle.BattleType BattleType;

        [Key(6)]
        public string BattleServerIp;

    }
}
