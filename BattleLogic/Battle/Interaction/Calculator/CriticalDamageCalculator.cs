namespace BattleLogic
{
    public static class CriticalDamageCalculator
    {
        public static float GetCriticalDamageAsNormalAttack(Unit performer, Unit target, bool isCritical, float originDamage, double firstTakeDamageTime)
        {
            if (isCritical == false)
                return originDamage;

            var rate = BuffStateCalculator.GetAtkWithNexts(performer.UnitData.UnitStatus.CriticalDamageRate, performer, 
                SkillEffectTypes.NextBuffCriRateByAttackers, target, firstTakeDamageTime);
            return originDamage * rate.attackerAtk;
        }

        public static int GetCriticalDamageAsSkillAttack(Unit performer, bool isCritical, int originDamage)
        {
            if (isCritical == false)
                return originDamage;

            return (int)(originDamage * performer.UnitData.UnitStatus.CriticalDamageRate);
        }
    }
}