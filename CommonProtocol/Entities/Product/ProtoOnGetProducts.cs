using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetProducts : BaseProtocol
    {
        [Key(1)]
        public List<ProtoProductEventDigital> ProductEventDigitals = new List<ProtoProductEventDigital>();

        [Key(2)]
        public List<ProtoProductEventReal> ProductEventReals = new List<ProtoProductEventReal>();

        [Key(3)]
        public List<ProtoProductFixDigital> ProductFixDigitals = new List<ProtoProductFixDigital>();

        [Key(4)]
        public List<ProtoProductFixReal> ProductFixReals = new List<ProtoProductFixReal>();

        [Key(5)]
        public List<ProtoProductEventDigital> ProductUserDigitals = new List<ProtoProductEventDigital>();

        [Key(6)]
        public List<ProtoProductEventReal> ProductUserReals = new List<ProtoProductEventReal>();

        [Key(7)]
        public List<ProtoPurchase> Purchases = new List<ProtoPurchase>();

        [Key(8)]
        public  Dictionary<uint , List<ProtoReward>> Products;
    }
}

