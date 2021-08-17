using AkaUtility;
using CsvHelper.Configuration;
using System.Collections.Generic;

namespace AkaData
{
    public class MappingDataAnimationEvent : ClassMap<DataAnimationEvent>
    {
        public MappingDataAnimationEvent()
        {
            Map(m => m.AnimationEventId);
            Map(m => m.Length);

            Map(m => m.AttackTimingList).ConvertUsing(row =>
            {
                var attackTimingList = new List<int>();
                var typeTexts = row.GetField<string>("Type");
                var typeList = typeTexts.CastToArray<string>(ele => (string)ele);

                var timingTexts = row.GetField<string>("Timing");
                var timingList = timingTexts.CastToArray<int>(int.Parse);

                for (int i =0; i < typeList.Length; i++)
                {
                    if(typeList[i] == "TAKE_DAMAGE")
                    {
                        attackTimingList.Add(timingList[i]);
                    }
                }

                return attackTimingList;
            });
        }
    }
}
