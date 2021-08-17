using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoInfusionBox : BaseProtocol
    {
        [Key(1)]
        public uint Id;

        [Key(2)]
        public int BoxEnergy;

        [Key(3)]
        public int UserEnergy;

        [Key(4)]
        public int UserBonusEnergy;

        [Key(5)]
        public long UserEnergyRecentUpdateDatetime;
    }
}
