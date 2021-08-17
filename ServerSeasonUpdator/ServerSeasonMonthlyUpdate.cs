using AkaDB.MySql;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;
using AkaData;

namespace ServerSeasonUpdator
{
    public class ServerSeasonMonthlyUpdate
    {
        private DBContext _accountDb;

        public ServerSeasonMonthlyUpdate(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task Run()
        {
            await SeasonUpdate(
                (int)ServerCommonTable.SeasonPass);
        }

        private async Task SeasonUpdate(int commonId)
        {
            var cursor = _accountDb.ExecuteReaderAsync(
                "SELECT commonValue, commonStartDateTime, commonNextStartDateTime " +
                "FROM _common WHERE commonId= " + commonId + ";").Result;
            if (cursor.Read())
            {
                var currentSeason = (uint)cursor["commonValue"];
                var nextSeasonStartDateTime = (DateTime)cursor["commonNextStartDateTime"];
                var seasonStartDateTime = (DateTime)cursor["commonStartDateTime"];

                if (IsNeedSeasonChange(nextSeasonStartDateTime))
                {
                    await SeasonUpdate(commonId, currentSeason, nextSeasonStartDateTime, seasonStartDateTime);
                }

            }
            else
                throw new Exception("Check the _common table");
        }

        private bool IsNeedSeasonChange(DateTime nextSeasonStartDateTime)
        {
            var utcNowMonth = DateTime.UtcNow
                .AddHours(-(int)Data.GetConstant(DataConstantType.SEASON_PASS_MONTHLY_BASE_HOUR).Value)
                .Month;
            if (nextSeasonStartDateTime.Month == 1 && (utcNowMonth == 12))
                return true;
            else if (nextSeasonStartDateTime.Month - 1 == utcNowMonth)
                return true;

            return false;
        }

        private async Task SeasonUpdate(int commonId, uint currentSeason, 
            DateTime nextSeasonStartDateTime, DateTime seasonStartDateTime)
        {
            var newCurrentSeason = currentSeason + 1;
            var AddMonthNextSeasonStartDateTime
                = nextSeasonStartDateTime.AddMonths(1);

            var newNextSeasonStartDateTime = new DateTime(
                AddMonthNextSeasonStartDateTime.Year, 
                AddMonthNextSeasonStartDateTime.Month, 
                1,
                (int)Data.GetConstant(DataConstantType.SEASON_PASS_MONTHLY_BASE_HOUR).Value, 
                0, 0);

            StringBuilder query = new StringBuilder();
            query.Append("UPDATE _common SET commonValue =").Append(newCurrentSeason)
                .Append(", commonBeforeStartDateTime ='").Append(seasonStartDateTime.ToTimeString()).Append("'")
                .Append(", commonStartDateTime ='").Append(nextSeasonStartDateTime.ToTimeString()).Append("'")
                .Append(", commonNextStartDateTime ='").Append(newNextSeasonStartDateTime.ToTimeString()).Append("'")
                .Append(" WHERE commonId = ").Append(commonId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
