
using System;

namespace Common.Entities.Season
{
    public class ServerSeasonInfo
    {
        public uint CurrentSeason;
        public DateTime NextSeasonStartDateTime;
        public DateTime CurrentSeasonStartDateTime;
    }
    
    public class ServerSeasonInfoWithSeasonYearNum
    {
        public uint CurrentSeason;
        public DateTime NextSeasonStartDateTime;
        public DateTime CurrentSeasonStartDateTime;
        public int SeasonYear;
        public uint SeasonYearNum;
    }
}
