using System;
using AkaEnum;
using AkaEnum.Battle;

namespace AkaLogger.Battle
{
    public sealed class LogDoAttack
    {
        public void Log(string roomId, PlayerType playerType, uint PerformerUnitId, uint TargetUnitId, int prevHP, int HP, int Damage, bool isCritical, int prevShield, int shield, bool isShieldIgnore, int poisonDamage)
        {
            Logger.Instance().Analytics("AttackResult", "NormalAttack",
                "RoomId", roomId,
                "Player", playerType.ToString(),
                "UnitId", PerformerUnitId.ToString(),
                "TargetUnitId", TargetUnitId.ToString(),
                "PrevHP", prevHP.ToString(),
                "HP", HP.ToString(),
                "Damage", Damage.ToString(),
                "PoisonDamage", poisonDamage.ToString(),
                "PrevShield", prevShield.ToString(),
                "Shield", shield.ToString(),
                "IsCritical",isCritical.ToString(),
                "IsShieldIgnore", isShieldIgnore.ToString());
        }


    }
}
