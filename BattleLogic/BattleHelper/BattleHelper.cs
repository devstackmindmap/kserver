using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleHelper
    {
        private Battle _battle;

        public IBattleRecorder BattleRecorder { get; }

        public BattleProgress BattleProgress { get; }

        public BattleTimer BattleTimer { get; }

        public BattleController BattleController { get; }
        

        public BattlePatternBehavior BattlePatternBehavior { get; }

        public BattleHelper(BattleProgress battleProgress,
                            BattleTimer battleTimer, 
                            BattleController battleController, 
                            BattlePatternBehavior battlePatternBehavior,
                            IBattleRecorder battleRecorder)
        {
            BattleProgress = battleProgress;
            BattleTimer = battleTimer;
            BattleController = battleController;
            BattlePatternBehavior = battlePatternBehavior;
            BattleRecorder = battleRecorder;
        }


        public void BattleControllerInitialize(Deck player1Deck, Deck player2Deck)
        {
            BattleController.BattleControllerInitialize(player1Deck, player2Deck);
        }

        public void BattleBehaviorIntialize(StagePatternContainer patternContainer)
        {
            BattlePatternBehavior.BattleBehaviorIntialize(patternContainer);
        }

        public void SetBattle(Battle battle)
        {
            _battle = battle;
            BattleRecorder.SetBattle(battle);
            BattleTimer.SetBattle(battle);
            BattlePatternBehavior.SetBattle(battle);
        }

        public void BattleStart()
        {
            BattleProgress.TimerStart();
            BattleTimer.Start();
            BattlePatternBehavior.ScheduleStart();
        }

        public void Pause()
        {
            BattleTimer.Pause();
            BattlePatternBehavior.SchedulePause();
        }

        public void Restart(int bulletTime)
        {
            BattleTimer.Restart(bulletTime);
            BattlePatternBehavior.ScheduleRestart(bulletTime);
        }

        public void RemoveUnit(PlayerType playerType, int unitPositionIndex, uint unitId)
        {
            BattleController.RemoveUnit(playerType, unitId);
        }

        public Dictionary<PlayerType, BattleCard> GetBattleCards()
        {
            return BattleController.GetBattleCards();
        }

        public void EnqueueAttack(BattleActionAttack battleActionAttack)
        {
            BattleProgress.EnqueueAttack(battleActionAttack);
        }

        public void EnqueueSequantialAttack(BattleActionAttack battleActionAttack)
        {
            BattleProgress.EnqueueSequantialAttack(battleActionAttack);
        }

        public void EnqueueBuffEnd(BattleActionBuffEnd buffEnd)
        {
            BattleProgress.EnqueueBuffEnd(buffEnd);
        }

        public void EnqueueUpdateUnitAttackTime(BattleActionUpdateUnitAttackTime action)
        {
            BattleProgress.EnqueueUpdateUnitAttackTime(action);
        }

        public void EnqueueAddElixir(BattleActionAddElixir action)
        {
            BattleProgress.EnqueueAddElixir(action);
        }

        public Dictionary<int, Card> GetHandCards(PlayerType playerType)
        {
            return BattleController.GetHandCards(playerType);
        }


        public uint? GetNexCardStatId(PlayerType playerType)
        {
            return BattleController.GetNexCardStatId(playerType);
        }

        public Queue<Card> GetWaitCards(PlayerType playerType)
        {
            return BattleController.GetWaitCards(playerType);
        }

        public void Send(BattleSendData data)
        {
            _battle.Send(data);
            BattleRecorder.EnqueueBehaviorForS2CRecord(data);
        }

    }
}
