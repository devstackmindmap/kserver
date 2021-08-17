using System.Collections.Generic;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoHandCardCostDecrease : BaseSkillProto
    {
        [Key(3)]
        public List<int> HandIndex;
    }
}