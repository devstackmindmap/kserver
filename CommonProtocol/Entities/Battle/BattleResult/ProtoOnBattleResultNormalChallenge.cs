using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBattleResultNormalChallenge : ProtoOnBattleResult
    {
        [Key(6)]
        public ProtoOnStageLevelExit StageClearInfo; //Roguelike
    }
}
