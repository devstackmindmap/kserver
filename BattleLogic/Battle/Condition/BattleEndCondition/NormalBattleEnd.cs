using AkaEnum.Battle;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class NormalBattleEndCondition : IBattleEndCondition
    {
        public PlayerType FinalWinner { get; set; }

        public bool IsBattleEnd(Dictionary<PlayerType, Player> players, bool IsExtensionTime, Unit deathUnit)
        {
            var isBattleEnd = players.Values.Any(player => !player.Units.Any() );
            FinalWinner = GetWinner(players);
            return isBattleEnd || (IsExtensionTime && FinalWinner != PlayerType.None);
        }
        
        public bool CorrectBattleExtension(Dictionary<PlayerType, Player> players)
        {
            return GetWinner(players) == PlayerType.None;
        }

        private PlayerType GetWinner(Dictionary<PlayerType, Player> players)
        {
            if (players[PlayerType.Player1].Units.Count == players[PlayerType.Player2].Units.Count)
                return PlayerType.None;

            if (players[PlayerType.Player1].Units.Count > players[PlayerType.Player2].Units.Count)
                return PlayerType.Player1;
            else
                return PlayerType.Player2;            
        }
    }
}
