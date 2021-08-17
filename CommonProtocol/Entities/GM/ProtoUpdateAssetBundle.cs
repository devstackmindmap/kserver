using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUpdateAssetBundle : BaseProtocol
    {
        [Key(1)]
        public string Command;
    }
}
