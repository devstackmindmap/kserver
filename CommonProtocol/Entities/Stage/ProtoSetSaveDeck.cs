using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetSaveDeck : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public BattleType BattleType;

        [Key(3)]
        public uint NewCardStatId;

        [Key(4)]
        public uint OldCardStatId;

        [Key(5)]
        public uint NewTreasureId;
    }
}
