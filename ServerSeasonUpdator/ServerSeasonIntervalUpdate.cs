using AkaDB.MySql;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;

namespace ServerSeasonUpdator
{
    public class ServerSeasonIntervalUpdate
    {
        private DBContext _accountDb;

        public ServerSeasonIntervalUpdate(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task Run(ServerCommonTable serverCommonTable, DataConstantType intervalMinuteConstantType)
        {
            await SeasonUpdate(
                (int)serverCommonTable,
                AkaData.Data.GetConstant(intervalMinuteConstantType).Value);
        }

        private async Task SeasonUpdate(int commonId, double intervalMinute)
        {
            var cursor = _accountDb.ExecuteReaderAsync(
                "SELECT commonValue, commonValue2, commonValue3, commonValue4 " +
                ", commonStartDateTime, commonNextStartDateTime " +
                "FROM _common WHERE commonId= " + commonId + ";").Result;
            if (cursor.Read())
            {
                var currentSeason = (uint)cursor["commonValue"];
                var seasonYear = (int)(uint)cursor["commonValue2"];
                var seasonYearNum = (uint)cursor["commonValue3"];
                var baseCurrentSeason = (uint)cursor["commonValue4"];
                var nextSeasonStartDateTime = (DateTime)cursor["commonNextStartDateTime"];
                var seasonStartDateTime = (DateTime)cursor["commonStartDateTime"];

                if (IsNeedSeasonChange(nextSeasonStartDateTime, intervalMinute))
                {
                    await SeasonUpdate(commonId, currentSeason, seasonYear, seasonYearNum, baseCurrentSeason,
                        nextSeasonStartDateTime, seasonStartDateTime, intervalMinute);
                }

            }
            else
                throw new Exception("Check the _common table");
        }

        private bool IsNeedSeasonChange(DateTime nextSeasonStartDateTime, double intervalMinute)
        {
            return DateTime.UtcNow.AddMinutes(intervalMinute)
            >= nextSeasonStartDateTime;
        }

        private async Task SeasonUpdate(int commonId, uint currentSeason, int seasonYear, uint seasonYearNum, 
            uint baseCurrentSeason, DateTime nextSeasonStartDateTime, DateTime seasonStartDateTime, double intervalMinute)
        {
            var newSeasonYearInfo = GetSeasonYearNum(seasonYear, seasonYearNum);

            var newCurrentSeason = currentSeason + 1;
            var newNextSeasonStartDateTime 
                = nextSeasonStartDateTime.AddMinutes(intervalMinute);

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE _common SET commonValue =").Append(newCurrentSeason)
                .Append(", commonValue2 = ").Append(newSeasonYearInfo.newSeasonYear)
                .Append(", commonValue3 = ").Append(newSeasonYearInfo.newSeasonYearNum)
                .Append(", commonValue4 = ").Append(baseCurrentSeason + 1)
                .Append(", commonValue5 = ").Append(seasonYear)
                .Append(", commonValue6 = ").Append(seasonYearNum)
                .Append(", commonBeforeStartDateTime ='").Append(seasonStartDateTime.ToTimeString()).Append("'")
                .Append(", commonStartDateTime ='").Append(nextSeasonStartDateTime.ToTimeString()).Append("'")
                .Append(", commonNextStartDateTime ='").Append(newNextSeasonStartDateTime.ToTimeString()).Append("'")
                .Append(" WHERE commonId = ").Append(commonId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }
        
        private (int newSeasonYear, uint newSeasonYearNum) GetSeasonYearNum(int seasonYear, uint seasonYearNum)
        {
            var utcNowYear = DateTime.UtcNow.Year;

            if (utcNowYear != seasonYear)
                return (utcNowYear, 1);
            else
                return (utcNowYear, seasonYearNum + 1);
        }
    }
}
