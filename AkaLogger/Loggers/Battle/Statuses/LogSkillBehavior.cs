using AkaEnum.Battle;
using System;
using System.Collections.Generic;

namespace AkaLogger.Battle
{
    public sealed class LogSkillBehavior
    {
        public void Log(string roomId, PlayerType performerPlayerType, uint UnitId, uint TargetUnitId, int prevHP, int HP, int logDamage, uint cardId, uint skillId)
        {
            Logger.Instance().Analytics("DoSpell", "SkillBehavior",
                "RoomId", roomId,
                "Player", performerPlayerType.ToString(),
                "UnitId", UnitId.ToString(),
                "TargetUnitId", TargetUnitId.ToString(),
                "PrevHP", prevHP.ToString(),
                "Damage", logDamage.ToString(),
                "HP", HP.ToString(),
                "CardId", cardId.ToString(),
                "SkillId", skillId.ToString()
                );
        }

        //        public void Log(string roomId, PlayerType performerPlayerType, uint UnitId, uint cardId, uint CardUnitId)
        //        {
        //            Logger.Instance().Analytics("DoSkillError", "SkillBehavior",
        //                "RoomId", roomId,
        //                "Player", performerPlayerType.ToString(),
        //                "UnitId", UnitId.ToString(),
        //                "CardId", cardId.ToString(),
        //                "CardUnitId", CardUnitId.ToString());
        //        }


        //        public void Log(string roomId, PlayerType performerPlayerType, uint UnitId, uint CardStatId, uint skillId,
        //                        IEnumerable<uint> skillOptionIdList, IEnumerable<uint> targetIdList)
        //        {
        //            Logger.Instance().Analytics("DoSkillResult", "SkillBehavior",
        //                "RoomId", roomId,
        //                "Player", performerPlayerType.ToString(),
        //                "UnitId", UnitId.ToString(),
        //                "CardStatId", CardStatId.ToString(),
        //                "SkillId", skillId.ToString(),
        //                "SkillOptionIdList", string.Join(",", skillOptionIdList),
        //                "TargetIdList", string.Join(",", targetIdList)
        //                );                
        //        }


        //        public void SpellFailLog(string roomId, PlayerType performerPlayerType, uint UnitId, uint TargetUnitId, uint cardId, uint skillId)
        //        {
        //            Logger.Instance().Analytics("DoSpellFail", "SkillBehavior",
        //                "RoomId", roomId,
        //                "Player", performerPlayerType.ToString(),
        //                "UnitId", UnitId.ToString(),
        //                "TargetUnitId", TargetUnitId.ToString(),
        //                "CardId", cardId.ToString(),
        //                "SkillId", skillId.ToString()
        //                );
        //        }


        //        public void BuffFailLog(string roomId, PlayerType performerPlayerType, PlayerType targetPlayerType, uint UnitId, uint TargetUnitId, uint skillOptionId)
        //        {
        //            Logger.Instance().Analytics("DoBuffFail", "SkillBehavior",
        //                "RoomId", roomId,
        //                "Player", performerPlayerType.ToString(),
        //                "UnitId", UnitId.ToString(),
        //                "TargetPlayer", targetPlayerType.ToString(),
        //                "TargetUnitId", TargetUnitId.ToString(),
        //                "SkillOption", skillOptionId.ToString()
        //                );
        //        }

    }
}
