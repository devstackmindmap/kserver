using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserExp : BaseProtocol
    {
        [Key(1)]
        public uint Level;

        [Key(2)]
        public uint OldLevel;

        [Key(3)]
        public ulong GettingExp;

        [Key(4)]
        public ulong Exp;

        [Key(5)]
        public ulong OldExp;
    }
}
