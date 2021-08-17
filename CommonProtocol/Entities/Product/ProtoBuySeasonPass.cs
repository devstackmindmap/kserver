using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBuySeasonPass : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
        
        [Key(2)]
        public PlatformType PlatformType;

        [Key(3)]
        public SeasonPassType SeasonPassType;

        [Key(4)]
        public string PurchaseToken;

        [Key(5)]
        public string TransactionId;

        [Key(6)]
        public bool IsPending;

        [Key(7)]
        public uint SeasonPassSeason;
    }
}

