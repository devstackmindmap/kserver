using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoChallengeStageList : BaseProtocol
    {
        [Key(1)]
        public uint CurrentSeason;

        [Key(2)]
        public List<ProtoChallengeStage> stages = new List<ProtoChallengeStage>();

        [Key(3)]
        public int TodayKnightLeagueWinCount;

        [Key(4)]
        public long NextChallengeStartDateTime;
    }
}
