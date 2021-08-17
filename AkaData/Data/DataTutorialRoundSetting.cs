using System.Collections.Generic;

namespace AkaData
{
    public class DataTutorialRoundSetting
    {
        public uint Round { get; set; }
        public List<uint> UnitIdListOnRound { get; set; }
        public List<uint> CardIdListOnRound { get; set; }
        public uint StageRoundId { get; set; }
    }
}
