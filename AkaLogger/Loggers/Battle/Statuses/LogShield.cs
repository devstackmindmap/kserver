//using AkaEnum.Battle;
//using System;

//namespace AkaLogger.Battle
//{
//    public sealed class LogShield
//    {

//        public void ShieldDamageLog(string roomId, PlayerType playerType, uint unitId, int shield, float damage)
//        {
//            Logger.Instance().Analytics("Damage", "Shield",
//                "RoomId", roomId,
//                "Player", playerType.ToString(),
//                "UnitId", unitId.ToString(),
//                "PrevShield", shield.ToString(),
//                "Damage", damage.ToString()
//                );
//        }

//        public void ShieldEndLog(string roomId, PlayerType playerType, uint unitId)
//        {
//            Logger.Instance().Analytics("ShieldEnd", "Shield",
//                "RoomId", roomId,
//                "Player", playerType.ToString(),
//                "UnitId", unitId.ToString()
//                );
//        }

//        public void AddShieldLog(string roomId, PlayerType playerType, uint unitId, int prevShield, int addShield)
//        {
//            Logger.Instance().Analytics("AddShield", "Shield",
//                "RoomId", roomId,
//                "Player", playerType.ToString(),
//                "UnitId", unitId.ToString(),
//                "PrevShield", prevShield.ToString(),
//                "AddShield", addShield.ToString()
//                );
//        }
//    }
//}
