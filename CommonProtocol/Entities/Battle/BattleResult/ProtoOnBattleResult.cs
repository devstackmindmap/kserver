using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBattleResult : BaseProtocol
    {
        [Key(1)]
        public Dictionary<RewardCategoryType, List<ProtoItemResult>> ItemResults;

        [Key(2)]
        public BattleResultType BattleResultType;

        [Key(3)]
        public ProtoUserExp UserLevelAndExp;

        [Key(4)]
        public List<ProtoQuestInfo> QuestInfoList;

        [Key(5)]
        public uint CurrentSeason;
    }
}
