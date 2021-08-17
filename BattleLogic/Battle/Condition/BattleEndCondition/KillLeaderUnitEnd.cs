using AkaEnum.Battle;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class KillLeaderUnitEndCondition : IBattleEndCondition
    {
        public PlayerType FinalWinner { get; set; }

        public bool IsBattleEnd(Dictionary<PlayerType, Player> players, bool IsExtensionTime, Unit deathUnit)
        {
            players.Values.Any(player => !player.Units.Any());

            if (deathUnit.PlayerType == PlayerType.Player1 && false == players[PlayerType.Player1].Units.Any())
            {
                FinalWinner = PlayerType.Player2;
                return true;
            }
            else if (deathUnit.PlayerType == PlayerType.Player2 
                && deathUnit.UnitData.UnitIdentifier.MonsterType == AkaEnum.MonsterType.Boss)
            {
                FinalWinner = PlayerType.Player1;
                return true;
            }

            FinalWinner = PlayerType.None;
            return false;
        }
        
        public bool CorrectBattleExtension(Dictionary<PlayerType, Player> players)
        {
            return FinalWinner == PlayerType.None;
        }
    }
}
