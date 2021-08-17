using CommonProtocol;
using System.Collections.Generic;

namespace Common.Entities.InfusionBox
{
    public class InfusionBoxOpenInfo
    {
        public List<ProtoItemResult> ItemResults;
        public ProtoNewInfusionBox NewInfusionBox;
        public IDictionary<uint, int> Corrections;
    }
}
