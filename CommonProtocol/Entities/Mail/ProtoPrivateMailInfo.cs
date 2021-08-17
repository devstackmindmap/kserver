using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoPrivateMailInfo : BaseProtocol
    {
        [Key(1)]
        public List<ProtoDbMailData> PrivateMailDatas = new List<ProtoDbMailData>();

        [Key(2)]
        public Dictionary<uint, List<ProtoReward>> Products;
    }
}
