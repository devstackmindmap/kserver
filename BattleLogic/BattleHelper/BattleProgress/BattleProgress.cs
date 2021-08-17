using AkaEnum;
using AkaTimer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleLogic
{
    public class BattleProgress : IDisposable
    {
        private readonly List<IBattleProgress> _progresses = new List<IBattleProgress>()
        {
            new BattleProgressEnd(),
            new BattleProgressExtension(),
            new BattleProgressCardUse(),
            new BattleProgressElectricShock(),
            new BattleProgressMarkerShock(),
            new BattleProgressIgnitionBomb(),
            new BattleProgressShield(),
            new BattleProgressPoison(),
            new BattleProgressAddElixir(),
            new BattleProgressBuffEnd(),
            new BattleProgressAttack(),
            new BattleProgressSkill(),
            new BattleProgressUpdateUnitAttackTime(),
            new BattleProgressAttack()
        };

        private readonly Timer _progressTimer;
        private readonly object _lockObject = new object();

        private enum ProgressType
        {
            End,
            Extension,
            CardUse,
            ElectricShock,
            MarkerShock,
            IgnitionBomb,
            Shield,
            Poison,
            AddElixir,
            BuffEnd,
            SequantialAttack,
            Skill,
            UpdateUnitAttackTime,
            Attack
        }

        public BattleProgress(BattleTimer battleTimer)
        {
            _progressTimer = new Timer(10, true, BattleProgressTimerElapsed)
            {
                Name = "progress"
            };
        }

        public void Dispose()
        {
            _progressTimer.Dispose();
        }


        public void TimerStart()
        {
            _progressTimer.Start();
        }

        private void BattleProgressTimerElapsed()
        {
            if (!System.Threading.Monitor.TryEnter(_lockObject))
                return;
            try
            {
                bool didProcess = _progresses.Any(progress =>
                {
                    if (progress.IsProgress())
                    {
                        progress.Update();
                        return true;
                    }
                    return false;
                });

                if (!didProcess)
                {
                    var nextProgress = _progresses.FirstOrDefault(progress => progress.HasWork());
                    nextProgress?.Update();
                }
            }
            catch (Exception ex)
            {
                AkaLogger.Log.Debug.Exception("BattleProgress", ex);
                AkaLogger.Logger.Instance().Error("\t[BattleProgress Exception :" + ex.GetType().ToString() + ex.Message + ex.StackTrace);
            }
            finally
            {
                System.Threading.Monitor.Exit(_lockObject);
            }
        }

        public void EnqueueEnd(BattleActionEnd battleActionEnd)
        {
            _progresses[(int)ProgressType.End].EnqueueAction(battleActionEnd);
        }

        public void EnqueueExtension(BattleActionExtension battleActionExtension)
        {
            _progresses[(int)ProgressType.Extension].EnqueueAction(battleActionExtension);
        }

        public void EnqueueCardUse(BattleActionCardUse battleActionCardUse)
        {
            _progresses[(int)ProgressType.CardUse].EnqueueAction(battleActionCardUse);
        }


        public void EnqueueElectricShock(BattleActionElectricShock action)
        {
            _progresses[(int)ProgressType.ElectricShock].EnqueueAction(action);
        }

        public void EnqueueMarkerShock(BattleActionMarkerShock action)
        {
            _progresses[(int)ProgressType.MarkerShock].EnqueueAction(action);
        }

        public void EnqueueIgnitionBomb(BattleActionIgnitionBomb action)
        {
            _progresses[(int)ProgressType.IgnitionBomb].EnqueueAction(action);
        }

        public void EnqueueAttack(BattleActionAttack battleActionAttack)
        {
            _progresses[(int)ProgressType.Attack].EnqueueAction(battleActionAttack);
            S2CManager.SendEnqueueState(battleActionAttack, BattleInteractionType.NormalAttack);
        }

        public void EnqueueSequantialAttack(BattleActionAttack battleActionAttack)
        {
            _progresses[(int)ProgressType.SequantialAttack].EnqueueAction(battleActionAttack);
            S2CManager.SendEnqueueState(battleActionAttack, BattleInteractionType.NormalAttack);
        }

        public void EnqueueSkill(BattleActionSkill battleActionSkill)
        {
            _progresses[(int)ProgressType.Skill].EnqueueAction(battleActionSkill);
            S2CManager.SendEnqueueState(battleActionSkill, BattleInteractionType.Skill);
        }

        public void EnqueuePassive(BattleActionSkill battleActionSkill)
        {
            _progresses[(int)ProgressType.Skill].EnqueueAction(battleActionSkill);
            S2CManager.SendEnqueueState(battleActionSkill, BattleInteractionType.Passive);
        }

        public void EnqueueBuffEnd(BattleActionBuffEnd buffEnd)
        {
            _progresses[(int)ProgressType.BuffEnd].EnqueueAction(buffEnd);
        }

        public void EnqueueShield(UnitShieldOld unitShield)
        {
            _progresses[(int)ProgressType.Shield].EnqueueAction(unitShield);
        }

        public void EnqueuePoison(UnitPoison unitShield)
        {
            _progresses[(int)ProgressType.Poison].EnqueueAction(unitShield);
        }

        public void EnqueueUpdateUnitAttackTime(BattleActionUpdateUnitAttackTime action)
        {
            _progresses[(int)ProgressType.UpdateUnitAttackTime].EnqueueAction(action);
        }

        public void EnqueueAddElixir(BattleActionAddElixir addElixir)
        {
            _progresses[(int)ProgressType.AddElixir].EnqueueAction(addElixir);
        }
    }
}
