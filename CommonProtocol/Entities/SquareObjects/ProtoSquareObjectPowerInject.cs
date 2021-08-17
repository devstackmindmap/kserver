
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObjectPowerInject : BaseProtocol
    {
        [Key(1)]
        public uint UserId { get; set; }

        [Key(2)]
        public int SquareObjectEnergy { get; set; }

        [Key(3)]
        public int AgencyEnergy { get; set; }

    }
}
