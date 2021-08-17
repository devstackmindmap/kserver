using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventChallengeStageList : BaseProtocol
    {
        [Key(1)]
        public List<ProtoEventChallengeStage> stages = new List<ProtoEventChallengeStage>();
    }
}
