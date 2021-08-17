using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Season;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class RankUserVirtualLeague : RankUser
    {
        public RankUserVirtualLeague(DBContext db, uint userId, RankType rankType, int changeRankPoint) 
            : base(db, userId, rankType, changeRankPoint, 0)
        {
           
        }

        protected override async Task GetDbUserRankData()
        {
            if (_rankData != null)
                return;

            var query = new StringBuilder();
            query.Append("SELECT maxVirtualRankLevel, maxVirtualRankPoint, currentVirtualRankPoint FROM user_info WHERE userId=").Append(_userId.ToString()).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if (cursor.Read())
                {
                    _rankData = new ProtoRankData
                    {
                        MaxRankLevel = (uint)cursor["maxVirtualRankLevel"],
                        CurrentSeasonRankPoint = (int)cursor["currentVirtualRankPoint"],
                        MaxRankPoint = (int)cursor["maxVirtualRankPoint"],
                    };
                }
                else
                {
                    _rankData = new ProtoRankData
                    {
                        MaxRankLevel = 1,
                    };
                }
            }
            
        }
                     
        protected override async Task UpdateUserRankInfo(uint newRankLevel, int newRankPoint, int nextSeasonRankPoint, int maxRankPoint , int addVictoryCount , uint winsCount )
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO user_info (userid, maxVirtualRankLevel, maxVirtualRankPoint, currentVirtualRankPoint) VALUES (")
                 .Append(_userId.ToString()).Append(",")
                 .Append(newRankLevel.ToString()).Append(",")
                 .Append(maxRankPoint.ToString()).Append(",")
                 .Append(newRankPoint.ToString()).Append(") ON DUPLICATE KEY UPDATE  maxVirtualRankLevel=").Append(newRankLevel.ToString())
                 .Append(", maxVirtualRankPoint=").Append(maxRankPoint.ToString())
                 .Append(", currentVirtualRankPoint=").Append(newRankPoint.ToString())
                 .Append(";");
                
            await _db.ExecuteNonQueryAsync(query.ToString());
            
            Log.User.UserVirtualRank.Log(_userId, newRankLevel, newRankPoint);
        }
    }
}
