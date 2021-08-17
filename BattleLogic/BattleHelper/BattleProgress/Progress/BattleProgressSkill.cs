using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressSkill : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionSkill> _skills = new ConcurrentQueue<BattleActionSkill>();
        
        private BattleActionSkillResult _result;

        public void Update()
        {
            if (_result?.IsDoing == true)
            {
                CheckBulletTime();
                return;
            }

            if (_skills.Count == 0)
                return;

            _result = null;
            if (_skills.TryDequeue(out var skill))
                _result = skill.DoAction() as BattleActionSkillResult;
        }

        public bool IsProgress()
        {
            return (_result?.IsDoing ?? false);
        }
        public bool HasWork()
        {
            return _skills.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _skills.Enqueue(action as BattleActionSkill);
        }

        private void CheckBulletTime()
        {
            if (DateTime.UtcNow < _result.CatchBattleProgressTime)
                return;
            
            _result.IsDoing = false;
            if (_result.BulletTime > 0)
                _result.BattleHelper.Restart(_result.BulletTime);
        }
    }
}