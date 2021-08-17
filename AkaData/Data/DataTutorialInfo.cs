using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataTutorialInfo
    {
        public uint TutorialId { get; set; }
        public uint TutorialStartConditionWho { get; set; }
        public int TutorialStartConditionValue { get; set; }
        public TutorialStartConditionType TutorialStartConditionType { get; set; }
        public OperationConditionType OperationConditionType { get; set; }
        public List<TutorialActionType> TutorialActionTypeList { get; set; }
        public List<uint> TutorialActionWhoList { get; set; }
        public List<uint> TutorialActionValueList { get; set; }
    }
}
