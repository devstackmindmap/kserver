using System.Collections.Generic;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSkillOptionResult
    {
        [Key(0)]
        public List<BaseSkillProto> SkillTargetResults;

        [Key(1)]
        public uint SkillOptionId;
    }
}