using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCurrentBuffInfo
    {
        [Key(0)]
        public uint SkillOptionId;

        [Key(1)]
        public long BuffStartTime;

        [Key(2)]
        public long BuffEndTime;

        [Key(3)]
        public int RemainCount;
    }
}
