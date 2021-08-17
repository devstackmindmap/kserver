using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    public abstract class Passive : IPassive
    {
        private Unit _unit;
        private ProtoTarget _protoTarget = new ProtoTarget();
        private Card _card;
        private bool _isEnqueuePassive = false;

        protected int _passiveConditionCount = 0;
        protected int _passiveConditionLimitCount = 0;
        protected DataPassiveCondition _passive;

        public PassiveType PassiveType => _passive.PassiveType;

        public abstract bool IsConditionOk();
        public abstract bool IsConditionOk(float baseValue, float compareValue);
        public abstract bool IsConditionOk(float compareValue);


        public Passive(Unit unit, DataPassiveCondition passive)
        {
            _unit = unit;
            _passive = passive;
            Initialize();
        }

        private void Initialize()
        {
            var cardStat = Data.GetCardStat(_passive.CardStatId);
            InitializeProtoTarget(cardStat.TargetGroupType);
            InitializeCard();
        }

        private void InitializeProtoTarget(TargetGroupType targetGroupType)
        {
            if (targetGroupType == TargetGroupType.All)
                _protoTarget.PlayerType = AkaEnum.Battle.PlayerType.All;
            else if (targetGroupType == TargetGroupType.Ally)
                _protoTarget.PlayerType = _unit.PlayerType;
            else
                _protoTarget.PlayerType = _unit.PlayerType == AkaEnum.Battle.PlayerType.Player1 ? AkaEnum.Battle.PlayerType.Player2 : AkaEnum.Battle.PlayerType.Player1;
        }

        private void InitializeCard()
        {
            var dataCardStat = Data.GetCardStat(_passive.CardStatId);
            _card = new Card(_unit.UnitData.UnitIdentifier.UnitId, dataCardStat)
            {
                Action = PassiveEnd,
                CardRarity = CardRarity.NORMAL
            };
        }

        public bool PassiveConditionCheck(PassiveConditionType passiveConditionType)
        {
            if (passiveConditionType == _passive.PassiveConditionType
                && _isEnqueuePassive == false)
            {
                _passiveConditionCount++;

                if (IsConditionOk())
                {
                    _passiveConditionCount = 0;
                    _passiveConditionLimitCount++;
                    return true;
                }
            }
            return false;
        }

        public bool PassiveConditionCheck(PassiveConditionType passiveConditionType, int baseValue, int compareValue)
        {
            if (passiveConditionType == _passive.PassiveConditionType
                && _isEnqueuePassive == false)
            {
                _passiveConditionCount++;

                if (IsConditionOk(baseValue, compareValue))
                {
                    _passiveConditionCount = 0;
                    _passiveConditionLimitCount++;
                    return true;
                }
            }
            return false;
        }

        public bool PassiveConditionCheck(PassiveConditionType passiveConditionType, float compareValue)
        {
            if (passiveConditionType == _passive.PassiveConditionType
                && _isEnqueuePassive == false)
            {
                _passiveConditionCount++;

                if (IsConditionOk(compareValue))
                {
                    _passiveConditionCount = 0;
                    _passiveConditionLimitCount++;
                    return true;
                }
            }
            return false;
        }

        public void EnqueueSkill()
        {
            _isEnqueuePassive = true;
            _unit.EnqueuePassive(new CardUseActionData
            {
                UseCard = _card,
                Target = _protoTarget,
                ReplaceCardInfo = new ReplaceCardInfo
                {
                    NextCardStatId = null,
                    ReplacedCard = null,
                    ReplacedHandIndex = 0
                }
            });
        }

        public void DoSkill()
        {
            _unit.DoSkill(new CardUseActionData
            {
                ReplaceCardInfo = new ReplaceCardInfo
                {
                    NextCardStatId = null,
                    ReplacedCard = null,
                    ReplacedHandIndex = 0
                },
                Target = _protoTarget,
                UseCard = _card
            });
        }

        public bool IsPassiveExceedLimit()
        {
            return _passiveConditionLimitCount >= _passive.PassiveConditionLimitCount;
        }

        public void PassiveEnd()
        {
            _isEnqueuePassive = false;
        }
    }
}
