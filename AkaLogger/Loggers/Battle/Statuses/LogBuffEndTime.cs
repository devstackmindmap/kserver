//using System;
//using AkaEnum;

//namespace AkaLogger.Battle
//{
//    public sealed class LogBuffEndTime
//    {
//        public void Log(string roomId, uint unitId, SkillEffectType skillEffectType, int addMilsec, DateTime prevEndDateTime, DateTime endDateTime ,string TimeType)
//        {
//            Logger.Instance().Analytics("BuffAddEndTime", "BuffEndTime",
//                "RoomId", roomId,
//                "UnitId", unitId.ToString(),
//                "SkillEffectType", skillEffectType.ToString(),
//                "AddTime", addMilsec.ToString(),
//                "PrevEndTime", prevEndDateTime,
//                "EndTime", endDateTime,
//                "BuffEndTimeType",TimeType);
//        }
//    }
//}
