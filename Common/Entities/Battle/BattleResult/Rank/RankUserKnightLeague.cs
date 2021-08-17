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
    public class RankUserKnightLeague : RankUser
    {
        public RankUserKnightLeague(DBContext accountDb, uint userId, RankType rankType, int changeRankPoint, int nextSeasonChangeRankPoint) 
            : base(accountDb, userId, rankType, changeRankPoint, nextSeasonChangeRankPoint)
        {
           
        }

        protected override async Task GetDbUserRankData()
        {
            if (_rankData != null)
                return;

            var query = new StringBuilder();
            query.Append("SELECT maxRankLevel, currentSeasonRankPoint, maxRankPoint, countryCode, wins FROM accounts WHERE userId=").Append(_userId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Check the Rank Logic");

                _rankData = new ProtoRankData { 
                    MaxRankLevel = (uint)cursor["maxRankLevel"],
                    CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                    MaxRankPoint = (int)cursor["maxRankPoint"],
                    Wins = (uint)cursor["wins"],
                };

                CountryCode = (string)cursor["countryCode"];
            }  
        }

        protected override async Task UpdateUserRankInfo(uint newRankLevel, int newRankPoint, int nextSeasonRankPoint, int maxRankPoint, int addVictoryCount, uint winsCount )
        {
            var query = new StringBuilder();
            query.Append("UPDATE accounts SET maxRankLevel=").Append(newRankLevel.ToString())
                .Append(", currentSeasonRankPoint=").Append(newRankPoint.ToString())
                .Append(", nextSeasonRankPoint=nextSeasonRankPoint+").Append(_nextSeasonChangeRankPoint.ToString())
                .Append(", maxRankPoint=").Append(maxRankPoint.ToString())
                .Append(", rankVictoryCount=rankVictoryCount+").Append(addVictoryCount.ToString())
                .Append(", wins=").Append(winsCount.ToString())
                .Append(" WHERE userId=").Append(_userId.ToString()).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
            Log.User.UserRank.Log(_userId, newRankLevel, newRankPoint);
        }

    }
}
