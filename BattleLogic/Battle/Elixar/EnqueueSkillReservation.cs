using CommonProtocol;
using System;

namespace BattleLogic
{
    class EnqueueSkillReservation
    {
        private Action<CardUseActionData> _enqueueSkillAction;
        private CardUseActionData _cardUseActionData;
        private ElixirValue _elixirValue;

        public EnqueueSkillReservation(ElixirValue elixirValue)
        {
            _elixirValue = elixirValue;
        }

        public void Reservation(Action<CardUseActionData> enqueueSkillAction, CardUseActionData cardUseActionData)
        {
            _enqueueSkillAction = enqueueSkillAction;
            _cardUseActionData = cardUseActionData;
        }

        public bool IsDoReservation()
        {
            return HasReservation() && _elixirValue.CurrentElixir >= 0;
        }

        public bool HasReservation()
        {
            return _enqueueSkillAction != null;
        }

        public void EnqueueSkill()
        {
            _enqueueSkillAction(_cardUseActionData);
            _enqueueSkillAction = null;
            _cardUseActionData = null;
        }
    }
}
