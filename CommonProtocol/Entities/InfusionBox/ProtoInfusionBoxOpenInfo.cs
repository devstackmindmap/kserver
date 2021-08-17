using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoInfusionBoxOpenInfo
    {
        [Key(0)]
        public List<ProtoItemResult> ItemResults;

        [Key(1)]
        public ProtoNewInfusionBox NewInfusionBox;
    }
}
