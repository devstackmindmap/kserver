using System.Collections.Generic;
using AkaEnum;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoActionStatus
    {
        [Key(0)]
        public ActionStatusType ActionStatusType;

        [Key(1)]
        public uint ClassId;

        [Key(2)]
        public int Value;
    }
}
