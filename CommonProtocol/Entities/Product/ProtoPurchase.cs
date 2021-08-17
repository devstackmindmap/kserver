using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoPurchase
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public int CountOfPurchases;
    }
}

