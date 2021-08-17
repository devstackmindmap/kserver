using AkaEnum;
using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataTutorialInfo : ClassMap<DataTutorialInfo>
    {
        public MappingDataTutorialInfo()
        {
            Map(m => m.TutorialId);
            Map(m => m.TutorialStartConditionType);
            Map(m => m.TutorialStartConditionWho);
            Map(m => m.OperationConditionType);
            Map(m => m.TutorialStartConditionValue);

            Map(m => m.TutorialActionTypeList).ConvertUsing(row =>
            {
                var data = row.GetField("TutorialActionTypeList");

                return data.CastToList<TutorialActionType>(actionName => (TutorialActionType)actionName.ToInt());
            });

            Map(m => m.TutorialActionWhoList).ConvertUsing(row =>
            {
                var data = row.GetField("TutorialActionWhoList");

                return data.CastToList<uint>(uint.Parse);
            });

            Map(m => m.TutorialActionValueList).ConvertUsing(row =>
            {
                var data = row.GetField("TutorialActionValueList");

                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
