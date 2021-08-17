using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoPublicMailInfo : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoDbMailData> PublicMailDatas = new Dictionary<uint, ProtoDbMailData>();

        [Key(2)]
        public Dictionary<uint, List<ProtoReward>> Products;
    }
}
