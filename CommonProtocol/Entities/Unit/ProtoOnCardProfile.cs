using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnCardProfile : BaseProtocol
    {
        [Key(1)]
        public List<ProtoCardProfile> CardProfiles = new List<ProtoCardProfile>();
    }


    [MessagePackObject]
    public class ProtoCardProfile
    {
        [Key(0)]
        public uint CardId;

        [Key(1)]
        public uint Level;
    }
}
