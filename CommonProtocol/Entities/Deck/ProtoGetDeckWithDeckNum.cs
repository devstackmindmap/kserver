using AkaEnum;
using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetDeckWithDeckNum : BaseProtocol
    {
        [Key(1)]
        public  List<KeyValuePair<uint,byte>> UserIdAndDeckNums;

        [Key(2)]
        public ModeType ModeType;

        [Key(3)]
        public BattleType BattleType;

    //    [Key(4)]
    //    public byte DeckNum;
    }
}
