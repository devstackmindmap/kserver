using AkaData;
using AkaEnum;
using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using PlayerType = AkaEnum.Battle.PlayerType;

namespace BattleLogic
{
    public class Unit : IDisposable
    {
        private readonly NextBuffs _nextBuffs = new NextBuffs();
        private readonly NextBuffSkills _nextBuffSkills = new NextBuffSkills();
        private readonly UnitBuffs _unitBuffs = new UnitBuffs();
        private readonly UnitShields _shields = new UnitShields();

        public UnitData UnitData;

        private Player _myPlayer;
        private NormalAttackTimer _attackTimer;
        private BattleHelper _battleHelper;
        private Player _enemyPlayer;
        private UnitActionStatus _unitActionStatus = UnitActionStatus.None;
        private DateTime _recentAttackStartTime;
        private UnitPoison _poison;
        private UnitPassive _unitPassive;
        private UnitElixirFactory _elixirFactory;
        private DataWeaponStat _dataWeaponStat;
        private UnitActionLog _actionStatus;

        public Player MyPlayer { get { return _myPlayer; } }
        public NormalAttackTimer AttackTimer { get { return _attackTimer; } }
        public long NextAttackTime => AttackTimer?.NextAttackTime() ?? 0;
        public BattleHelper BattleHelper { get { return _battleHelper; } }
        public UnitActionStatus UnitActionStatus { get { return _unitActionStatus; } set { _unitActionStatus = value; } }
        public DateTime RecentAttackStartTime { get { return _recentAttackStartTime; } }
        public UnitShields UnitShields => _shields;
        public int Shield => _shields.Shield;
        public PlayerType PlayerType => _myPlayer.PlayerIdentifier.Player;
        public UnitBuffs UnitBuffs => _unitBuffs;
        public NextBuffs NextBuffs => _nextBuffs;
        public NextBuffSkills NextBuffSkills => _nextBuffSkills;
        public UnitPassive UnitPassive => _unitPassive;
        public bool IsPoison => _poison.IsPoison;
        public UnitPoison PoisonInfo => _poison;
        public DataWeaponStat DataWeaponStat { get => _dataWeaponStat; set => _dataWeaponStat = value; }
        public uint SkinId { get; }
        public ActionLog ActionLog { get; }


        public Unit(uint unitId, uint level, uint skinId, int position, MonsterType monsterType = MonsterType.Normal)
        {
            var unitStatInfo = Data.GetUnitStat(unitId, level);
            var unitIdentifier = new UnitIdentifier(unitId, level, position, skinId, unitStatInfo.PassiveConditionId, monsterType);
            var animationLength = Data.GetAnimationLength(unitIdentifier.UnitInitial, AnimationType.Attack);

            var unitStat = new UnitStatus(unitStatInfo, animationLength);
            unitStat.AttackDelay = animationLength.Bullet;
            unitStat.FirstTakeDamage = animationLength.TakeDamage;
            UnitData = new UnitData(unitStat, unitIdentifier);
            SkinId = skinId;
            ActionLog = new UnitActionLog();
        }

        public virtual void UnitInitialize(Player myPlayer, Player enemyPlayer, BattleHelper battleHelper, List<uint> affixList)
        {
            _battleHelper = battleHelper;
            _myPlayer = myPlayer;
            _enemyPlayer = enemyPlayer;
            _attackTimer = new NormalAttackTimer(UnitData.UnitStatus.AttackSpeed * 1000, false, EnqueueAttack);
            _attackTimer.Name = UnitData.UnitIdentifier.UnitId.ToString();

            UnitData.UnitStatus.Unit = this;
            _poison = new UnitPoison(this, (int)Data.GetConstant(DataConstantType.POISON_TIC_SEC).Value);
            _unitPassive = new UnitPassive(this, _dataWeaponStat, affixList, myPlayer.TreasureIdList);
            _elixirFactory = new UnitElixirFactory(this);
        }

