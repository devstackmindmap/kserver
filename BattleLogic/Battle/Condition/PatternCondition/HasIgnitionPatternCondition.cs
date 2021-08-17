using AkaData;

namespace BattleLogic
{
    [PatternCondition(PatternType = AkaEnum.ActionPatternType.HasIgnition)]
    public sealed class HasIgnitionPatternCondition : MonsterPatternCondition
    {
        public HasIgnitionPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }

        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId, int stack)
        {
            if (IsMyTeam(player) == false)
            {
                CurrentActionPatternValue = stack;
                DidActionPatternValue = 0;
            }
        }
    }
}