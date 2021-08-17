using AkaEnum.Battle;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaLogger;

namespace BattleLogic
{
    public class BattleController
    {
        private readonly Dictionary<PlayerType, BattleCard> _battleCards = new Dictionary<PlayerType, BattleCard>(PlayerTypeComparer.Comparer);

        public void BattleControllerInitialize(Deck player1Deck, Deck player2Deck)
        {
            _battleCards.Add(PlayerType.Player1, BattleCardFactory.CreateBattleCard(player1Deck));
            _battleCards.Add(PlayerType.Player2, BattleCardFactory.CreateBattleCard(player2Deck));

            foreach (var unit in player1Deck.Units)
            {
                //AkaLogger.Logger.Instance().Info($"[SetAction] UnitId:{unit.Value.UnitData.UnitIdentifier.UnitId}, PositionIndex:{unit.Value.UnitData.UnitIdentifier.UnitPositionIndex}" +
                //    $", PlayerType:{unit.Value.PlayerType}, Attack Speed:{unit.Value.UnitData.UnitStatus.AttackSpeed}");
             //   AkaLogger.Log.Battle.SetAction.Log(unit.Value.UnitData.UnitIdentifier.UnitId, unit.Value.UnitData.UnitIdentifier.UnitPositionIndex, unit.Value.PlayerType, unit.Value.UnitData.UnitStatus.AttackSpeed);
                SetAction(PlayerType.Player1, unit.Value.UnitData.UnitIdentifier.UnitPositionIndex, unit.Value.EnqueueSkill);
            }

            foreach (var unit in player2Deck.Units)
            {
                //AkaLogger.Logger.Instance().Info($"[SetAction] UnitId:{unit.Value.UnitData.UnitIdentifier.UnitId}, PositionIndex:{unit.Value.UnitData.UnitIdentifier.UnitPositionIndex}" +
                //    $", PlayerType:{unit.Value.PlayerType}, Attack Speed:{unit.Value.UnitData.UnitStatus.AttackSpeed}");
             //   AkaLogger.Log.Battle.SetAction.Log(unit.Value.UnitData.UnitIdentifier.UnitId, unit.Value.UnitData.UnitIdentifier.UnitPositionIndex, unit.Value.PlayerType, unit.Value.UnitData.UnitStatus.AttackSpeed);
                SetAction(PlayerType.Player2, unit.Value.UnitData.UnitIdentifier.UnitPositionIndex, unit.Value.EnqueueSkill);
            }
        }

        public Dictionary<PlayerType, BattleCard> GetBattleCards()
        {
            return _battleCards;
        }

        public CardUseActionData CardUse(ProtoCardUse protoCardUse)
        {
            return _battleCards[protoCardUse.Performer.PlayerType].CardUse(protoCardUse);
        }

        public Card GetHandCard(PlayerType playerType, int handIndex)
        {
            return _battleCards[playerType].GetHandCard(handIndex);
        }

        public void CardUseWithPattern(uint cardId, uint patternId, ProtoTarget performer, ProtoTarget target)
        {
            _battleCards[performer.PlayerType].CardUseWithPattern(cardId, patternId, performer, target);
        }

        public void RemoveUnit(PlayerType playerType, uint unitId)
        {
            _battleCards[playerType].RemoveCard(unitId);
        }

        private void SetAction(PlayerType playerType, int unitPositionIndex, Action<CardUseActionData> action)
        {
            _battleCards[playerType].SetAction(unitPositionIndex, action);
        }

        public Dictionary<int, Card> GetHandCards(PlayerType playerType)
        {
            return _battleCards[playerType].HandCards;
        }

        //public uint? GetNexCardStatId(PlayerType playerType)
        public uint? GetNexCardStatId(PlayerType playerType)
        {
            return _battleCards[playerType].NextHandCardStatId();
        }

        public Queue<Card> GetWaitCards(PlayerType playerType)
        {
            return _battleCards[playerType].WaitCards;
        }
    }
}
