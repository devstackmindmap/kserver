using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBuyProductDigital : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public List<List<ProtoItemResult>> ItemResults;

        [Key(3)]
        public ProtoMaterialInfo MaterialInfo;
    }
}

