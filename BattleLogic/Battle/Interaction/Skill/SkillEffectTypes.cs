using AkaEnum;

namespace BattleLogic
{
    public class SkillEffectTypes
    {
        public readonly static SkillEffectType[] NormalAttackAtkByAttackers =
        {
            SkillEffectType.BUFF_STATE_BARISADA,    // 합연산, 항상 먼저 계산되어야함.
            SkillEffectType.BUFF_STATE_BLIND,
            SkillEffectType.BUFF_STATE_BERSERK,
            SkillEffectType.BUFF_STATE_MADNESS,
            SkillEffectType.BUFF_STATE_SEVEN_DEVIL,
            SkillEffectType.BUFF_STATE_EIGHT_DEVIL,
        };  // 공격자 일반 공격 공격력 상승

        public readonly static SkillEffectType[] NormalAttackDamageByAttackers =
        {
            SkillEffectType.BUFF_STATE_TARGET
        };  // 공격자 일반 공격 데미지 상승

        public readonly static SkillEffectType[] NextBuffAtkByAttackers =
        {
            SkillEffectType.BUFF_NEXT_ATTACK_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_WEAK_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_BLIND_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_BERSERK_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_POISON_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_ATTENTION_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_ATTACKSPEED_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_STEALTH_ATK_UP,
            SkillEffectType.BUFF_NEXT_TARGET_HP_UNDER_ATK_UP,
            SkillEffectType.BUFF_NEXT_ATTACK_ATK_UP_AND_ALL_TARGET,
            SkillEffectType.BUFF_NEXT_ATTACK_IGNITION_BOMB,
            SkillEffectType.BUFF_NEXT_TARGET_HOLD_STATE_COUNT,
            SkillEffectType.BUFF_NEXT_TARGET_ADD_BUFF_STATE,
            SkillEffectType.BUFF_NEXT_ATTACK_IGNORE_STEEL_COUNTER
        };  // 공격자 다음 일반 공격력 상승

        public readonly static SkillEffectType[] NextBuffCriRateByAttackers =
        {
            SkillEffectType.BUFF_NEXT_ATTACK_CRI_DMG
        };

        public readonly static SkillEffectType[] NextBuffSkillCriRateByAttackers =
        {
            SkillEffectType.BUFF_NEXT_SKILL_ATTACK_CRITICAL_RATE
        };

        public readonly static SkillEffectType[] NormalAttackByTargets =
        {
            SkillEffectType.BUFF_STATE_WEAK,
            SkillEffectType.BUFF_STATE_STEEL,
            SkillEffectType.BUFF_STATE_STEEL_IMMUNE_DISAPPEARANCE,
            SkillEffectType.BUFF_STATE_STEEL_ALL,
            SkillEffectType.BUFF_STATE_WEAK_NORMAL,
            SkillEffectType.BUFF_STATE_ACCUMULATE_NORMAL_ATTACK,
            SkillEffectType.BUFF_STATE_REDUCTION,
            SkillEffectType.BUFF_STATE_SEVEN_DEVIL,
            SkillEffectType.BUFF_STATE_EIGHT_DEVIL,
        };  // 피격자 받는 일반 데미지 상승

        public readonly static SkillEffectType[] NormalAttackByAttackersAtCriProbabilityUp = 
        {
            SkillEffectType.BUFF_NEXT_ATTACK_CRI_PROBABILITY_UP
        };

        public readonly static SkillEffectType[] SkillAttackByAttackers =
        {
            SkillEffectType.BUFF_STATE_BARISADA,    // 합연산, 항상 먼저 계산되어야함.
            SkillEffectType.BUFF_STATE_BLIND,
            SkillEffectType.BUFF_STATE_BERSERK,
            SkillEffectType.BUFF_STATE_MADNESS,
            SkillEffectType.BUFF_STATE_TARGET,
            SkillEffectType.BUFF_STATE_SEVEN_DEVIL,
            SkillEffectType.BUFF_STATE_EIGHT_DEVIL,
        };  // 공격자 스킬 공격력 상승

        public readonly static SkillEffectType[] SkillAttackByTargets =
        {
            SkillEffectType.BUFF_STATE_WEAK,
            SkillEffectType.BUFF_STATE_WEAK_SKILL,
            SkillEffectType.BUFF_STATE_ACCUMULATE_SKILL_ATTACK,
            SkillEffectType.BUFF_STATE_REDUCTION,
            SkillEffectType.BUFF_STATE_STEEL_ALL,
            SkillEffectType.BUFF_STATE_SEVEN_DEVIL,
            SkillEffectType.BUFF_STATE_EIGHT_DEVIL,
        };  // 피격자 받는 스킬 데미지 상승

        public readonly static SkillEffectGroupType[] SkillCounter =
        {
            SkillEffectGroupType.SpellDmg,
            SkillEffectGroupType.SpellFixingDmg
        };  // 스킬 공격 반격

        public readonly static SkillEffectType[] SkillDefence =
        {
            SkillEffectType.BUFF_STATE_DEFENCE_SKILL_ATTACK,
            SkillEffectType.BUFF_STATE_CURSER
        };  // 스킬 공격 방어

        public readonly static SkillEffectType[] CurserIgnore =
        {
            SkillEffectType.BUFF_STATE_SEVEN_DEVIL,
            SkillEffectType.BUFF_STATE_EIGHT_DEVIL
        }; // Curser 강화불가에 영향받지 않는 버프 스킬들
    }
}
