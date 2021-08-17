using AkaEnum.Battle;
using System.Collections.Generic;

namespace BattleLogic
{
    public interface IBattleEndCondition
    {
        bool IsBattleEnd(Dictionary<PlayerType, Player> players, bool IsExtensionTime, Unit deathUnit);

        bool CorrectBattleExtension(Dictionary<PlayerType, Player> players);

        PlayerType FinalWinner { get; set; }
    }
}
