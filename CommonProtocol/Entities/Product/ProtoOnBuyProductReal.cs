using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBuyProductReal : BaseProtocol
    {
        [Key(1)]
        public List<ProtoOnBuyProductRealItem> Results = new List<ProtoOnBuyProductRealItem>(); 
    }

    [MessagePackObject]
    public class ProtoOnBuyProductRealItem
    {
        [Key(0)]
        public ResultType ResultType;

        [Key(1)]
        public List<List<ProtoItemResult>> ItemResults;

        [Key(2)]
        public uint ProductId;

        [Key(3)]
        public string StoreProductId;
    }
}

