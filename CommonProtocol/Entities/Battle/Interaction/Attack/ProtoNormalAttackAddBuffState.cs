using System.Collections.Generic;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoNormalAttackAddBuffState
    {
        [Key(0)]
        public List<uint> SkillOptionIds;

        [Key(1)]
        public List<BaseSkillProto> BuffStates;
    }
}