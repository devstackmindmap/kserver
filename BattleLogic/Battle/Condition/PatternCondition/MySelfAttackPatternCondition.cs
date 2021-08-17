
using AkaData;

namespace BattleLogic
{
    [PatternCondition(PatternType = AkaEnum.ActionPatternType.PerMyselfHitCount)]
    public class MySelfHitPatternCondition : MonsterPatternCondition
    {
        public MySelfHitPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId)
        {
            if (IsMe(player,monsterId))
            {
                CurrentActionPatternValue++;
            }
        }
    }


}
