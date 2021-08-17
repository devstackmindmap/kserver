using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities.Battle
{
    public class RankUserFactory
    {
        public static RankUser CreateRankUser(DBContext db, uint userId, RankType rankType, int changeRankPoint, int nextSeasonChangeRankPoint)
        {
            switch(rankType)
            {
                case RankType.AllUnitRankPoint:
                    return new RankUserKnightLeague(db, userId, rankType, changeRankPoint, nextSeasonChangeRankPoint);
                case RankType.AllUnitVirtualRankPoint:
                    return new RankUserVirtualLeague(db, userId, rankType, changeRankPoint);
                default:
                    throw new Exception("Wrong RankType");
            }
        }
    }
}
