using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoElectricShock : BaseProtocol
    {
        [Key(1)]
        public ProtoTargetUnitInfo TargetUnitInfo;
    }
}