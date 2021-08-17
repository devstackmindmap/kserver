using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressExtension : IBattleProgress
    {
        private BattleActionExtension _battleExtension;
        private bool _isDoing = false;
        public void Update()
        {
            if (_battleExtension == null)
                return;

            _battleExtension?.DoAction();
            _isDoing = false;
            _battleExtension = null;
        }

        public bool IsProgress()
        {
            return _isDoing;
        }

        public bool HasWork()
        {
            return _battleExtension != null;
        }

        public void EnqueueAction<T>(T action)
        {
            _isDoing = true;
            _battleExtension = action as BattleActionExtension;
        }
    }
}