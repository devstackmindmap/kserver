using System;
using AkaEnum.Battle;
using AkaData;
using AkaUtility;

namespace BattleLogic
{
    public class BattleFactory
    {
        private readonly BattleEnviroment _enviroment;
        private DataContentsConstant Enviroment => _enviroment.Enviroment;
        private BattleFactory() { }

        private BattleFactory(BattleType battleType)
        {
            _enviroment = new BattleEnviroment(battleType);
        }

        public static Battle CreateBattle(BattleType battleType, int battleNum, uint stageRoundId)
        {
            var factory = new BattleFactory(battleType);

            Battle battle = null;
            switch (factory.Enviroment?.BattleType)
            {
                case BattleType.FriendlyBattle:
                case BattleType.LeagueBattle:
                case BattleType.LeagueBattleAi:
                case BattleType.PracticeBattle:
                case BattleType.VirtualLeagueBattle:
                case BattleType.Challenge:
                case BattleType.EventChallenge:
                    battle = new CommonPvPBattle(battleNum, factory._enviroment);
                    break;
                case BattleType.AkasicRecode_RogueLike:
                case BattleType.AkasicRecode_UserDeck:
                case BattleType.Tutorial:
                    battle = new RoguelikeBattle(battleNum, factory._enviroment);
                    break;
                case BattleType.Assault_Arena_High:
                case BattleType.Assault_Arena_Low:
                case BattleType.DoubleElixir:
                case BattleType.ThripleElixir:
                case BattleType.KillTheOneUnit:
                case BattleType.KillTheLeader:
                case BattleType.DamageStrike:
                case BattleType.Max20Elixir:
                default:
                    throw new Exception("Invalid Battle Type");
            }

            battle.SetBattleEndCondition(factory.CreateBattleEndCondition(stageRoundId));
            battle.SetDataManager(factory.CreateDataManager());
            battle.SetBattleHelper(factory.CreateBattleHelper());

            return battle;
        }

        private BattleHelper CreateBattleHelper()
        {
            var battleTimer = new BattleTimer();
            var battleProgress = new BattleProgress(battleTimer);
            var battleController = new BattleController();
            var battlePatternBehavior = new BattlePatternBehavior();
            var battleRecorder = CreateBattleRecorder();

            return new BattleHelper(battleProgress, battleTimer, battleController, battlePatternBehavior, battleRecorder);
        }

        private IBattleEndCondition CreateBattleEndCondition(uint stageRoundId)
        {
            var dataStageRound = Data.GetStageRound(stageRoundId);
            if (dataStageRound == null)
                return new NormalBattleEndCondition();

            switch (dataStageRound.EndConditionType)
            {
                case BattleEndConditionType.NormalBattleEnd:
                    return new NormalBattleEndCondition();
                case BattleEndConditionType.KillTheOneUnitEnd:
                    return new KillOneUnitEndCondition();
                case BattleEndConditionType.KillTheLeaderUnitEnd:
                    return new KillLeaderUnitEndCondition();
                case BattleEndConditionType.DamageStrikeEnd:
                    return new DamageStrikeEndCondition();
                case BattleEndConditionType.KillAllUnitEnd:
                    return new KillAllUnitEnd();
                default:
                    throw new Exception("Invalid Mode Type for DataFillManager");
            }
        }

        private DataFillManager CreateDataManager()
        {
            switch (Enviroment.BattleType)
            {
                case BattleType.LeagueBattle:
                case BattleType.FriendlyBattle:
                    return new HiDataFillManager();
                case BattleType.LeagueBattleAi:
                case BattleType.AkasicRecode_RogueLike:
                case BattleType.AkasicRecode_UserDeck:
                case BattleType.Tutorial:
                case BattleType.PracticeBattle:
                case BattleType.VirtualLeagueBattle:
                case BattleType.Challenge:
                case BattleType.EventChallenge:
                    return new AiDataFillManager();
                case BattleType.Assault_Arena_High:
                case BattleType.Assault_Arena_Low:
                case BattleType.DoubleElixir:
                case BattleType.ThripleElixir:
                case BattleType.KillTheOneUnit:
                case BattleType.KillTheLeader:
                case BattleType.DamageStrike:
                case BattleType.Max20Elixir:
                default:
                    throw new Exception("Invalid Mode Type for DataFillManager");
            }
        }

        private IBattleRecorder CreateBattleRecorder()
        {
            if (Enviroment.CanRecordBattle)
            {
                if (Enviroment.BattleType.In(BattleType.LeagueBattle, BattleType.LeagueBattleAi))
                    return new BattleRecorderRank();
                else
                    return new BattleRecorder();
            }
            else
            {
                return new BattleEmptyRecorder();
            }
        }
    }
}
