using AkaData;
using AkaEnum.Battle;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class BattleRecorder : IBattleRecorder
    {
        private Battle Battle { get; set; }
        private ConcurrentQueue<ProtoBattleRecordBehavior> _behaviors = new ConcurrentQueue<ProtoBattleRecordBehavior>();
        private long _lastBehaviorTick = DateTime.UtcNow.Ticks;
        private Dictionary<PlayerType, ProtoBeforeBattleStart> _protoBattleStarts;
        private Dictionary<PlayerType, ProtoOnBattleResult> _protoBattleResults;
        private ProtoOnGetDeckWithDeckNum _protoDeckForBattle;

        public BattleRecorder()
        {
            _protoBattleResults = new Dictionary<PlayerType, ProtoOnBattleResult>(PlayerTypeComparer.Comparer);
            _behaviors.Enqueue(new ProtoBattleRecordBehavior
                {
                    PlayerType = PlayerType.All,
                    BehaviorType = RecordBehaviorType.State,
                    Ticks = 0
                }
            );
        }

        public void SetBattle(Battle battle)
        {
            Battle = battle;
        }

        public void BattleResultRecord(BattleSendData battleSendData)
        {
            var battleResult = AkaSerializer.AkaSerializer<ProtoOnBattleResult>.Deserialize(battleSendData.Data);
            _protoBattleResults.Add(battleSendData.PlayerType, battleResult);
            EnqueueBehaviorForS2CRecord(battleSendData);
        }

        public void EnqueueBehaviorForS2CRecord(BattleSendData sendData)
        {
            long refreshTick = DateTime.UtcNow.Ticks;
            long elapsedTick = refreshTick - _lastBehaviorTick;
            _lastBehaviorTick = refreshTick;

            _behaviors.Enqueue(new ProtoBattleRecordBehavior
            {
                PlayerType = sendData.PlayerType,
                BehaviorType = RecordBehaviorType.Server2Client,
                Ticks = elapsedTick,
                Behavior = sendData.Data
            }
            );
        }

        public void C2SRecord()
        {
        }

        public void StateRecord()
        {
        }

        private ProtoBattleRecordPlayer GetPlayerInfo(Player player, PlayerType enemyPlayer)
        {
            List<uint> cardStatIdList ;
           
            if (_protoDeckForBattle != null && true == _protoDeckForBattle.UserAndDecks.TryGetValue(player.PlayerIdentifier.UserId, out var deckInfo) )
            {
                cardStatIdList = deckInfo.Deck.DeckElements.Where(deckElement => deckElement.SlotType == AkaEnum.SlotType.Card)
                                            .OrderBy(cardElement => cardElement.OrderNum)
                                            .Select(cardElement =>
                                            {
                                                var dataCard = Data.GetCard(cardElement.ClassId);
                                                var dataCardStat = Data.GetCardStat(dataCard.CardId, deckInfo.CardsLevel[cardElement.OrderNum]);
                                                return dataCardStat.CardStatId;
                                            })
                                            .ToList();
            }
            else
            {
                cardStatIdList = new List<uint>();
            }
            
            var playerInfo = new ProtoBattleRecordPlayer
            {
                Score = player.Units.Count,
                UserId = player.PlayerIdentifier.UserId,
                NickName = player.PlayerIdentifier.Nickname,
                Units = _protoBattleStarts[enemyPlayer].EnemyPlayer.Units,
                CardStatIds = cardStatIdList
            };

            foreach (var unit in player.DeathUnits)
            {
                var index = unit.Value.UnitData.UnitIdentifier.UnitPositionIndex;
                if (index < playerInfo.Units.Count)
                    playerInfo.Units[index].IsDeath = true;
            }

            return playerInfo;
        }

        public ProtoBattleRecord ToBattleRecord(PlayerType winPlayer)
        {
            var player1 = Battle.Players[PlayerType.Player1];
            var player2 = Battle.Players[PlayerType.Player2];

            var playerInfos = new Dictionary<PlayerType, ProtoBattleRecordPlayer>(PlayerTypeComparer.Comparer)
            {
                {PlayerType.Player1, GetPlayerInfo(player1, PlayerType.Player2) },
                {PlayerType.Player2, GetPlayerInfo(player2, PlayerType.Player1) }
            };

            var battleRecordInfo = new ProtoBattleRecordInfo
            {
                Winner = winPlayer,
                BattleStartInfo = playerInfos
            };

            return new ProtoBattleRecord
            {
                BattleType = Battle.Enviroment.BattleType,
                UserId = player1.PlayerIdentifier.UserId,
                EnemyUserId = player2.PlayerIdentifier.UserId,
                BattleInfo = battleRecordInfo,
                Behaviors = new List<ProtoBattleRecordBehavior>(_behaviors),
                BattleStartTime = Battle.Status.StartDateTime.Ticks,
                BattleEndTime = Battle.Status.EndDateTime.Ticks,
                MessageType = MessageType.SaveBattleRecordInfo,
                IsHost = true
            };
        }

        public void SetBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            _protoBattleStarts = protoBattleStarts;
        }
        public void SetPlayerDeckInfo(ProtoOnGetDeckWithDeckNum playerDeckInfo)
        {
            _protoDeckForBattle = playerDeckInfo;
        }
    }
}
