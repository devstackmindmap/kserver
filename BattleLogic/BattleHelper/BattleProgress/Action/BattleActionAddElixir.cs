using AkaEnum;
using System;

namespace BattleLogic
{
    public class BattleActionAddElixir : BattleAction
    {
        private readonly int _addElixir;
        private readonly DateTime _recentAddTime;

        public BattleActionAddElixir(Unit attacker, int addElixir, DateTime recentAddTime) : base(attacker)
        {
            _addElixir = addElixir;
            _recentAddTime = recentAddTime;
        }

        public override BattleActionResult DoAction()
        {
            if (IsEnd())
            {
                Attacker.AddElixirFactoryEnd();
            }
            else
            {
                Attacker.MyPlayer.AddElixir(_addElixir);
                S2CManager.SendAddElixir(Attacker, _addElixir);
            }

            return new BattleActionResult() { IsDoing = true };
        }

        private bool IsEnd()
        {
            var buff = Attacker.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_ELIXIR_FACTORY);
            if (buff == null)
                return true;

            if (buff.EndDateTime < _recentAddTime)
            {
                Attacker.RemoveConditionBuff(SkillEffectType.BUFF_STATE_ELIXIR_FACTORY);
                return true;
            }

            return false;
        }
    }
}