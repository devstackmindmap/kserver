using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailInfo : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoDbMailData> PublicMailDatas = new Dictionary<uint, ProtoDbMailData>();

        [Key(2)]
        public List<ProtoDbMailData> PrivateMailDatas = new List<ProtoDbMailData>();

        [Key(3)]
        public List<ProtoSystemMailData> SystemMailDatas = new List<ProtoSystemMailData>();

        [Key(4)]
        public Dictionary<uint, List<ProtoReward>> Products;
    }
}
