using AkaData;
using CommonProtocol;
using System;

namespace BattleLogic
{
    public interface ISpellSkill
    {
        BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical);
        ISpellSkill Clone();
    }
}
