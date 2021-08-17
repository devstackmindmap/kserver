using AkaEnum.Battle;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class KillOneUnitEndCondition : IBattleEndCondition
    {
        public PlayerType FinalWinner { get; set; }

        public bool IsBattleEnd(Dictionary<PlayerType, Player> players, bool IsExtensionTime, Unit deathUnit)
        {
            FinalWinner = GetWinner(players);
            return FinalWinner != PlayerType.None;
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
