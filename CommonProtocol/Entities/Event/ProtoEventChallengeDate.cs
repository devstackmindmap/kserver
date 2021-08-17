using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventChallengeDate
    {
        [Key(0)]
        public uint ChallengeEventId;

        [Key(1)]
        public uint ChallengeEventNum;

        [Key(2)]
        public long StartDateTime;

        [Key(3)]
        public long EndDateTime;
    }
}
