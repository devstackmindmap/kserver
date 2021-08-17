
using MessagePack;
using System;
namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObjectInvasionHistory
    {
        [Key(0)]
        public uint MonsterId { get; set; }

        [Key(1)]
        public uint InvasionLevel { get; set; }

        [Key(2)]
        public DateTime InvasionTime { get; set; }

        [Key(3)]
        public int RemainedShield { get; set; }

        [Key(4)]
        public int Power { get; set; }

        [Key(5)]
        public int MonsterAtk { get; set; }

        [Key(6)]
        public int PreviousShield { get; set; }

        [Key(7)]
        public uint GettingCoreExp { get; set; }

        [Key(8)]
        public uint GettingAgencyExp { get; set; }

        [Key(9)]
        public int MonsterCount { get; set; }
    }
}
