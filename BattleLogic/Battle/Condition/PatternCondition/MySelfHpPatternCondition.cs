
using AkaData;

namespace BattleLogic
{
    [PatternCondition(PatternType = AkaEnum.ActionPatternType.MyselfHp)]
    public class MySelfHpPatternCondition : SingleUsePatternCondition
    {
        public MySelfHpPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId, int maxValue, int currentValue)
        {
            if (Available() && IsMe(player,monsterId))
            {
                CurrentActionPatternValue = currentValue * 100 / maxValue;
            }
        }
    }


}