        public ProtoTargetDecreaseHpInfo DecreaseHp(Unit performer, float damage, DamageReasonType reasonType, int attackDelay, bool isShieldIgnore = false)
        {
            var addDmgRate = BuffStateCalculator.GetAddDamageRateByApplyUnitSpecies(performer, this);
            var decreaseHpInfo = DecreaseHp(damage * addDmgRate, reasonType, attackDelay, isShieldIgnore, performer.UnitData.UnitStatus.ShieldDamageRate);

            performer.ActionLog.AddStatus(ActionStatusType.UnitActionDealing, decreaseHpInfo.Damage);
            return decreaseHpInfo;
        }

        public ProtoTargetDecreaseHpInfo DecreaseHp(float damage, DamageReasonType reasonType, int attackDelay, bool isShieldIgnore, float shieldDamageRate = 1, bool checkOffering = true)
        {
            var buffSkillInvincibility = GetConditionBuffSkill(SkillEffectType.BUFF_STATE_INVINCIBILITY);
            if (buffSkillInvincibility != null && buffSkillInvincibility.IsValid())
            {
                buffSkillInvincibility.DecreaseCount();
                if (buffSkillInvincibility.IsValid() == false)
                    EnqueueBuffEnd(buffSkillInvincibility);
                return new ProtoTargetDecreaseHpInfo();
            }

            var preConsumedPoison = _poison.GetPreConsumedPoison(attackDelay);
            int realDamage = (int)damage;

            if (isShieldIgnore)
                damage = (damage + preConsumedPoison);
            else
            {
                var damageInfo = _shields.GetExtraDamageAndDoDamageToShield(damage, shieldDamageRate);
                damage = preConsumedPoison + damageInfo.ExtraDamage;
                realDamage = (int)damageInfo.realDamage;
            }

            if (checkOffering)
            {
                var offeringTargetUnitInfo = GetOfferingTargetUnitInfo(damage, false);
                if (offeringTargetUnitInfo != null)
                {
                    return new ProtoTargetDecreaseHpInfo()
                    {
                        Damage = 0,
                        OfferingTarget = offeringTargetUnitInfo
                    };
                }
            }

            UnitData.UnitStatus.Hp -= (int)damage;

            if (false == _myPlayer.AlreadyUnitDeath(this))
                CheckBattleEnd(reasonType);

            return new ProtoTargetDecreaseHpInfo()
            {
                Damage = realDamage
            };
        }

        public ProtoTargetDecreaseHpInfo CardUseDecreaseHp(float needHp, bool checkOffering)
        {
            if (checkOffering)
            {
                var offeringTargetUnitInfo = GetOfferingTargetUnitInfo(needHp, true);
                if (offeringTargetUnitInfo != null)
                {
                    return new ProtoTargetDecreaseHpInfo()
                    {
                        Damage = 0,
                        OfferingTarget = offeringTargetUnitInfo
                    };
                }
            }

            UnitData.UnitStatus.Hp -= (int)needHp;

            if (false == _myPlayer.AlreadyUnitDeath(this))
                CheckBattleEnd(DamageReasonType.Offering);

            return new ProtoTargetDecreaseHpInfo()
            {
                Damage = (int)needHp
            };
        }
        
