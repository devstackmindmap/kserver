using AkaEnum.Battle;
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetBattleRecord : BaseProtocol
    {
        [Key(1)]
        public byte[] ArchivedData;
    }
}