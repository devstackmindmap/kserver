using AkaEnum.Battle;
using CommonProtocol;
using System;
using AkaEnum;

namespace BattleLogic
{
    public class BattleActionCardUse : BattleAction
    {
        private ProtoCardUse _cardUseData;
        private Card _card;
        private Battle _battle;

        public BattleActionCardUse(Battle battle, Card card, ProtoCardUse cardUseData) : base(null)
        {
            _cardUseData = cardUseData;
            _card = card;
            _battle = battle;
        }

        public override BattleActionResult DoAction()
        {
            var battleResult = new BattleActionResult() { IsDoing = false };

            //카드 한번더 검증 
            var card = _battle.GetBattleController().GetHandCard(_cardUseData.Performer.PlayerType, _cardUseData.HandIndex);
            if (_battle.IsBattleEnd || (card?.Equals(_card) ?? false) == false)
            {
                S2CManager.SendCardUseResult(_battle, _cardUseData, AkaEnum.ElixirCountState.NotEnough);
                return battleResult;
            }

            if (IsSilence())
            {
                S2CManager.SendCardUseResult(_battle, _cardUseData, AkaEnum.ElixirCountState.NotEnough);
                return battleResult;
            }

            if (IsEnoughHp() == false)
            {
                S2CManager.SendCardUseResult(_battle, _cardUseData, AkaEnum.ElixirCountState.NotEnoughHp);
                return battleResult;
            }

            var elixirValue = _battle.Players[_cardUseData.Performer.PlayerType].GetElixirCountState(card);

            var logValidateCardResult = ValidateCardResultType.Fail;
            S2CManager.SendCardUseResult(_battle, _cardUseData, elixirValue.ElixirCountState);


            if (elixirValue.ElixirCountState == AkaEnum.ElixirCountState.Enough
                || elixirValue.ElixirCountState == AkaEnum.ElixirCountState.CardUseReservation)
            {
                var cardUseActionData = _battle.GetBattleController().CardUse(_cardUseData);
                var performerUnit = _battle.Players[_cardUseData.Performer.PlayerType].Units[_cardUseData.Performer.UnitPositionIndex];
                
                if (IsValidateEnqueueSkill(performerUnit, cardUseActionData))
                {
                    UseElixir(_cardUseData, elixirValue.NeedElixir);
                    UseHp(cardUseActionData);
                    if (elixirValue.ElixirCountState == AkaEnum.ElixirCountState.Enough)
                        EnqueueSkill(_cardUseData, cardUseActionData);
                    else if (elixirValue.ElixirCountState == AkaEnum.ElixirCountState.CardUseReservation)
                        EnqueueSkillReservation(_cardUseData, cardUseActionData);

                    logValidateCardResult = ValidateCardResultType.Success;

                    IncreaseCardUseStatus(card, _cardUseData.Performer.PlayerType);
                    performerUnit.ActionLog.IncreaseStatus(ActionStatusType.UnitActionUseCard);
                }
                else
                {
                    S2CManager.SendProtoValidateCard(performerUnit, cardUseActionData, ValidateCardResultType.Pass);
                    logValidateCardResult = ValidateCardResultType.Pass;
                }

            }


            var userId = _cardUseData.Performer.PlayerType == PlayerType.Player1 ? _battle.Players[PlayerType.Player1].PlayerIdentifier.UserId : _battle.Players[PlayerType.Player2].PlayerIdentifier.UserId;
            AkaLogger.Log.Battle.CardUse.Log(_cardUseData.RoomId,
                                             userId,
                                             (byte)_battle.Enviroment.BattleType,
                                             _cardUseData.HandIndex,
                                             _cardUseData.Performer.PlayerType,
                                             _cardUseData.Performer.UnitPositionIndex,
                                             _cardUseData.Target.PlayerType,
                                             _cardUseData.Target.UnitPositionIndex,
                                             card.UnitId,
                                             card.DataCardStat.CardStatId,
                                             elixirValue.ElixirCountState.ToString(),
                                             elixirValue.CurrentElixir,
                                             elixirValue.NeedElixir,
                                             logValidateCardResult.ToString());

            return new BattleActionResult() { IsDoing = false };
        }

        private bool HasBuffTauntDiscordTarget()
        {
            if (_battle.Players.ContainsKey(_cardUseData.Target.PlayerType) == false)
                return false;

            if (_cardUseData.Target.PlayerType == _cardUseData.Performer.PlayerType)
                return false;

            if (_battle.Players[_cardUseData.Target.PlayerType].Units.ContainsKey(_cardUseData.Target.UnitPositionIndex) == false)
                return false;

            var target = _battle.Players[_cardUseData.Target.PlayerType].Units[_cardUseData.Target.UnitPositionIndex];
            if (target == null)
                return false;

            foreach (var units in _battle.Players[_cardUseData.Target.PlayerType].Units)
            {
                if (units.Value == target)
                    continue;

                if (units.Value.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_TAUNT))
                    return true;

                if (units.Value.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_SEVEN_DEVIL))
                    return true;

