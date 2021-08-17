using AkaEnum;
using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleLogic
{
    public class BattleCreator
    {
        private PlayerInfo _playerInfo;
        private Func<AkaEnum.ModeType, Task<ProtoOnGetDeckWithDeckNum>> _getDeckInfo;
        private AdditionalBattleInfo _additionalBattleInfo;

        public BattleCreator(PlayerInfo playerInfo, Func<AkaEnum.ModeType, Task<ProtoOnGetDeckWithDeckNum>> getDeckInfo)
        {
            _playerInfo = playerInfo;
            _getDeckInfo = getDeckInfo;
        }

        public async Task<(Battle battle, ResultType resultType)> GetBattle(int battleNum, BattleType battleType,
            Action<PlayerType, bool> battleEnd, Action<BattleSendData> sessionSend,
            AdditionalBattleInfo additionalBattleInfo = null)
        {
            _additionalBattleInfo = additionalBattleInfo;

            var battle = BattleFactory.CreateBattle(battleType, battleNum, _playerInfo.StageRoundId);

            var (players, resultType) = await GetPlayer(battle);

            if (resultType != ResultType.Success)
                return (null, resultType);

            battle.BattleInitialize(players, battleEnd, sessionSend);

            return (battle, resultType);
        }

        private async Task<(Dictionary<PlayerType, Player> players, ResultType resultType)> GetPlayer(Battle battle)
        {
            var playerCreator = new PlayerCreator(_playerInfo);
            var players = playerCreator.GetPlayers();

            var resultType = await PlayerInitialize(players, battle);
            return (players, resultType);
        }

        public async Task<ResultType> PlayerInitialize(Dictionary<PlayerType, Player> players, Battle battle)
        {
            var deckModeType = _playerInfo.DeckModeType;
            var playerDeckInfos = await _getDeckInfo(deckModeType);
            battle.BattleHelper.BattleRecorder.SetPlayerDeckInfo(playerDeckInfos);

            var player1UserId = players[PlayerType.Player1].PlayerIdentifier.UserId;
            var player2UserId = players[PlayerType.Player2].PlayerIdentifier.UserId;

            var player1Nick = "";
            var player2Nick = "";

            var player1ProfileId = 0u;
            var player2ProfileId = 0u;

            Deck player1Deck = null;
            Deck player2Deck = null;

            foreach (var playerDeckInfo in playerDeckInfos.UserAndDecks)
            {
                var userId = playerDeckInfo.Key;
                var deckInfo = playerDeckInfo.Value;

                var deckCreator = new DeckCreator(userId, deckModeType, deckInfo.DeckNum, deckInfo);
                if (player1UserId == userId)
                {
                    player1Nick = deckInfo.Nickname;
                    player1ProfileId = deckInfo.ProfileIconId;
                    player1Deck = deckCreator.GetDeckByDeckInfo();

                    if (IsBanCard(player1Deck))
                        return ResultType.BanCard;
                }
                else
                {
                    player2Nick = deckInfo.Nickname;
                    player2ProfileId = deckInfo.ProfileIconId;
                    player2Deck = deckCreator.GetDeckByDeckInfo();
                }
            }

            if (false == battle.Enviroment.IsPvPBattle())
            {
                var deckCreator = new StageDeckCreator(_playerInfo.StageRoundId, PlayerType.Player2, PlayerType.Player1);
                var player2PatternContainer = deckCreator.GetPatternContainer();

                player2Nick = "AI";
                player2ProfileId = 0;
                player2Deck = deckCreator.GetDeck(player2PatternContainer);

                battle.BattleHelper.BattleBehaviorIntialize(player2PatternContainer);
            }

            players[PlayerType.Player1].PlayerInitialize(players[PlayerType.Player2], player1Deck.Units, battle, player1Nick, 
                player1ProfileId, null, _playerInfo.Player1TreasureIdList);

            players[PlayerType.Player2].PlayerInitialize(players[PlayerType.Player1], player2Deck.Units, battle, player2Nick, 
                player2ProfileId, _additionalBattleInfo?.AffixList, _playerInfo.Player2TreasureIdList);

            battle.BattleHelper.BattleControllerInitialize(player1Deck, player2Deck);

            return ResultType.Success;
        }

        private bool IsBanCard(Deck deck)
        {
            if (_additionalBattleInfo == null || _additionalBattleInfo.BanCardIdList == null)
                return false;

            foreach (var card in deck.Cards)
            {
                if (_additionalBattleInfo.BanCardIdList.Contains(card.CardId))
                    return true;
            }
            return false;
        }
    }
}
