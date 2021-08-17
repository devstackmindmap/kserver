using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoNewInfusionBox
    {
        [Key(0)]
        public uint Id;

        [Key(1)]
        public int UseUserEnergy;

        [Key(2)]
        public int UseUserBonusEnergy;

        [Key(3)]
        public int NewTotalUserEnergy;

        [Key(4)]
        public int NewTotalUserBonusEnergy;

        [Key(5)]
        public  int NewTotalBoxEnergy;

        [Key(6)]
        public long NewUserEnergyRecentUpdateDatetime;
    }
}
