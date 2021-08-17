
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObjectState : BaseProtocol
    {
        [Key(1)]
        public uint UserId { get; set; }

        [Key(2)]
        public bool IsActivated { get; set; }

        [Key(3)]
        public uint NextInvasionLevel { get; set; }

        [Key(4)]
        public DateTime ActivatedTime { get; set; }

        [Key(5)]
        public DateTime NextInvasionTime { get; set; }

        [Key(6)]
        public uint NextInvasionMonsterId { get; set; }
        
        [Key(7)]
        public uint SquareObjectLevel { get; set; }

        [Key(8)]
        public int CurrentPlanetBoxExp { get; set; }

        [Key(9)]
        public uint CurrentPlanetBoxLevel { get; set; }

        [Key(10)]
        public int CurrentShield { get; set; }

        [Key(11)]
        public int SquareObjectPower { get; set; }

        [Key(12)]
        public DateTime PowerRefreshTime { get; set; }

        [Key(13)]
        public int CoreEnergy { get; set; }

        [Key(14)]
        public int ExtraCoreEnergy { get; set; }

        [Key(15)]
        public DateTime EnergyRefreshTime { get; set; }

        [Key(16)]
        public DateTime ExtraEnergyInjectedTime { get; set; }
        
        [Key(17)]
        public bool EnableReward { get; set; }

        [Key(18)]
        public List<ProtoSquareObjectInvasionHistory> InvasionHistory { get; set; }

    }

}
