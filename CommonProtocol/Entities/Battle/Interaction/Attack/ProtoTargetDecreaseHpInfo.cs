using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTargetDecreaseHpInfo
    {
        [Key(0)]
        public int Damage;

        [Key(1)]
        public ProtoTargetUnitInfo OfferingTarget;
    }
}