using AkaData;
using AkaEnum;

namespace Common.Entities.Battle
{
    public class RankSeason
    {
        public static int GetNextSeasonRankPoint(int newCurrentSeasonRankPoint)
        {
            var baseMedal = (int)Data.GetConstant(DataConstantType.LEAGUE_SEASON_SOFT_RESET_BASE_MEDAL).Value;
            if (newCurrentSeasonRankPoint > baseMedal)
            {
                return (int)(newCurrentSeasonRankPoint - (newCurrentSeasonRankPoint - baseMedal) * (Data.GetConstant(DataConstantType.LEAGUE_SEASON_SOFT_RESET_REDUCE_RATE).Value / 100));
            }
            return newCurrentSeasonRankPoint;
        }

        public static uint GetUnitNewCurrentLevel(uint oldCurrentLevel, int newRankPoint)
        {
            var newCurrentLevel = oldCurrentLevel;
            while (true)
            {
                if (newCurrentLevel == 1)
                    return newCurrentLevel;
                
                if (Data.GetUnitRankPoint(newCurrentLevel).NeedRankPointForNextLevelUp > newRankPoint &&
                    Data.GetUnitRankPoint(newCurrentLevel - 1).NeedRankPointForNextLevelUp <= newRankPoint)
                {
                    return newCurrentLevel;
                }

                newCurrentLevel--;
            }
        }
    }
}
