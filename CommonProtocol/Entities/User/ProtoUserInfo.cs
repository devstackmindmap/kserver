using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserInfo : BaseProtocol
    {
        [Key(1)]
        public List<ProtoMaterialInfo> MaterialInfoList = new List<ProtoMaterialInfo>();

        [Key(2)]
        public List<ProtoTermMaterialInfo> TermMaterialInfoList = new List<ProtoTermMaterialInfo>();

        [Key(3)]
        public ProtoUserExp LevelAndExp = new ProtoUserExp();
    }
}