        private ProtoTargetUnitInfo GetOfferingTargetUnitInfo(float damage, bool isCardUse)
        {
            foreach (var myPlayerUnit in _myPlayer.Units)
            {
                if (myPlayerUnit.Value.UnitData.UnitIdentifier.UnitId == UnitData.UnitIdentifier.UnitId)
                    continue;

                var offeringBuff = myPlayerUnit.Value.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_OFFERING) as BuffSkillOffering;
                if (offeringBuff == null)
                    continue;

                if (offeringBuff.IsValid() == false)
                {
                    EnqueueBuffEnd(offeringBuff);
                    continue;
                }

                if (offeringBuff.Performer.UnitData.UnitIdentifier.UnitId != UnitData.UnitIdentifier.UnitId)
                    continue;

                offeringBuff.CalculateValue(ref damage, null);
                var decreaseHpInfo = isCardUse ? myPlayerUnit.Value.CardUseDecreaseHp(damage, false) : 
                    myPlayerUnit.Value.DecreaseHp(damage, DamageReasonType.Offering, 0, true, 1, false);
                return new ProtoTargetUnitInfo()
                {
                    PlayerType = myPlayerUnit.Value.PlayerType,
                    UnitId = myPlayerUnit.Value.UnitData.UnitIdentifier.UnitId,
                    Hp = myPlayerUnit.Value.UnitData.UnitStatus.Hp,
                    DecreaseHpInfo = decreaseHpInfo,
                    IsCritical = false,
                    Shields = myPlayerUnit.Value.UnitShields.GetShieldInfos(),
                    IsShieldIgnore = false,
                    RemoveBuffs = null
                };
            }

            return null;
        }

        private void CheckUnderHp()
        {
            UnitPassive.PassiveConditionCheck(PassiveConditionType.PerUnderHp, UnitData.UnitStatus.MaxHp, UnitData.UnitStatus.Hp);
            BattleHelper.BattlePatternBehavior.DoHpCheckPatternSchedule(PlayerType, UnitData.UnitIdentifier.UnitId, UnitData.UnitStatus.MaxHp, UnitData.UnitStatus.Hp);
        }

        private void CheckBattleEnd(DamageReasonType reasonType)
        {
            if (IsDeath())
            {
                BattleHelper.BattleController.RemoveUnit(PlayerType, UnitData.UnitIdentifier.UnitId);
                S2CManager.SendProtoUnitDeath(this);

                _myPlayer.RemoveUnit(this);

                if (_myPlayer.Battle.BattleEndCheck(this))
                {
                    _myPlayer.Battle.TryBattleEnd();
                }
                else
                {
                    _myPlayer.Battle.UnitDeathPassiveConditionCheck();
                    BattleHelper.BattlePatternBehavior.DoDeadUnitCountPatternSchedule(PlayerType, UnitData.UnitIdentifier.UnitId);
                }
            }
            else
            {
                CheckUnderHp();
                BattleHelper.BattlePatternBehavior.DoGottenHitPatternSchedule(PlayerType, UnitData.UnitIdentifier.UnitId, reasonType);
            }
        }

        public void EnqueueAttack()
        {
            if (IsDeath())
                return;

            _unitActionStatus = UnitActionStatus.EnqueueAttack;

            CheckDoubleAttack();

            //var _battleNum = _myPlayer.Battle.GetBattleNum();
            //Logger.Instance().Debug($"[BNUM:{_battleNum}] [{PlayerType.ToString()}] [Unit] [Enqueue Attack] [ID:{UnitData.UnitIdentifier.UnitId}] ");
            //AkaLogger.Log.Battle.Enqueue.Log(MyPlayer.Battle.RoomId, PlayerType, UnitData.UnitIdentifier.UnitId, "Attack");
        }

        private void CheckDoubleAttack()
        {
            var doubleAttackBuff = _unitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_DOUBLE_ATTACK);

