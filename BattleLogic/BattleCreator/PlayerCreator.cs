using AkaEnum.Battle;
using AkaUtility;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public class PlayerCreator
    {
        private PlayerInfo _playerInfo;

        public PlayerCreator(PlayerInfo playerInfo)
        {
            _playerInfo = playerInfo;
        }

        public Dictionary<PlayerType, Player> GetPlayers()
        {
            var players = new Dictionary<PlayerType, Player>(PlayerTypeComparer.Comparer)
            {
                { PlayerType.Player1, new Player(new PlayerIdentifier {Player = PlayerType.Player1, UserId = _playerInfo.Player1UserId, DeckNum = _playerInfo.Player1DeckNum})},
                { PlayerType.Player2, new Player(new PlayerIdentifier {Player = PlayerType.Player2, UserId = _playerInfo.Player2UserId, DeckNum = _playerInfo.Player2DeckNum})}
            };

            if (players.Count != 2)
                throw new Exception("Player Count Isn't 2");

            return players;
        }
    }
}
