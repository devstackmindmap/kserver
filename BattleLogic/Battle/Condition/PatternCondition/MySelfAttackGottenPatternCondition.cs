using AkaData;
using AkaUtility;
using AkaEnum;

namespace BattleLogic
{
    [PatternCondition(PatternType = AkaEnum.ActionPatternType.PerMyselfGottenHitCount)]
    public class MySelfGottenHitPatternCondition : MonsterPatternCondition
    {
        public MySelfGottenHitPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }

        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId, DamageReasonType reasonType )
        {
            if (IsMe(player, monsterId) && reasonType.In(DamageReasonType.NormalAttack, DamageReasonType.NormalCounterAttack) )
            {
                CurrentActionPatternValue++;
            }
        }
    }
}