                if (units.Value.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_EIGHT_DEVIL))
                    return true;
            }

            return false;
        }

        private void IncreaseCardUseStatus(Card card, PlayerType playerType)
        {
            var rarity = ActionStatusType.CardUseWithNormal;

            switch (card.CardRarity)
            {
                case AkaEnum.CardRarity.SPECIAL:
                    rarity = ActionStatusType.CardUseWithSpecial;
                    break;
                case AkaEnum.CardRarity.ULTIMATE:
                    rarity = ActionStatusType.CardUseWithUltimate;
                    break;
            }
            _battle.Players[playerType].ActionLog.IncreaseStatus(rarity);

            if (Enum.TryParse<ActionStatusType>("CardUseWith" + card.Elixir.ToString(), true, out var elixirCost))
            {
                _battle.Players[playerType].ActionLog.IncreaseStatus(elixirCost);
            }

            card.ActionLog.IncreaseStatus(ActionStatusType.CardActionUse);
        }

        private bool IsValidateEnqueueSkill(Unit performerUnit, CardUseActionData cardUseActionData)
        {
            if (performerUnit.IsDeath() || cardUseActionData.UseCard == null)
            {
                return false;
            }
            return true;
        }

        private void UseElixir(ProtoCardUse protoCardUse, int needElixir)
        {
            _battle.Players[protoCardUse.Performer.PlayerType].UseElixir(needElixir);
        }

        private void UseHp(CardUseActionData cardUseActionData)
        {
            var performer = _battle.Players[_cardUseData.Performer.PlayerType].Units[_cardUseData.Performer.UnitPositionIndex];
            var needHp = performer.UnitData.UnitStatus.MaxHp * _card.HpCost;
            var decreaseHpInfo = performer.CardUseDecreaseHp(needHp, true);
            cardUseActionData.PerformerUnitInfo = new ProtoTargetUnitInfo()
            {
                PlayerType = performer.PlayerType,
                UnitId = performer.UnitData.UnitIdentifier.UnitId,
                Hp = performer.UnitData.UnitStatus.Hp,
                DecreaseHpInfo = decreaseHpInfo,
                IsCritical = false,
                Shields = null,
                IsShieldIgnore = false,
                RemoveBuffs = null
            };

            if (_card.HpCost <= 0)
                return;

            S2CManager.SendCardUseDecreaseHp(performer, cardUseActionData.PerformerUnitInfo);
        }

        private void EnqueueSkill(ProtoCardUse protoCardUse, CardUseActionData cardUseActionData)
        {
            _battle.Players[protoCardUse.Performer.PlayerType].Units[protoCardUse.Performer.UnitPositionIndex].EnqueueSkill(cardUseActionData);
        }

        private void EnqueueSkillReservation(ProtoCardUse protoCardUse, CardUseActionData cardUseActionData)
        {
            _battle.EnqueueSkillReservation(
                        _battle.Players[protoCardUse.Performer.PlayerType].Units[protoCardUse.Performer.UnitPositionIndex].EnqueueSkill,
                        protoCardUse,
                        cardUseActionData);
        }

        private bool IsSilence()
        {
            if (_battle.Players[_cardUseData.Performer.PlayerType].Units.ContainsKey(_cardUseData.Performer.UnitPositionIndex) == false)
                return false;

            var unit = _battle.Players[_cardUseData.Performer.PlayerType].Units[_cardUseData.Performer.UnitPositionIndex];
            if (unit.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_SILENCE))
                return true;

            return false;
        }

        private bool IsEnoughHp()
        {
            if (_battle.Players.ContainsKey(_cardUseData.Performer.PlayerType) == false)
                return false;

            if (_battle.Players[_cardUseData.Performer.PlayerType].Units.ContainsKey(_cardUseData.Performer.UnitPositionIndex) == false)
                return false;

            var performer = _battle.Players[_cardUseData.Performer.PlayerType].Units[_cardUseData.Performer.UnitPositionIndex];
            var needHp = performer.UnitData.UnitStatus.MaxHp * _card.HpCost;

            var offeringUnit = GetHasOfferingUnit();
            if (offeringUnit != null)
            {
                if (offeringUnit.UnitData.UnitStatus.Hp > needHp)
                    return true;

                return false;
            }

            if (performer.UnitData.UnitStatus.Hp <= needHp)
                return false;

            return true;
        }

        private Unit GetHasOfferingUnit()
        {
            foreach (var unit in _battle.Players[_cardUseData.Performer.PlayerType].Units)
            {
                if (unit.Key == _cardUseData.Performer.UnitPositionIndex)
                    continue;

                if (unit.Value.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_OFFERING))
                    return unit.Value;
            }

            return null;
        }
    }
}