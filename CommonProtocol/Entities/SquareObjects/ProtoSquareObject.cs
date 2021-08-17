
using MessagePack;
namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObject : BaseProtocol
    {
        [Key(1)]
        public int SquareObjectExp { get; set; }

        [Key(2)]
        public uint SquareObjectLevel { get; set; }

        [Key(3)]
        public int CoreExp { get; set; }

        [Key(4)]
        public uint CoreLevel { get; set; }

        [Key(5)]
        public int AgencyExp { get; set; }

        [Key(6)]
        public uint AgencyLevel { get; set; }

        [Key(7)]
        public ProtoSquareObjectState CurrentState { get; set; }

    }

}
