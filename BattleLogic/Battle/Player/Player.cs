using AkaEnum;
using AkaEnum.Battle;
using AkaUtility;
using CommonProtocol;
using CommonProtocol.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BattleLogic
{
    public class Player : IDisposable
    {
        public PlayerIdentifier PlayerIdentifier;
        public Dictionary<int, Unit> Units = new Dictionary<int, Unit>();
        public Dictionary<int, Unit> DeathUnits = new Dictionary<int, Unit>();

        public ActionLog ActionLog { get; private set; }


        private Player _enemy;
        public Player Enemy => _enemy;
        //   private IBattleEndCondition _battleEndCondition;
        private Battle _battle;
        private Elixir _elixir;
        private SpinLock _unitsLock = new SpinLock();

        public Battle Battle => _battle;
        public double CurrentElixir => _elixir.CurrentElixir;
        public long RecentOnChargingTime => _elixir.RecentOnChargingTime;

        private List<uint> _affixList;
        public List<uint> TreasureIdList { get; private set; }

        public Player(PlayerIdentifier playerIdentifier)
        {
            PlayerIdentifier = playerIdentifier;
        }

        public void PlayerInitialize(Player enemy, Dictionary<int, Unit> units, Battle battle, string nickname, uint profileIconId,
            List<uint> affixList, List<uint> treasureIdList)
        {
            _battle = battle;
            _enemy = enemy;
            _elixir = new Elixir(battle.Enviroment);
            ActionLog = new PlayerActionLog();
            PlayerIdentifier.Nickname = nickname;
            PlayerIdentifier.ProfileIconId = profileIconId;
            Units = units;
            TreasureIdList = treasureIdList;
            _affixList = affixList;
            UnitsInitialize();
        }

        public void Start(DateTime battleStartTime)
        {
            _elixir.Start(battleStartTime);
        }

        public void Dispose()
        {
            _elixir.Dispose();
        }


        private void UnitsInitialize()
        {
            foreach (var unit in Units)
            {
                unit.Value.UnitInitialize(this, _enemy, Battle.BattleHelper, _affixList);
            }
        }

        public bool AlreadyUnitDeath(Unit unit)
        {
            return DeathUnits.TryGetValue(unit.UnitData.UnitIdentifier.UnitPositionIndex, out var deathUnit);
        }

        public void RemoveUnit(Unit unit)
        {
            bool lockTaken = false;
            try
            {
                _unitsLock.Enter(ref lockTaken);
                unit.Dispose();
                Units.Remove(unit.UnitData.UnitIdentifier.UnitPositionIndex);
                DeathUnits.Add(unit.UnitData.UnitIdentifier.UnitPositionIndex, unit);
                Enemy.ActionLog.IncreaseStatus(ActionStatusType.KillUnit);
                ActionLog.IncreaseStatus(ActionStatusType.UnitDeath);

            }
            catch (Exception ex)
            {
                AkaLogger.Log.Debug.Exception("RemoveUnit:" + Units.Count + " RemoveIndex:" + unit.UnitData.UnitIdentifier.UnitPositionIndex, ex);
                AkaLogger.Logger.Instance().Error($"[Player.RemoveUnit] {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (lockTaken)
                    _unitsLock.Exit();
            }
        }

        public Unit[] GetSafeUnits()
        {
            bool lockTaken = false;
            Unit[] result = null;
            try
            {
                _unitsLock.Enter(ref lockTaken);
                result = Units.Values.ToArray();
            }
            catch (Exception ex)
            {
                AkaLogger.Log.Debug.Exception("Player.GetSafeUnits", ex);
                AkaLogger.Logger.Instance().Error($"[Player.GetSafeUnits] {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (lockTaken)
                    _unitsLock.Exit();
            }
            return result;
        }

        public bool IsEndBattle()
        {
            if (Battle.IsBattleEnd)
                return true;

            return false;
        }

        public Unit GetUnitByRandom()
        {
            var choiceIndex = AkaRandom.Random.Next(0, Units.Count);
            return Units[choiceIndex];
        }

        public void AddBulletTime(int bulletTime)
        {
            foreach (var unit in Units)
            {
                unit.Value.AddBulletTime(bulletTime);
            }
        }

        public void FillEnemyInfo(ProtoBeforeBattleStart protoBattleStart)
        {
            protoBattleStart.EnemyPlayer.Nickname = _enemy.PlayerIdentifier.Nickname;
            protoBattleStart.EnemyPlayer.UserId = _enemy.PlayerIdentifier.UserId;
            protoBattleStart.EnemyPlayer.ProfileIconId = _enemy.PlayerIdentifier.ProfileIconId;
            _enemy.FillUnitInfo(protoBattleStart.EnemyPlayer.Units);
        }

        public void FillEnemyInfo(ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            protoCurrentBattleStatus.EnemyPlayer.Nickname = _enemy.PlayerIdentifier.Nickname;
            protoCurrentBattleStatus.EnemyPlayer.UserId = _enemy.PlayerIdentifier.UserId;
            protoCurrentBattleStatus.EnemyPlayer.ProfileIconId = _enemy.PlayerIdentifier.ProfileIconId;
            _enemy.FillUnitInfo(protoCurrentBattleStatus.EnemyPlayer.Units);
        }

        private void FillUnitInfo(List<ProtoUnitInfo> units)
        {
            foreach (var unit in Units)
            {
                units.Add(new ProtoUnitInfo
                {
                    UnitId = unit.Value.UnitData.UnitIdentifier.UnitId,
                    Level = unit.Value.UnitData.UnitIdentifier.Level,
                    SkinId = unit.Value.SkinId,
                    WeaponInfo = unit.Value.DataWeaponStat == null ? null : new ProtoEquipInfo()
                    {
                        Id = unit.Value.DataWeaponStat.WeaponId,
                        Level = unit.Value.DataWeaponStat.Level
                    }
                });
            }
        }

        public void FillUnitInfo(List<ProtoCurrentUnitInfo> units)
        {
            foreach (var unit in Units)
            {
                units.Add(new ProtoCurrentUnitInfo
                {
                    UnitId = unit.Value.UnitData.UnitIdentifier.UnitId,
                    Level = unit.Value.UnitData.UnitIdentifier.Level,
                    Hp = unit.Value.UnitData.UnitStatus.Hp,
                    GrowthAtk = unit.Value.UnitData.UnitStatus.GrowthAtk,
                    GrowthCriticalRate = unit.Value.UnitData.UnitStatus.GrowthCriticalRate,
                    GrowthCriticalDamageRate = unit.Value.UnitData.UnitStatus.GrowthCriticalDamageRate,
                    Shields = unit.Value.UnitShields.GetShieldInfos(),
                    UnitPositionIndex = unit.Value.UnitData.UnitIdentifier.UnitPositionIndex,
                    Buffs = GetFillBuffs(unit.Value),
                    NextAttackTime = unit.Value.NextAttackTime,
                    SkinId = unit.Value.SkinId,
                    WeaponInfo = unit.Value.DataWeaponStat == null ? null : new ProtoEquipInfo()
                    {
                        Id = unit.Value.DataWeaponStat.WeaponId,
                        Level = unit.Value.DataWeaponStat.Level
                    },
                    AttackSpeed = GetCurrentUnitAttackSpeed(unit.Value)
                });
            }

            foreach (var unit in DeathUnits)
            {
                units.Add(new ProtoCurrentUnitInfo
                {
                    UnitId = unit.Value.UnitData.UnitIdentifier.UnitId,
                    Level = unit.Value.UnitData.UnitIdentifier.Level,
                    Hp = 0,
                    GrowthAtk = unit.Value.UnitData.UnitStatus.GrowthAtk,
                    GrowthCriticalRate = unit.Value.UnitData.UnitStatus.GrowthCriticalRate,
                    GrowthCriticalDamageRate = unit.Value.UnitData.UnitStatus.GrowthCriticalDamageRate,
                    Shields = new List<ProtoShieldInfo>(),
                    UnitPositionIndex = unit.Value.UnitData.UnitIdentifier.UnitPositionIndex,
                    Buffs = new List<ProtoCurrentBuffInfo>(),
                    NextAttackTime = 0
                });
            }
        }

        private float GetCurrentUnitAttackSpeed(Unit unit)
        {
            var attackSpeed = (float)unit.UnitData.UnitStatus.AttackSpeed;
            var buffAttackSpeed = unit.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_ATTACKSPEED_RATE);
            if (buffAttackSpeed != null)
                attackSpeed *= buffAttackSpeed.Value;

            var buffSlowAttackSpeed = unit.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_SLOWATTACKSPEED_RATE);
            if (buffSlowAttackSpeed != null)
                attackSpeed *= buffSlowAttackSpeed.Value;

            return attackSpeed;
        }


        private List<ProtoCurrentBuffInfo> GetFillBuffs(Unit unit)
        {
            List<ProtoCurrentBuffInfo> buffs = new List<ProtoCurrentBuffInfo>();
            foreach (var buff in unit.UnitBuffs.Buffs)
            {
                buffs.Add(new ProtoCurrentBuffInfo
                {
                    SkillOptionId = buff.Value.SkillOptionId,
                    BuffStartTime = buff.Value.BuffStartTime.Ticks,
                    BuffEndTime = buff.Value.EndDateTime.Ticks,
                    RemainCount = buff.Value.RemainCount
                });
            }

            return buffs;
        }

        public void Pause()
        {
            _elixir.Pause();
        }

        public void Stop()
        {
            _elixir.Stop();
        }

        public void Restart(int bulletTime)
        {
            _elixir.Restart(bulletTime);
        }

        public ElixirCountStateData GetElixirCountState(Card card)
        {
            return _elixir.GetElixirCountState(card.Elixir);
        }

        public void EnqueueSkillReservation(Action<CardUseActionData> enqueueSkillAction, CardUseActionData cardUseActionData)
        {
            _elixir.EnqueueSkillReservation(enqueueSkillAction, cardUseActionData);
        }

        public void UseElixir(int needCount)
        {
            _elixir.UseElixir(needCount);
        }

        public void StartBooster()
        {
            _elixir.StartBooster();
            ActionLog.SetStatus(ActionStatusType.EnterBoostTime, 1);
        }

        public void AddElixir(int elixir)
        {
            _elixir.AddElixir(elixir);
        }

        public bool IsCanRetreat(uint userId)
        {
            return true;// userId == PlayerIdentifier.UserId && Battle.IsCanRetreat();
        }

        public void UnitDeathPassiveConditionCheck()
        {
            foreach (var unit in Units)
            {
                unit.Value.UnitPassive.PassiveConditionCheck(PassiveConditionType.PerDeadUnitCount);
            }
        }
    }
}