            int attackCount = 1;
            if (doubleAttackBuff != null)
            {
                if (doubleAttackBuff.IsValid())
                    attackCount += (int)doubleAttackBuff.Value;
                else
                    _unitBuffs.RemoveBuff(SkillEffectType.BUFF_STATE_DOUBLE_ATTACK);
            }
            _battleHelper.EnqueueAttack(new BattleActionAttack(this, attackCount));

        }

        public void EnqueueSkill(CardUseActionData data)
        {
            S2CManager.SendProtoValidateCard(this, data, ValidateCardResultType.Success);
            _battleHelper.BattleProgress.EnqueueSkill(new BattleActionSkill(this, data));

            //Logger.Instance().Debug("[Unit] [Enqueue Skill] [PlayerType:{0}] [Unit ID:{1}] [Card ID:{2}] [CardStat ID:{3}] [Position Index:{4}] ",
            //    PlayerType, UnitData.UnitIdentifier.UnitId, data.UseCard.CardId, data.UseCard.CardStatId, UnitData.UnitIdentifier.UnitPositionIndex);
            //AkaLogger.Log.Battle.Enqueue.Log(MyPlayer.Battle.RoomId, PlayerType, UnitData.UnitIdentifier.UnitId, data.UseCard.CardId, data.UseCard.CardStatId, UnitData.UnitIdentifier.UnitPositionIndex, "Skill");
        }

        public void EnqueuePassive(CardUseActionData cardData)
        {
            _battleHelper.BattleProgress.EnqueuePassive(new BattleActionSkill(this, cardData));
            //Logger.Instance().Debug($"[Unit] [Enqueue Skill Passive] [PlayerType:{PlayerType}] [Unit ID:{UnitData.UnitIdentifier.UnitId}] " +
            //    $"[Card ID:{card.CardId}] [CardStat ID:{card.CardStatId}] [Position Index:{UnitData.UnitIdentifier.UnitPositionIndex}] ");
            //AkaLogger.Log.Battle.Enqueue.Log(MyPlayer.Battle.RoomId, PlayerType, UnitData.UnitIdentifier.UnitId, card.CardId, card.CardStatId, UnitData.UnitIdentifier.UnitPositionIndex, "Passive");
        }

        public void EnqueueBuffEnd(IBuffSkill skillBuff)
        {
            BattleHelper.EnqueueBuffEnd(new BattleActionBuffEnd(this, skillBuff));

            //Logger.Instance().Debug("[Unit] [Enqueue BuffEnd ] [ID:{0}] ", UnitData.UnitIdentifier.UnitId);
            //AkaLogger.Log.Battle.Enqueue.Log(MyPlayer.Battle.RoomId, PlayerType,  UnitData.UnitIdentifier.UnitId, "BuffEnd");
        }

        public BattleActionAttackResult DoAttack()
        {
            var interaction = new NormalAttack(this, _myPlayer, _enemyPlayer);
            if (interaction.IsCanDoAction() == false)
                return new BattleActionAttackResult() { IsDoing = false };

            _recentAttackStartTime = DateTime.UtcNow;
            return interaction.DoAttack();
        }

        //public BattleActionSkillResult DoSkill(Card card, ProtoTarget target, uint replacedCardStatId = 0, int replacedHandIndex = 0, uint? nextCardStatId = 0)
        public BattleActionSkillResult DoSkill(CardUseActionData cardUseActionData)
        {
            var interaction = new SkillBehavior(this);
            return interaction.DoSkill(cardUseActionData);
        }

        public bool IsCanDoAction()
        {
            if (IsDeath() || _myPlayer.IsEndBattle())
                return false;

            return true;
        }

        public bool IsDeath()
        {
            if (UnitData.UnitStatus.Hp <= 0)
                return true;

            return false;
        }

        public void Dispose()
        {
            _attackTimer?.Dispose();
            _attackTimer = null;

            UnitBuffs.Dispose();

            _shields.Dispose();
            _poison.Dispose();
            _elixirFactory.Dispose();
        }

        public void AddBulletTime(int bulletTime)
        {
            _unitBuffs.AddBulletTime(bulletTime);
            _shields.AddBulletTime(bulletTime);
        }

        public BaseSkillProto AddConditionBuff(SkillEffectType skillEffectType, DataSkillOption skillOption, Unit performer, Unit target, int bulletTime)
        {
            UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenCondition);
            if (skillEffectType == SkillEffectType.BUFF_STATE_POISON)
                UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenPoision);

            return _unitBuffs.AddBuff(skillEffectType, skillOption, performer, target, bulletTime);
        }

        public void AddNextBuff(SkillEffectType skillEffectType, DataSkillOption skillOption, int animationLength, int takeDamageTime)
        {
            _nextBuffs.AddBuff(skillEffectType, skillOption, animationLength, takeDamageTime);
        }

        public void AddNextBuffSkill(SkillEffectType skillEffectType, DataSkillOption skillOption, int animationLength, int takeDamageTime)
        {
            _nextBuffSkills.AddBuff(skillEffectType, skillOption, animationLength, takeDamageTime);
        }

        public bool RemoveConditionBuff(SkillEffectType skillEffectType)
        {
            return _unitBuffs.RemoveBuff(skillEffectType);
        }

        public List<SkillEffectType> RemoveConditionBuffs()
        {
            return _unitBuffs.RemoveBuffs();
        }

        public List<SkillEffectType> RemoveConditionBuffs(List<SkillEffectType> ignoreTypes, params StateGoodBadType[] goodBadTypes)
        {
            return _unitBuffs.RemoveBuffs(ignoreTypes, goodBadTypes);
        }

        public void RemoveNextBuff(params SkillEffectType[] skillEffectTypes)
        {
            _nextBuffs.RemoveBuff(skillEffectTypes);
        }

        public void RemoveNextBuffSkills(List<SkillEffectType> ignoreEffectTypes)
        {
            _nextBuffSkills.RemoveBuff(ignoreEffectTypes);
        }

        public bool IsContainNextBuff(SkillEffectType skillEffectType)
        {
            return _nextBuffs.Buffs.ContainsKey(skillEffectType);
        }

        public IBuffSkill GetConditionBuffSkill(SkillEffectType skillEffectType)
        {
            return _unitBuffs.GetBuffSkill(skillEffectType);
        }

        public List<ProtoCurrentBuffInfo> IncreaseConditionTime(float increaseTime)
        {
            return _unitBuffs.IncreaseConditionTime(increaseTime);
        }

        public List<ProtoCurrentBuffInfo> IncreaseConditionTimeRate(float rate)
        {
            return _unitBuffs.IncreaseConditionTimeRate(rate);
        }

        public void Pause()
        {
            if (UnitActionStatus == UnitActionStatus.None)
                AttackTimer.Pause();

            UnitBuffs.Pause();
            _shields.Pause();
            _poison.Pause();
            _elixirFactory.Pause();
        }

        public void Restart(int bulletTime)
        {
            if (UnitActionStatus == UnitActionStatus.None)
                AttackTimer.Restart(bulletTime);

            UnitBuffs.Restart(bulletTime);
            _shields.Restart(bulletTime);
            _poison.Restart(bulletTime);
            _elixirFactory.Restart(bulletTime);
        }

        public UnitShield AddShield(Card card, DataSkillOption dataSkillOption, int bulletTime)
        {
            return _shields.AddShield(card, dataSkillOption, bulletTime, UnitPassive);
        }

        public void AddPoison(int addPoison)
        {
            _poison.AddPoison(addPoison);
        }

        public void MultiplePoison(float rate)
        {
            _poison.MultiplePoison(rate);
        }

        public void AddElixirFactory(float interval, DateTime startTime)
        {
            _elixirFactory.AddElixir(interval, startTime);
        }

        public void AddElixirFactoryEnd()
        {
            _elixirFactory.ElixirFactoryEnd();
        }

        public void ShieldEnd()
        {
            _shields.ShieldEnd();
        }

        public void GottenSkillAttack()
        {
            UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenSkillAttack);
            UnitPassive.PassiveConditionCheck(PassiveConditionType.PerGottenAttack);
        }

        public int GetPoisonStack()
        {
            return _poison.Stack;
        }

        public void PoisonEnd()
        {
            _poison.PoisonEnd();
        }

        public bool IsContainConditionBuffAndNotValidRemove(SkillEffectType skillEffectType)
        {
            var buff = GetConditionBuffSkill(skillEffectType);
            if (buff == null)
                return false;

            if (buff.IsValid() == false)
            {
                EnqueueBuffEnd(buff);
                return false;
            }

            return true;
        }
    }
}
