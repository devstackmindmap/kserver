using AkaEnum.Battle;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class KillAllUnitEnd : IBattleEndCondition
    {
        public PlayerType FinalWinner { get; set; }

        public bool IsBattleEnd(Dictionary<PlayerType, Player> players, bool IsExtensionTime, Unit deathUnit)
        {
            var isBattleEnd = players.Values.Any(player => !player.Units.Any() );
            if (isBattleEnd)
                FinalWinner = GetWinner(players);

            return isBattleEnd;
        }
        
        public bool CorrectBattleExtension(Dictionary<PlayerType, Player> players)
        {
            return false;
        }

        private PlayerType GetWinner(Dictionary<PlayerType, Player> players)
        {
            if (players[PlayerType.Player2].Units.Count == 0) 
                return PlayerType.Player1;
            else
                return PlayerType.Player2;
        }
    }
}
