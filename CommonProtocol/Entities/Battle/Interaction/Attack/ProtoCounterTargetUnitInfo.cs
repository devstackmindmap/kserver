using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCounterTargetUnitInfo
    {
        [Key(0)]
        public ProtoTargetUnitInfo TargetUnitInfo;

        [Key(1)]
        public uint SkillOptionId;
    }
}