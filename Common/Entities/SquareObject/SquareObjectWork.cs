
using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Common.Entities.SquareObject
{
    public class SquareObjectWork
    {
        public uint UserId { get; private set; }

        public ProtoSquareObject ObjectInfo { get; private set; }
        public ProtoSquareObjectState State => ObjectInfo.CurrentState;

        public virtual DateTime Now() => DateTime.UtcNow;

        public SquareObjectWork(ProtoSquareObject squareObject)
        {
            UserId = squareObject.CurrentState.UserId;
            ObjectInfo = Utility.CopyFrom(squareObject);
            ObjectInfo.CurrentState = Utility.CopyFrom(squareObject.CurrentState);
        }

        public void Reset(uint userId, uint objectLevel)
        {
            var now = Now();
            State.UserId = userId;
            State.IsActivated = true;
            State.ActivatedTime = now;
            State.SquareObjectPower = (int)Data.GetConstant(DataConstantType.SQUARE_OBJECT_START_POWER).Value;
            State.SquareObjectLevel = objectLevel;
            State.CurrentShield = Data.GetSquareObject(ObjectInfo.SquareObjectLevel).MaxShield;
            State.CurrentPlanetBoxExp = 0;
            State.CurrentPlanetBoxLevel = 1;
            State.CoreEnergy = Data.GetSquareObjectPlanetCore(ObjectInfo.CoreLevel).StartEnergy;
            State.EnergyRefreshTime = now;
            State.PowerRefreshTime = now;
            State.InvasionHistory = new List<ProtoSquareObjectInvasionHistory>();
            
            SetNextInvasionTime(now);
        }

        public void Recovery()
        {
            var now = Now();
            State.IsActivated = true;
            State.ActivatedTime = now;
            State.SquareObjectPower = (int)Data.GetConstant(DataConstantType.SQUARE_OBJECT_START_POWER).Value;

            var previousShield = State.CurrentShield;
            State.CurrentShield = Data.GetSquareObject(ObjectInfo.SquareObjectLevel).MaxShield;
            //    if (State.CurrentPlanetBoxLevel > 5)
            //        State.CurrentPlanetBoxLevel -= 5;
            //    State.CoreEnergy = Data.GetSquareObjectPlanetCore(ObjectInfo.CoreLevel).StartEnergy;
            //    State.ExtraCoreEnergy = 0;
            //    State.ExtraEnergyInjectedTime = now.AddDays(-2);
            var startEnergy = Data.GetSquareObjectPlanetCore(ObjectInfo.CoreLevel).StartEnergy;
            if (State.CoreEnergy + State.ExtraCoreEnergy < startEnergy)
                State.CoreEnergy = startEnergy;
            State.EnergyRefreshTime = now;
            State.PowerRefreshTime = now;
            State.EnableReward = false;
            State.InvasionHistory.Add(new ProtoSquareObjectInvasionHistory { 
                                            InvasionTime = now,
                                            PreviousShield = previousShield,
                                            RemainedShield = State.CurrentShield,
                                            Power = State.SquareObjectPower,
            });

            SetNextInvasionTime(now);
        }

        public bool IsMaxPlanetBoxExp()
        {
            return 0 == Data.GetSquareObjectPlanetBox(State.SquareObjectLevel, State.CurrentPlanetBoxLevel).NeedExpForNextLevelUp;
        }

        public void Stop()
        {
            State.IsActivated = false;
            State.EnableReward = false;

            var planetBox = Data.GetSquareObjectPlanetBox(State.SquareObjectLevel, State.CurrentPlanetBoxLevel);
            var gettingSquareObjectExp = planetBox.GiveToSquareObjectExp;
            ObjectInfo.SquareObjectExp += gettingSquareObjectExp;
        }

        private void StopDestroyed()
        {
            State.IsActivated = false;
            State.EnableReward = true;
        }


        public bool UseEnergy(int squareObjectEnergy, int agencyEnergy, bool hasSeasonPass)
        {
            var now = Now();
            var nextRefreshTime = now;

            var currentEnergy = State.CoreEnergy + State.ExtraCoreEnergy;
            var maxEnergy = Data.GetSquareObjectPlanetCore(ObjectInfo.CoreLevel).MaxPlanetCoreEnergy;

            if (currentEnergy < maxEnergy)
            {
                var acquistionEnergyMin = Data.GetConstant(DataConstantType.SQUARE_OBJECT_PLANET_CORE_ENERGY_ACQUISITION_UNIT_MINUTE).Value;
                var quantityEnergy = Data.GetConstant(DataConstantType.SQUARE_OBJECT_PLANET_CORE_ENERGY_QUANTITY_EACH_GET).Value;
                var elapsedMin = (int)((now - State.EnergyRefreshTime).TotalMinutes / acquistionEnergyMin);
                var gettingEnergy = elapsedMin * quantityEnergy;

                currentEnergy += (int)(float.Epsilon + gettingEnergy);

                if (currentEnergy > maxEnergy)
                    currentEnergy = maxEnergy;
                else
                    nextRefreshTime = State.EnergyRefreshTime.AddMinutes(elapsedMin * acquistionEnergyMin);
            }

            currentEnergy = currentEnergy - squareObjectEnergy - agencyEnergy;
            if (currentEnergy >= 0)
            {
                SquareObjectSpentPowerRefresh(now);
                if (State.SquareObjectPower < 1)
                    State.PowerRefreshTime = now;

                State.SquareObjectPower += squareObjectEnergy;
                if (true == hasSeasonPass)
                {
                    var convertRate = (double)Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_CONVERTING_RATE_ENERGY_PREMIUM).Value + double.Epsilon;
                    State.SquareObjectPower += (int)(squareObjectEnergy * convertRate);
                }
             //   State.PowerRefreshTime = now;

                var bonusBoxEnergy = (int)(agencyEnergy * Data.GetSquareObjectPlanetAgency(ObjectInfo.AgencyLevel).PlanetCoreEnergyAdditionalRate + ConstValue.FLOAT_EPSILON);

                State.CurrentPlanetBoxLevel = GetLevelUpedPlanetBox(agencyEnergy + bonusBoxEnergy).PlanetBoxLevel;

                if (currentEnergy > maxEnergy)
                {
                    State.CoreEnergy = maxEnergy;
                    State.ExtraCoreEnergy = currentEnergy - maxEnergy;
                }
                else
                {
                    State.CoreEnergy = currentEnergy;
                    State.ExtraCoreEnergy = 0;
                }

                State.EnergyRefreshTime = nextRefreshTime;

          //      Console.WriteLine($"--- {currentEnergy}   {squareObjectEnergy}   {agencyEnergy}  - {nextRefreshTime} {now}");
                return true;
            }
            return false;
        }

        public byte[] InvasionHistoryToBinary()
        {
            return AkaSerializer.AkaSerializer<List<ProtoSquareObjectInvasionHistory>>.Serialize(State.InvasionHistory);
        }

        public void UpdateNextInvasionTime(DateTime now)
        {
            var invasionTime = Data.GetSquareObjectMonsterInvasionTime(State.NextInvasionLevel);
            State.NextInvasionTime = AkaRandom.Random.NextDateTime(now.AddSeconds(invasionTime.Min), now.AddSeconds(invasionTime.Max));

            AkaLogger.Log.User.SquareObject.UpdateNextInvasionLog(State.UserId, State.ActivatedTime, State.NextInvasionTime, State.NextInvasionLevel, State.NextInvasionMonsterId
                                                                    , State.SquareObjectLevel, ObjectInfo.SquareObjectLevel, ObjectInfo.CoreLevel, ObjectInfo.AgencyLevel);
        }

        public bool IsInvadingTime(DateTime now)
        {
            return true == State.IsActivated && State.NextInvasionTime < now;
        }


        public bool IsInvade(DateTime now, List<(DateTime startTime, DateTime endTime)> maintenenceTimes)
        {
            bool isInvade = false;
            try
            {
                bool isStopped = false;
                while (isStopped == false && true == IsInvadingTime(now))
                {
                    isInvade = true;
                    var findMaintenenceTime = maintenenceTimes.FirstOrDefault(maintenenceTime => maintenenceTime.startTime <= State.NextInvasionTime && maintenenceTime.endTime >= State.NextInvasionTime);
                    if (findMaintenenceTime.startTime != default)
                    {
                        SetNextInvasionTime(findMaintenenceTime.endTime);
                        continue;
                    }

                    var defenced = IsDefenced(State.NextInvasionTime);
                    if (defenced)
                    {
                        SetNextInvasionTime(State.NextInvasionTime);
                    }
                    else
                    {
                        isStopped = true;
                    }

                    var invasionResult = State.InvasionHistory[State.InvasionHistory.Count - 1];
                    AkaLogger.Logger.Instance().Debug($"Invade:{State.UserId} Defenced:{State.IsActivated} Now:{invasionResult.InvasionTime} "
                                                     + $"NextInvasion:{State.NextInvasionTime} Energy:{State.CoreEnergy} BoxLevel:{State.CurrentPlanetBoxLevel}");

                    AkaLogger.Log.User.SquareObject.InvasionLog(State.UserId, State.ActivatedTime, invasionResult.InvasionTime, invasionResult.InvasionLevel, invasionResult.MonsterId
                                                                , State.CurrentPlanetBoxLevel, invasionResult.Power, invasionResult.PreviousShield, State.IsActivated, State.SquareObjectLevel
                                                                , ObjectInfo.SquareObjectLevel, ObjectInfo.CoreLevel, ObjectInfo.AgencyLevel);
                }
            }
            catch (Exception e)
            {
                AkaLogger.Log.Debug.Info($"User:{State.UserId}  stack:{e.StackTrace}", "SquareObjectCheckScheduleException");
                AkaLogger.Logger.Instance().Debug($"Exception A:{State.UserId}  stack:{e.StackTrace} message:{e.ToString()}");
                StopDestroyed();
            }
            return isInvade;
        }


        private void SquareObjectSpentPowerRefresh(DateTime now)
        {
            var powerReduceMin = Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_REDUCE_UNIT_MINUTE).Value;
            var quantityPower = Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_REDUCE_QUANTITY_EACH).Value;

            var elapsedMin = (int)((now - State.PowerRefreshTime).TotalMinutes / powerReduceMin);
            if (elapsedMin > 0)
            {
             //   var spentRating = Math.Pow((double)(1 - quantityPower), elapsedMin);
                var nextRefreshTime = State.PowerRefreshTime.AddMinutes(elapsedMin * powerReduceMin); 

            //    State.SquareObjectPower = (int)(State.SquareObjectPower * spentRating);
                State.SquareObjectPower -= (int)(elapsedMin * quantityPower);
                if (State.SquareObjectPower < 0)
                    State.SquareObjectPower = 0;

                State.PowerRefreshTime = nextRefreshTime;
            }
        }

        private bool IsDefenced(DateTime now)
        {
            var monsterId = State.NextInvasionMonsterId;
            var monster = Data.GetSquareObjectMonster(monsterId);

            AkaLogger.Logger.Instance().Debug($"IsDefenced:{State.UserId} Monster:{State.NextInvasionMonsterId} CorrectData:{monster != null} Shield:{State.CurrentShield} Energy:{State.CoreEnergy} BoxLevel:{State.CurrentPlanetBoxLevel}");

            var agencyExp = monster.GiveToPlanetAgencyExp;
            var coreExp = agencyExp;

            SquareObjectSpentPowerRefresh(now);

            var previousShield = State.CurrentShield;
            var monsterCount = monster.MonsterHP - State.SquareObjectPower;
            if (monsterCount < 0)
                monsterCount = 0;

            var monsterAtk = monsterCount * monster.MonsterATK;
            State.CurrentShield -= monsterAtk;


            State.InvasionHistory.Add(new ProtoSquareObjectInvasionHistory
            {
                InvasionTime = now,
                InvasionLevel = State.NextInvasionLevel,
                RemainedShield = State.CurrentShield,
                Power = State.SquareObjectPower,
                MonsterAtk = monsterAtk,
                PreviousShield = previousShield,
                MonsterId = monsterId,
                GettingCoreExp = coreExp,
                GettingAgencyExp = agencyExp,
                MonsterCount = monsterCount
            });

            //      Console.WriteLine($"\t\tDo Battle monster{monsterId} Lv{State.NextInvasionLevel} Shield{State.CurrentShield} Atk{monsterAtk} "
            //          + $"\n\t\t power{State.SquareObjectPower} TotalMin {(now - State.PowerRefreshTime).TotalMinutes}  history:" + State.InvasionHistory.Count);

            if (State.CurrentShield <= 0 || State.InvasionHistory.Count > ConstValue.MAX_SQUARE_OBJECT_INVASION_COUNT)
            {
                StopDestroyed();
                return false;
            }

            return true;
        }


        private DataSquareObjectPlanetBox GetLevelUpedPlanetBox(int plusExp)
        {
            var rawBoxList = Data.GetSquareObjectPlanetBoxList(State.SquareObjectLevel);
            var boxList = rawBoxList.SkipWhile(box => box.PlanetBoxLevel < State.CurrentPlanetBoxLevel);
            uint boxExp = 0;

            var currentExp = State.CurrentPlanetBoxExp + plusExp;

            foreach ( var box in boxList)
            {
                boxExp += box.NeedExpForNextLevelUp;

                if (boxExp > currentExp)
                {
                    State.CurrentPlanetBoxExp =(int)(currentExp - (boxExp - box.NeedExpForNextLevelUp));
                    return box;
                }
            }
            return rawBoxList[rawBoxList.Count - 1];
        }

        private DataSquareObjectPlanetBox GetCurrentPlanetBox()
        {
            var rawBoxList = Data.GetSquareObjectPlanetBoxList(State.SquareObjectLevel);
            return rawBoxList.FirstOrDefault(box => box.PlanetBoxLevel == State.CurrentPlanetBoxLevel) ?? rawBoxList[rawBoxList.Count - 1];

        }

        private void SetNextInvasionTime(DateTime now)
        {
            var currentPlanetBox = GetCurrentPlanetBox();
            var nextInvasion = Data.GetAllSquareObjectMonsterInvasionTimes().First(invasionTime => invasionTime.PlanetBoxLevel > currentPlanetBox.PlanetBoxLevel);
            State.NextInvasionLevel = nextInvasion.MonsterInvasionLevel;
            State.NextInvasionMonsterId = AkaRandom.Random.ChooseElementRandomlyInCount(Data.GetSquareObject(State.SquareObjectLevel).InvasionLvLists[(int)State.NextInvasionLevel - 1]);

            UpdateNextInvasionTime(now);
        }

    }
}
