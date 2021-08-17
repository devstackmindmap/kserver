using System.Collections.Generic;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellIncreaseConditionTime : BaseSkillProto
    {
        [Key(3)]
        public List<ProtoCurrentBuffInfo> Buffs;
    }
}