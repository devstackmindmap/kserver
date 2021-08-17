using System.Collections.Generic;
using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellDisapearance : BaseSkillProto
    {
        [Key(3)]
        public List<SkillEffectType> RemovedEffectTypes;
    }
}