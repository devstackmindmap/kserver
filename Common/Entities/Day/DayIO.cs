
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AkaUtility;
using AkaEnum.Battle;
using AkaSerializer;
using System;
using System.Text;
using AkaData;
using System.Data.Common;
using Common.Entities.Reward;
using Common.Pass;

namespace Common.UserInfo
{
    public class DayIO
    {
        private DBContext _db;
        private uint _userId;

        private bool _needDateUpdate;

        public int DailyQuestRefreshCount { get; set; }
        public int DailyQuestAddcount { get; set; }

        public DayIO(uint userId, DBContext db)
        {
            _db = db;
            _userId = userId;
            _needDateUpdate = true;
        }

        public DayIO()
        {

        }


        public async Task Select()
        {
            var query = new StringBuilder();
            query.Append("SELECT  lastRefreshDate, dailyQuestRefreshCount, dailyQuestAddcount FROM user_info WHERE userid = ")
                 .Append(_userId)
                 .Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                Select(cursor);
            }
        }

        public void Select(DbDataReader cursor, bool doReadCursor = true)
        {
            if (doReadCursor && cursor.Read() == false)
                return;

            var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var utcnow = DateTime.UtcNow.AddHours(-refreshBaseHour);
            var baseDate = new DateTime(utcnow.Year, utcnow.Month, utcnow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);
            var lastRefreshDate = new DateTime(((DateTime)cursor["lastRefreshDate"]).Ticks, DateTimeKind.Utc);
            if ( (lastRefreshDate - baseDate).TotalDays >= 0)
            {
                _needDateUpdate = false;
                DailyQuestRefreshCount = (int)cursor["dailyQuestRefreshCount"];
                DailyQuestAddcount = (int)cursor["dailyQuestAddcount"];
            }
            else
            {
                DailyQuestAddcount = - (int)(baseDate - lastRefreshDate).TotalDays;
            }
        }

        public async Task Update()
        {
            var query = new StringBuilder();
            query.Append("UPDATE user_info SET dailyQuestRefreshCount = ").Append(DailyQuestRefreshCount)
                 .Append(", dailyQuestAddcount = ").Append(DailyQuestAddcount);
            if (_needDateUpdate)
            {
                query.Append(", lastRefreshDate = '").Append(DateTime.UtcNow.ToTimeString()).Append("'");
            }
            query.Append(" WHERE userid = ").Append(_userId).Append(";");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }


    }
}
