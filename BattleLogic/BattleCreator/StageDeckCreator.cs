using AkaData;
using AkaUtility;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{


    public class StageDeckCreator
    {
        private uint _stageRoundId;
        private uint _stageMonsterListId;
        private AkaEnum.Battle.PlayerType _playerType;
        private AkaEnum.Battle.PlayerType _enemyPlayerType;

        public StageDeckCreator(uint stageRoundId, AkaEnum.Battle.PlayerType playerType, AkaEnum.Battle.PlayerType enemyPlayerType)
        {
            _stageRoundId = stageRoundId;
            _playerType = playerType;
            _enemyPlayerType = enemyPlayerType;

            var stageRound = Data.GetStageRound(_stageRoundId);
            _stageMonsterListId = AkaRandom.Random.ChooseElementRandomlyInCount(stageRound.MonsterGroupIdList);
        }

        public Deck GetDeck(StagePatternContainer patternContainer)
        {
            return new AiDeck
            {
                Units = GetUnits(),
                Cards = GetCards(patternContainer)
            };
        }

        private void CreateUnitPattern(List<Pattern> inoutPatterns, uint unitId, uint patternId, bool activePattern)
        {
            if (inoutPatterns.Any(findPattern => findPattern.PatternId == patternId && findPattern.UnitId == unitId))
                return;

            var pattern = Data.GetMonsterPattern(patternId);
            var patternObject = CreatePattern(pattern, unitId, activePattern);
            var flowPatterns = CreateFlowPatternList(pattern, patternObject, activePattern);

            inoutPatterns.Add(patternObject);
            inoutPatterns.AddRange(flowPatterns);

            foreach (var flowPattern in flowPatterns)
            {
                CreateUnitPattern(inoutPatterns, unitId, flowPattern.NextPatternId, false);
            }
        }

        public StagePatternContainer GetPatternContainer()
        {
            var monsterIdList = Data.GetMonsterGroup(_stageMonsterListId);

            var patternsOfMonster = new List<Pattern>();
            monsterIdList.MonsterIdList
                .Where(monsterId => monsterId != 0)
                .Select(monsterId => Data.GetMonster(monsterId))
                .SelectMany(monster => monster.MonsterPatternIdList, (monster, monsterPatternId) => (unitId: monster.BaseUnitId, monsterPatternId: monsterPatternId))
                .All(monsterIdAndPatternId =>
                {

                    CreateUnitPattern(patternsOfMonster, monsterIdAndPatternId.unitId, monsterIdAndPatternId.monsterPatternId, true);
                    return true;
                });
            return new StagePatternContainer(_playerType, patternsOfMonster);
        }

        private Pattern CreatePattern(DataMonsterPattern pattern, uint unitId, bool activePattern)
        {
            var conditions = MonsterPatternCondition.FromData(unitId, pattern.MonsterPatternConditionIdList, _playerType, _enemyPlayerType);
            var activeConditions = MonsterPatternCondition.FromData(unitId, pattern.ActivePatternConditionIdList, _playerType, _enemyPlayerType);
            var deactiveConditions = MonsterPatternCondition.FromData(unitId, pattern.DeactivePatternConditionIdList, _playerType, _enemyPlayerType);

            var deactived = activeConditions.Count > 0 && activePattern == true ? true : !activePattern;
            if (activeConditions.Count == 0)
                activeConditions.Add(new BoolPatternCondition(true, unitId, _playerType, _enemyPlayerType));
            if (deactiveConditions.Count == 0)
                deactiveConditions.Add(new BoolPatternCondition(false, unitId, _playerType, _enemyPlayerType));

            return  new Pattern(pattern.RepeatCount)
            {
                PatternId = pattern.MonsterPatternId,
                CardStatId = pattern.CardStatId,
                Conditions = conditions,
                ActiveConditions = activeConditions,
                DeactiveConditions = deactiveConditions,
                CheckConditions = conditions.Concat(activeConditions).Concat(deactiveConditions),
                UnitId = unitId,
                Deactived = deactived
            };
        }
        private IEnumerable<FlowPattern> CreateFlowPatternList(DataMonsterPattern pattern, Pattern patternObject, bool activePattern )
        {
            return pattern.MonsterPatternFlowIdList.Select(flowid =>
            {
                var patternFlow = Data.GetMonsterPatternFlow(flowid);
                var flowConditionIds = patternFlow.MonsterPatternConditionIdList.ToArray();
                var flowConditions = MonsterPatternCondition.FromData(patternObject.UnitId, flowConditionIds, _playerType, _enemyPlayerType);

                return new FlowPattern()
                {
                    FlowPatternId = flowid,
                    Conditions = flowConditions,
                    UnitId = patternObject.UnitId,
                    ParentPattern = patternObject,
                    NextPatternId = patternFlow.TransMonsterPatternId,
                    Deactived = !activePattern
                };
            });
        }

        private Dictionary<int, Unit> GetUnits()
        {
            var monsterIdList = Data.GetMonsterGroup(_stageMonsterListId);

            Dictionary<int, Unit> units = new Dictionary<int, Unit>();

            for (int i=0; i < monsterIdList.MonsterIdList.Count; i++)
            {
                var monster = Data.GetMonster(monsterIdList.MonsterIdList[i]);
                units.Add(i, new Unit(Data.GetMonster(monster.BaseUnitId).BaseUnitId, 1, 0, i, monster.MonsterType));
            }
            //foreach (var monsterId in monsterIdList.MonsterIdList)
            //{
            //    var monster = Data.GetMonster(monsterId);
            //    units.Add(new Unit(Data.GetMonster(monster.BaseUnitId).BaseUnitId, 1, 0, i));
            //}

            //var units = monsterIdList.MonsterIdList
            //                    .Select((monsterId, index) => (monsterId: monsterId, index: index))
            //                    .Where(tuple => tuple.monsterId != 0)
            //                    .ToDictionary(tuple => tuple.index
            //                        , tuple => new Unit(Data.GetMonster(tuple.monsterId).BaseUnitId, 1, 0, tuple.index));
            return units;
        }

        private Queue<Card> GetCards(StagePatternContainer patternContainer)
        {

            var cardEnum = patternContainer.PatternsOfMonster
                .Where(pattern => (pattern is FlowPattern) == false)
                .Select(pattern =>
                {
                    var dataCardStat = Data.GetCardStat(pattern.CardStatId);
                    var dataCard = Data.GetCard(dataCardStat.CardId);
                    return new PatternCard(dataCard, dataCardStat, pattern.PatternId);
                });

            return new Queue<Card>(cardEnum);
        }
    }
}
