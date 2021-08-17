using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSkinPutOn : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint UnitId;

        [Key(3)]
        public uint SkinId; //If PutOff, input 0.
    }
}
