using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataQuest
    {
        public uint QuestGroupId { get; set; }
        public uint QuestId { get; set; }
        public uint Order { get; set; }
        public bool QuestKeeping { get; set; }
        public QuestType QuestType { get; set; }
        public QuestProcessType QuestProcessType { get; set; }
        public QuestTargetType QuestTargetType { get; set; }
        public List<SkillEffectType> SkillEffectTypeList { get; set; }
        public uint ClassId { get; set; }
        public int QuestConditionValue { get; set; }
        public uint RewardId { get; set; }
        public uint MailRewardId { get; set; }
    }
}