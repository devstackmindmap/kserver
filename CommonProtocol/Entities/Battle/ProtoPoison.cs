using System.Collections.Generic;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoPoison : BaseProtocol
    {
        [Key(1)]
        public PlayerType PlayerType;

        [Key(2)]
        public uint UnitId;

        [Key(3)]
        public List<ProtoShieldInfo> Shields;

        [Key(4)]
        public int Damage;
    }
}