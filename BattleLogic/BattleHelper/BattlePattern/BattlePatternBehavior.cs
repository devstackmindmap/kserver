using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using AkaUtility;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    public class BattlePatternBehavior
    {
        private Battle _battle;
        private StagePatternContainer _patternContainer;
        public BattlePatternBehavior()
        {
            _patternContainer = new StagePatternContainer(AkaEnum.Battle.PlayerType.Player2, new List<Pattern>());
        }

        public void BattleBehaviorIntialize(StagePatternContainer patternContainer)
        {
            _patternContainer = patternContainer;
        }


        public void SetBattle(Battle battle)
        {
            _battle = battle;
        }

        public void DoActionPattern()
        {
            bool enterLock = false;
            try
            {
                System.Threading.Monitor.TryEnter(this, ref enterLock);
                if (!enterLock)
                    return;
                var willPatterns = _patternContainer.PatternsOfMonster.Where(pattern => !(pattern is FlowPattern) && pattern.CanDoAction() );
                var units = _battle.GetPlayers()[_patternContainer.PlayerType].GetSafeUnits();
                var enemyPlayer = _battle.GetPlayers()
                                        .SkipWhile(player => player.Key == _patternContainer.PlayerType)
                                        .First();  //TODO change

                var enemyUnits = enemyPlayer.Value.Units;

                if (false == (units?.Any() ?? false))
                    return;


                foreach (var pattern in willPatterns)
                {
                    var unit = units.FirstOrDefault(u => u.UnitData.UnitIdentifier.UnitId == pattern.UnitId);
                    if (unit != null && false == unit.IsDeath())
                    {
                        ProtoTarget performer = new ProtoTarget
                        {
                            PlayerType = _patternContainer.PlayerType,
                            UnitPositionIndex = unit.UnitData.UnitIdentifier.UnitPositionIndex
                        };

                        var target = SelectTarget(enemyPlayer.Key, enemyUnits, _patternContainer.PlayerType, pattern);

                        if (target == null)
                        {
                            pattern.DidAction();
                            //AkaLogger.Logger.Instance().Info("\t\t[Not Run Pattern :" + pattern.UnitId + ":" + pattern.CardStatId + "] Invalid Target" + "");
                            //AkaLogger.Log.Battle.RunPattern.Log( _battle.RoomId, pattern.UnitId, pattern.CardStatId, false);


                        }
                        else
                        {
                            pattern.DoAction();
                            _battle.GetBattleController().CardUseWithPattern(pattern.CardStatId, pattern.PatternId, performer, target);
                            //AkaLogger.Logger.Instance().Info("\t\t[Run Pattern :" + pattern.UnitId + ":" + pattern.CardStatId + "]" + "");
                            //AkaLogger.Log.Battle.RunPattern.Log(_battle.RoomId, pattern.UnitId, pattern.CardStatId, true);
                        }

                    }
                }

                DoReplacePattern();
            }
            catch (Exception ex)
            {
                AkaLogger.Log.Debug.Exception("DoActionPattern" , ex);
                AkaLogger.Logger.Instance().Error($"[DoActionPattern] {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                if (enterLock)
                    System.Threading.Monitor.Exit(this);
            }
        }

        private void DoReplacePattern()
        {
            var willFlowGroupPatterns = _patternContainer.PatternsOfMonster
                                    .OfType<FlowPattern>()
                                    .GroupBy(pattern => pattern.ParentPattern);

            foreach (var patternGroup in willFlowGroupPatterns)
            {
                var nextFlowPattern = patternGroup.FirstOrDefault(pattern => pattern.CanDoAction());
                if (nextFlowPattern != null)
                {
                    foreach (var flowPattern in patternGroup)
                    {
                        flowPattern.Deactived = true;
                    }
                    _patternContainer.ReplacePattern(nextFlowPattern);
                }
            }
        }

        private ProtoTarget SelectTarget(PlayerType enemyType, Dictionary<int, Unit> enemyUnits, PlayerType playerType, Pattern pattern)
        {
            SetTarget(pattern.CardStatId, out var targetGroupType, out var targetType, out var forcedTargetId);

            IEnumerable<Unit> targetUnits = null;
            PlayerType targetPlayerType = PlayerType.All;
            if (targetGroupType == TargetGroupType.Enemy)
            {
                targetPlayerType = enemyType;
                targetUnits = enemyUnits.Values.Where(unitInfo => unitInfo.IsDeath() == false);
            }
            else if (targetGroupType == TargetGroupType.Ally)
            {
                targetPlayerType = playerType;
                targetUnits = _battle.GetPlayers()[_patternContainer.PlayerType].Units.Values.Where(unitInfo => unitInfo.IsDeath() == false);
            }
            else
            {
                targetPlayerType = PlayerType.All;
                targetUnits = enemyUnits.Values.Concat(_battle.GetPlayers()[_patternContainer.PlayerType].Units.Values)
                                               .Where(unitInfo => unitInfo.IsDeath() == false);

            }

            int selectedIndex = 0;
            if (targetType == TargetType.Target)
            {
                var unit = AkaRandom.Random.ChooseElementRandomlyInCount(targetUnits );
                selectedIndex = unit.UnitData.UnitIdentifier.UnitPositionIndex;    
            }
            else if(targetType == TargetType.TargetAndSelf)
            {
                if (targetGroupType == TargetGroupType.Ally)
                {
                    var allyUnits = targetUnits.Where(targetUnit => targetUnit.UnitData.UnitIdentifier.UnitId != pattern.UnitId);
                    if (false == allyUnits.Any())
                        selectedIndex = targetUnits.FirstOrDefault()?.UnitData.UnitIdentifier.UnitPositionIndex ?? 0;        //self
                    else
                        selectedIndex = AkaRandom.Random.ChooseElementRandomlyInCount(allyUnits).UnitData.UnitIdentifier.UnitPositionIndex;
                }
                else
                    selectedIndex = AkaRandom.Random.ChooseElementRandomlyInCount(targetUnits).UnitData.UnitIdentifier.UnitPositionIndex;
            }
            else if ( targetType == TargetType.ForcedTarget)
            {
                var unit = targetUnits.FirstOrDefault(targetUnit => targetUnit.UnitData.UnitIdentifier.UnitId == forcedTargetId);
                if (forcedTargetId == 0 || unit == null)
                    unit = AkaRandom.Random.ChooseElementRandomlyInCount(targetUnits);
                selectedIndex = unit.UnitData.UnitIdentifier.UnitPositionIndex;
            }

            ProtoTarget target = new ProtoTarget
            {
                PlayerType = targetPlayerType,
                UnitPositionIndex = selectedIndex
            };

            return target;
        }

        private void SetTarget(uint cardStatId, out TargetGroupType targetGroupType, out TargetType targetType, out uint targetId)
        {
            var cardStat = Data.GetCardStat(cardStatId);
            var skillConditionId = cardStat.SkillConditionId;
            targetId = 0;
            if (skillConditionId == 0)
            {
                targetGroupType = cardStat.TargetGroupType;
                targetType = cardStat.TargetType;
                targetId = cardStat.TargetId;
            }
            else
            {
                var skillCondition = Data.GetSkillCondition(skillConditionId);

                targetGroupType = skillCondition.TargetGroupType;
                targetType = skillCondition.TargetType;
                targetId = skillCondition.TargetId;
            }
        }

        public void DoDeadUnitCountPatternSchedule(PlayerType player, uint unitId)
        {
            var deadUnitCountConditions = _patternContainer.DeadUnitCountConditions;
            foreach (var condition in deadUnitCountConditions)
            {
                condition.UpdateCondition(player, unitId);
            }

            DoActionPattern();
        }

        public void DoHpCheckPatternSchedule(PlayerType player, uint unitId, int maxHp, int currentHp)
        {
            var hpCheckConditions = _patternContainer.HpCheckConditions;
            foreach (var condition in hpCheckConditions)
            {
                condition.UpdateCondition(player, unitId, maxHp, currentHp);
            }

            DoActionPattern();
        }

        public void DoGottenHitPatternSchedule(PlayerType player, uint unitId, AkaEnum.DamageReasonType reasonType)
        {
            var gottenAttackConditions = _patternContainer.GottenAttackConditions;
            foreach (var condition in gottenAttackConditions)
            {
                condition.UpdateCondition(player, unitId, reasonType);
            }

            DoActionPattern();
        }

        public void DoAttackPatternSchedule(PlayerType player , uint unitId)
        {
            var attackConditions = _patternContainer.AttackActionConditions;
            foreach (var condition in attackConditions)
            {
                condition.UpdateCondition(player, unitId);
            }
            DoActionPattern();
        }

        public void DoTimePatternSchedule()
        {
            var timeConditions = _patternContainer.TimeActionConditions;
            foreach (var condition in timeConditions)
            {
                condition.UpdateCondition();
            }
            DoActionPattern();
        }

        public void DoHasIgnitionPatternSchedule(PlayerType player, uint unitId, int stack)
        {
            var hasIgnitionConditions = _patternContainer.HasIgnitionConditions;
            foreach (var condition in hasIgnitionConditions)
            {
                condition.UpdateCondition(player, unitId, stack);
            }
            DoActionPattern();
        }

        public void DoSkillPatternSchedule(PlayerType player, uint unitId, Card card)
        {
            var didPattern = _patternContainer.PatternsOfMonster.Find(pattern => card is PatternCard && pattern.CardStatId == card.CardStatId && pattern.UnitId == unitId && pattern.PatternId == ((PatternCard)card).PatternId );
            didPattern?.DidAction();

            var skillConditions = _patternContainer.GetSkillConditions(player, unitId, card);
            foreach (var condition in skillConditions)
            {
                condition.UpdateCondition(player, unitId);
            }
            DoActionPattern();
        }


        public void ScheduleStart()
        {
            if (_patternContainer.HasTimeAction)
                _battle.GetElapsedTimer().Start();
        }

        public void SchedulePause()
        {
            _battle.GetElapsedTimer().Pause();
        }

        public void ScheduleRestart(int bulletTime)
        {
            if (_patternContainer.HasTimeAction)
                _battle.GetElapsedTimer().Restart(bulletTime);
        }

        public List<uint> GetPatternOfCardStatIdList()
        {
            return _patternContainer.PatternsOfMonster//.Where(pattern => pattern.Deactived == false)
                                                      .Where(pattern => false == (pattern is FlowPattern))
                                                      .Select(pattern => pattern.CardStatId)
                                                      .Distinct()
                                                      .ToList();
        }
    }

}
