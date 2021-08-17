using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBuyProductReal : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<ProtoStoreInfo> StoreInfos = new List<ProtoStoreInfo>();
    }
}

