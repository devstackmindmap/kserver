using AkaData;
using AkaEnum;
using AkaLogger;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE, SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE)]
    public sealed class BuffSkillAttackSpeedRate : BuffSkill
    {
        private float _value2;
        private Unit _target;

        public override float Value => _value2;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _target = target;
            _value2 = option.Value2;

            var newAttackSpeed = GetNewAttackSpeed(option.SkillEffectType);

            _target.UnitBuffs.AddTimer(SkillEffectType, _maintainMilliSeconds, EnqueueBuffEnd);
            _target.AttackTimer.ChangeInterval(newAttackSpeed, _target.UnitActionStatus);
            _target.BattleHelper.EnqueueUpdateUnitAttackTime(new BattleActionUpdateUnitAttackTime(_target, newAttackSpeed * 0.001f));

            Logger.Instance().Debug("[BNUM:{0} [Unit] [[ON Buff] [ID:{1}] [BuffSkillAttackSpeed  Value: {2} EndTime: {3}:{4}]",
                _target.MyPlayer.Battle.GetBattleNum(), _target.UnitData.UnitIdentifier.UnitId, newAttackSpeed, _endDateTime, _endDateTime.Millisecond);
        }

        private float GetNewAttackSpeed(SkillEffectType effectType)
        {
            var unitAttackSpeed = _target.UnitData.UnitStatus.AttackSpeed * 1000;

            switch (effectType)
            {
                case SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE:
                    var slowAttackSpeed = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE);
                    if (slowAttackSpeed == null)
                        return unitAttackSpeed * _value2;
                    break;
                case SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE:
                    var attackSpeed = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE);
                    if (attackSpeed == null)
                        return unitAttackSpeed * _value2;
                    break;
            }

            return unitAttackSpeed;
        }

        private void EnqueueBuffEnd()
        {
            Logger.Instance().Debug("[EnqueueBuffEnd]");
            _target.EnqueueBuffEnd(this);
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillAttackSpeedRate();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        public override void BuffEnd()
        {
            switch (SkillEffectType)
            {
                case SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE:
                    var slowAttackSpeed = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE);
                    if (slowAttackSpeed == null)
                        RecoveryAttackSpeed(_target.UnitData.UnitStatus.AttackSpeed);
                    else
                        RecoveryAttackSpeed(_target.UnitData.UnitStatus.AttackSpeed * slowAttackSpeed.Value);
                    break;
                case SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE:
                    var attackSpeed = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE);
                    if (attackSpeed == null)
                        RecoveryAttackSpeed(_target.UnitData.UnitStatus.AttackSpeed);
                    else
                        RecoveryAttackSpeed(_target.UnitData.UnitStatus.AttackSpeed * attackSpeed.Value);
                    break;
            }

            Logger.Instance().Debug("[BNUM:{0} [Unit] [[OFF Buff] [ID:{1}] [BuffSkillAttackSpeed  Value: {2} EndTime: {3}:{4}]"
                , _target.MyPlayer.Battle.GetBattleNum(), _target.UnitData.UnitIdentifier.UnitId, _value2, _endDateTime, _endDateTime.Millisecond);
        }

        private void RecoveryAttackSpeed(float attackSpeed)
        {
            _target.AttackTimer.RecoveryInterval(_target.UnitActionStatus);
            _target.BattleHelper.EnqueueUpdateUnitAttackTime(new BattleActionUpdateUnitAttackTime(_target, attackSpeed));
        }
    }
}