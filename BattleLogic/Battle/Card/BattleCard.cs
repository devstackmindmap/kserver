using AkaLogger;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class BattleCard
    {
        private readonly Dictionary<int, Action<CardUseActionData>> _actions = new Dictionary<int, Action<CardUseActionData>>();

        private Queue<Card> _waitCards;
        private readonly int _maxHandCard;
        public Queue<Card> WaitCards => _waitCards;
        private Dictionary<int, Card> _handCards = new Dictionary<int, Card>();
        public Dictionary<int, Card> HandCards => _handCards;

        public List<Card> AllCards = new List<Card>();

        public BattleCard(Queue<Card> cards)
        {
            SaveAllCardIdForLog(cards);
            _maxHandCard = (int)AkaData.Data.GetConstant(AkaEnum.DataConstantType.MAX_CARD_HAND).Value;
            _waitCards = cards;
            SetStartHandCard();
        }

        private void SaveAllCardIdForLog(Queue<Card> cards)
        {
            AllCards.AddRange(cards);
        }


        public void SetAction(int unitPositionIndex, Action<CardUseActionData> action)
        {
            if (_actions.ContainsKey(unitPositionIndex))
                _actions[unitPositionIndex] = action;
            else
                _actions.Add(unitPositionIndex, action);
        }

        public virtual CardUseActionData CardUse(ProtoCardUse protoCardUse)
        {
            ResetDecreaseElixir(protoCardUse.HandIndex);
            var actionData = new CardUseActionData();
            HandToWaitCardMove(protoCardUse.HandIndex, actionData);

            //actionData.ReplaceCardInfo.NextCardStatId = NextHandCard();
            actionData.ReplaceCardInfo.NextCardStatId = NextHandCardStatId();
            //actionData.NextCardStatId = NextHandCardStatId();
            actionData.Target = protoCardUse.Target;

            return actionData;
        }

        private void ResetDecreaseElixir(int handIndex)
        {
            var card = GetHandCard(handIndex);
            if (card == null)
                return;

            card.DecreaseElixir = 0;
        }

        public virtual void CardUseWithPattern(uint cardStatId, uint patternId, ProtoTarget performer, ProtoTarget target)
        {

        }

        public void RemoveCard(uint unitId)
        {
            List<int> removeKeys = GetRemoveKeysOnHand(unitId);

            RemoveCardOnHandByKeys(removeKeys);
            RemoveCardOnWait(unitId);

            foreach (var key in removeKeys)
            {
                WaitToHandCardMove(key);
            }
        }

        protected bool ValidateActions()
        {
            return _actions != null;
        }

        public void EnqueueSkill(int unitPosition, CardUseActionData actionData)
        {
            DoAction(unitPosition, actionData);
        }

        protected void DoAction(int unitPosition, CardUseActionData actionData)
        {
            _actions[unitPosition](actionData);
        }

        private List<int> GetRemoveKeysOnHand(uint unitId)
        {
            List<int> removeKeys = new List<int>();
            foreach (var card in _handCards)
            {
                if (card.Value.UnitId == unitId)
                {
                    card.Value.IsDeath = true;
                    removeKeys.Add(card.Key);
                }
            }
            return removeKeys;
        }

        private void RemoveCardOnHandByKeys(List<int> removeKeys)
        {
            foreach (var key in removeKeys)
            {
                _handCards.Remove(key);
            }
        }

        private void RemoveCardOnWait(uint unitId)
        {
            foreach (var card in _waitCards)
            {
                if (card.UnitId == unitId)
                {
                    card.IsDeath = true;
                }
            }
        }

        private void SetStartHandCard()
        {
            for (int i = 0; i < _maxHandCard && _waitCards.Any(); ++i)
            {
                _handCards.Add(i, _waitCards.Dequeue());
            //    Log.Battle.StartHandCard.Log(i, _handCards[i].CardId);
            }
        }

        private void HandToWaitCardMove(int handIndex, CardUseActionData actionData)
        {
            if (IsValidateCard(handIndex) == false)
                return;

            var isDisappear = _handCards[handIndex].DataCard.IsDisappear;

            var card = _handCards[handIndex];
            if (isDisappear == false)
                _waitCards.Enqueue(card);
            _handCards.Remove(handIndex);

            //var replacedCardStatId = WaitToHandCardMove(index);
            var replacedCard = WaitToHandCardMove(handIndex);

            actionData.UseCard = card;
            //actionData ReplacedCardStatId = replacedCardStatId;
            actionData.ReplaceCardInfo.ReplacedCard = replacedCard;
            actionData.ReplaceCardInfo.ReplacedHandIndex = handIndex;
        }

        private bool IsValidateCard(int index)
        {
            if (_handCards.ContainsKey(index))
                return true;

            //AkaLogger.Logger.Instance().Info("Card doesn't exist Index:{0}", index);
            //AkaLogger.Log.Battle.CardUse.Log(index);
            return false;
        }

        private bool IsDisappear(int index)
        {
            return _handCards[index].DataCard.IsDisappear;
        }

        private Card WaitToHandCardMove(int indexOfHandBlank)
        {
            if (_waitCards.Count <= 0)
                return null;

            var card = _waitCards.Dequeue();

            while (card.IsDeath)
            {
                if (_waitCards.Count <= 0)
                    return null;

                card = _waitCards.Dequeue();
            }

            _handCards.Add(indexOfHandBlank, card);
            return card;
        }

        public uint? NextHandCardStatId()
        {
            if (_waitCards.Count <= 0)
                return null;

            var card = _waitCards.Peek();

            while (card.IsDeath)
            {
                _waitCards.Dequeue();

                if (_waitCards.Count <= 0)
                    return null;

                card = _waitCards.Peek();
            }
            return card.CardStatId;
        }

        public Card GetHandCard(int handIndex)
        {
            return _handCards.SafeGet(handIndex);
        }
    }
}
