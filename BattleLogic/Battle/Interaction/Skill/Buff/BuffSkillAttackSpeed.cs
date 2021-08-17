using AkaData;
using AkaEnum;
using AkaLogger;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_ATTACKSPEED, SkillEffectType.BUFF_STATE_DIVISION_CELL)]
    class BuffSkillAttackSpeed : BuffSkill
    {
        private float _newAttackSpeed;
        private Unit _target;

        public override float Value => _newAttackSpeed;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _newAttackSpeed = option.Value2;
            _target = target;

            _target.UnitBuffs.AddTimer(SkillEffectType, _maintainMilliSeconds, EnqueueBuffEnd);
            
            _target.AttackTimer.ChangeInterval(_newAttackSpeed * 1000, _target.UnitActionStatus);
            _target.BattleHelper.EnqueueUpdateUnitAttackTime(new BattleActionUpdateUnitAttackTime(_target, _newAttackSpeed));

            Logger.Instance().Debug("[BNUM:{0} [Unit] [[ON Buff] [ID:{1}] [BuffSkillAttackSpeed  Value: {2} EndTime: {3}:{4}]",
                _target.MyPlayer.Battle.GetBattleNum(), _target.UnitData.UnitIdentifier.UnitId, _newAttackSpeed, _endDateTime, _endDateTime.Millisecond);
        }

        public override void BuffEnd()
        {
            switch (SkillEffectType)
            {
                case SkillEffectType.BUFF_STATE_DIVISION_CELL:
                    RecoveryDivisionCell();
                    break;
                default:
                    var divisionCell = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_DIVISION_CELL);
                    if (divisionCell == null)
                        RecoveryAttackSpeed();
                    break;
            }

            Logger.Instance().Debug("[BNUM:{0} [Unit] [[OFF Buff] [ID:{1}] [BuffSkillAttackSpeed  Value: {2} EndTime: {3}:{4}]"
                , _target.MyPlayer.Battle.GetBattleNum(), _target.UnitData.UnitIdentifier.UnitId, _newAttackSpeed, _endDateTime, _endDateTime.Millisecond);
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillAttackSpeed();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        private void EnqueueBuffEnd()
        {
            Logger.Instance().Debug("[EnqueueBuffEnd]");
            _target.EnqueueBuffEnd(this);
        }

        private void RecoveryDivisionCell()
        {
            var attackSpeedCondition = _target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_ATTACKSPEED);
            if (attackSpeedCondition == null)
            {
                RecoveryAttackSpeed();
                return;
            }

            _target.AttackTimer.ChangeInterval(attackSpeedCondition.Value * 1000, _target.UnitActionStatus);
            _target.BattleHelper.EnqueueUpdateUnitAttackTime(new BattleActionUpdateUnitAttackTime(_target, attackSpeedCondition.Value));
        }

        private void RecoveryAttackSpeed()
        {
            _target.AttackTimer.RecoveryInterval(_target.UnitActionStatus);
            _target.BattleHelper.EnqueueUpdateUnitAttackTime(new BattleActionUpdateUnitAttackTime(_target, _target.UnitData.UnitStatus.AttackSpeed));
        }
    }
}
