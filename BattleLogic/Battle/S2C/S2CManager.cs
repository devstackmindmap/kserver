using AkaEnum;
using AkaEnum.Battle;
using AkaSerializer;
using CommonProtocol;
using CommonProtocol.Battle;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public struct BattleSendData
    {
        public MessageType MessageType;
        public PlayerType PlayerType;
        public byte[] Data;
    }

    public static class S2CManager
    {
        public static void SendProtoUnitDeath(Unit unit)
        {
            //XXX PvE일 경우 전송하지않음.
            if (unit.MyPlayer.PlayerIdentifier.UserId == 0)
                return;

            ProtoUnitDeath protoUnitDeath = new ProtoUnitDeath();
            protoUnitDeath.UnitId = unit.UnitData.UnitIdentifier.UnitId;
            unit.MyPlayer.Battle.FillUpdateCardInfo(unit.MyPlayer.PlayerIdentifier.Player, protoUnitDeath);

            unit.MyPlayer.Battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.UnitDeath,
                PlayerType = unit.MyPlayer.PlayerIdentifier.Player,
                Data = AkaSerializer<ProtoUnitDeath>.Serialize(protoUnitDeath)
            });
        }

        public static void SendProtoValidateCard(Unit unit, CardUseActionData data, ValidateCardResultType resultType)
        {
            var protoValidateCard = new ProtoValidateCard()
            {
                MessageType = MessageType.ValidateCard,
                ResultType = resultType,
                PlayerType = unit.MyPlayer.PlayerIdentifier.Player,
                CurrentElixirCount = unit.MyPlayer.CurrentElixir,
                RecentElixirChargingTime = unit.MyPlayer.RecentOnChargingTime,
                HandIndex = data.ReplaceCardInfo.ReplacedHandIndex,
                PerformerUnitInfo = data.PerformerUnitInfo
                //HandIndex = data.ReplacedHandIndex
            };

            unit.MyPlayer.Battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.ValidateCard,
                PlayerType = protoValidateCard.PlayerType,
                Data = AkaSerializer<ProtoValidateCard>.Serialize(protoValidateCard)
            });
        }

        public static void SendBattleExtentionTime(Battle battle, PlayerType playerType)
        {
            battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.StartExtensionTime,
                PlayerType = playerType,
                Data = AkaSerializer.AkaSerializer<ProtoEmpty>.Serialize(new ProtoEmpty())
            });
        }

        public static void SendBoosterTime(Battle battle, PlayerType playerType)
        {
            battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.StartBoosterTime,
                PlayerType = playerType,
                Data = AkaSerializer<ProtoEmpty>.Serialize(new ProtoEmpty
                {
                    MessageType = MessageType.StartBoosterTime
                })
            });
        }

        internal static void SendBattleStart(Battle battle, PlayerType playerType)
        {
            battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.BattleStart,
                PlayerType = playerType,
                Data = AkaSerializer<ProtoBattleStart>.Serialize(new ProtoBattleStart
                {
                    MessageType = MessageType.BattleStart,
                    BattleStartTime = battle.Status.StartDateTime.Ticks
                })
            });
        }

        public static void SendNormalAttack(Unit unit, ProtoNormalAttack protoNormalAttack)
        {
            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.AttackUnit,
                PlayerType = PlayerType.All,
                Data = AkaSerializer<ProtoNormalAttack>.Serialize(protoNormalAttack)
            });
        }

        public static void SendSkill(Unit unit, ProtoSkill protoSkill)
        {
            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.Skill,
                PlayerType = PlayerType.All,
                Data = AkaSerializer<ProtoSkill>.Serialize(protoSkill)
            });
        }

        public static void SendInvalidateCardUse(Unit unit, ReplaceCardInfo replaceCardInfo)
        {
            unit.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.InvalidateCardUse,
                PlayerType = unit.PlayerType,
                Data = AkaSerializer<ProtoInvalidateCardUse>.Serialize(new ProtoInvalidateCardUse()
                {
                    MessageType = MessageType.InvalidateCardUse,
                    ReplacedCardStatId = replaceCardInfo.ReplacedCard.CardStatId,
                    ReplacedHandIndex = replaceCardInfo.ReplacedHandIndex,
                    NextCardStatId = replaceCardInfo.NextCardStatId
                })
            });
        }


        public static void SendCardUseResult(Battle battle, ProtoCardUse protoCardUse, ElixirCountState elixirCountState)
        {
            var serializedData = AkaSerializer<ProtoCardUseResult>.Serialize(new ProtoCardUseResult
            {
                ElixirCountState = elixirCountState,
                HandIndex = protoCardUse.HandIndex,
                MessageType = MessageType.CardUseResult,
            });

            battle.BattleHelper.BattleRecorder.EnqueueBehaviorForS2CRecord(new BattleSendData
            {
                Data = serializedData,
                MessageType = MessageType.CardUseResult,
                PlayerType = protoCardUse.Performer.PlayerType
            });

            battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.CardUseResult,
                PlayerType = protoCardUse.Performer.PlayerType,
                Data = serializedData
            });
        }


        public static void SendUpdateUnitAttackTime(Unit unit, float attackSpeed)
        {
            var protoUpdateUnitAttackTime = new ProtoUpdateUnitAttackTime()
            {
                PlayerType = unit.PlayerType,
                UnitId = unit.UnitData.UnitIdentifier.UnitId,
                MessageType = MessageType.UpdateUnitAttackTime,
                NextAttackTime = DateTime.UtcNow.AddMilliseconds(unit.AttackTimer.NowInterval).Ticks,
                AttackSpeed = attackSpeed
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.UpdateUnitAttackTime,
                Data = AkaSerializer<ProtoUpdateUnitAttackTime>.Serialize(protoUpdateUnitAttackTime),
                PlayerType = PlayerType.All
            });
        }

        public static void SendPoison(Unit unit, int damage)
        {
            var protoPoison = new ProtoPoison()
            {
                MessageType = MessageType.Poison,
                PlayerType = unit.PlayerType,
                UnitId = unit.UnitData.UnitIdentifier.UnitId,
                Shields = unit.UnitShields.GetShieldInfos(),
                Damage = damage
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.Poison,
                Data = AkaSerializer<ProtoPoison>.Serialize(protoPoison),
                PlayerType = PlayerType.All
            });
        }

        public static void SendAddElixir(Unit unit, int addElixir)
        {
            var protoElixirFactory = new ProtoAddElixir()
            {
                MessageType = MessageType.AddElixir,
                AddElixir = addElixir
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.AddElixir,
                Data = AkaSerializer<ProtoAddElixir>.Serialize(protoElixirFactory),
                PlayerType = unit.PlayerType
            });
        }

        public static void SendElectricShock(Unit unit, ProtoTargetDecreaseHpInfo decreaseHpInfo)
        {
            var protoElectricShock = new ProtoElectricShock()
            {
                MessageType = MessageType.ElectricShock,
                TargetUnitInfo = new ProtoTargetUnitInfo()
                {
                    PlayerType = unit.PlayerType,
                    UnitId = unit.UnitData.UnitIdentifier.UnitId,
                    Hp = unit.UnitData.UnitStatus.Hp,
                    DecreaseHpInfo = decreaseHpInfo,
                    IsCritical = false,
                    Shields = unit.UnitShields.GetShieldInfos(),
                    IsShieldIgnore = false
                }
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.ElectricShock,
                Data = AkaSerializer<ProtoElectricShock>.Serialize(protoElectricShock),
                PlayerType = PlayerType.All
            });
        }

        public static void SendEnqueueState(BattleAction enqueuedAction, BattleInteractionType battleInteractionType)
        {
            var unit = enqueuedAction?.Attacker;
            if (unit != null)
            {
                var protoEnqueuedAction = new ProtoEnqueuedAction()
                {
                    MessageType = MessageType.EnqueuedAction,
                    PlayerType = unit.PlayerType,
                    UnitId = unit.UnitData.UnitIdentifier.UnitId,
                    BattleInteractionType = battleInteractionType
                };

                unit.BattleHelper.Send(new BattleSendData()
                {
                    MessageType = MessageType.EnqueuedAction,
                    Data = AkaSerializer<ProtoEnqueuedAction>.Serialize(protoEnqueuedAction),
                    PlayerType = PlayerType.All
                });
            }
        }

        public static void SendEmoticonUse(Battle battle, ProtoEmoticonUse emoticonUse)
        {
            battle.BattleHelper.Send(new BattleSendData
            {
                MessageType = MessageType.OnEmoticonUse,
                PlayerType = PlayerType.All,
                Data = AkaSerializer<ProtoOnEmoticonUse>.Serialize(new ProtoOnEmoticonUse
                {
                    MessageType = MessageType.OnEmoticonUse,
                    EmoticonId = emoticonUse.EmoticonId,
                    PlayerType = emoticonUse.PlayerType,
                    IsPlayer = emoticonUse.IsPlayer
                })
            });
        }

        public static void SendIgnitionBomb(Unit unit, ProtoTargetDecreaseHpInfo decreaseHpInfo)
        {
            var protoElectricShock = new ProtoElectricShock()
            {
                MessageType = MessageType.IgnitionBomb,
                TargetUnitInfo = new ProtoTargetUnitInfo()
                {
                    PlayerType = unit.PlayerType,
                    UnitId = unit.UnitData.UnitIdentifier.UnitId,
                    Hp = unit.UnitData.UnitStatus.Hp,
                    DecreaseHpInfo = decreaseHpInfo,
                    IsCritical = false,
                    Shields = unit.UnitShields.GetShieldInfos(),
                    IsShieldIgnore = false
                }
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.IgnitionBomb,
                Data = AkaSerializer<ProtoElectricShock>.Serialize(protoElectricShock),
                PlayerType = PlayerType.All
            });
        }

        public static void SendMarkShock(Unit unit, ProtoTargetDecreaseHpInfo decreaseHpInfo)
        {
            var protoMark = new ProtoElectricShock()
            {
                MessageType = MessageType.MarkerShock,
                TargetUnitInfo = new ProtoTargetUnitInfo()
                {
                    PlayerType = unit.PlayerType,
                    UnitId = unit.UnitData.UnitIdentifier.UnitId,
                    Hp = unit.UnitData.UnitStatus.Hp,
                    DecreaseHpInfo = decreaseHpInfo,
                    IsCritical = false,
                    Shields = unit.UnitShields.GetShieldInfos(),
                    IsShieldIgnore = false
                }
            };

            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.MarkerShock,
                Data = AkaSerializer<ProtoElectricShock>.Serialize(protoMark),
                PlayerType = PlayerType.All
            });
        }

        public static void SendCardUseDecreaseHp(Unit unit, ProtoTargetUnitInfo targetUnitInfo)
        {
            unit.BattleHelper.Send(new BattleSendData()
            {
                MessageType = MessageType.CardUseDecreaseHp,
                Data = AkaSerializer<ProtoTargetUnitInfo>.Serialize(targetUnitInfo),
                PlayerType = unit.PlayerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1
            });
        }
    }
}
