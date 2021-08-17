using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSystemMailInfo : BaseProtocol
    {
        [Key(1)]
        public List<ProtoSystemMailData> SystemMailDatas = new List<ProtoSystemMailData>();
    }
}
