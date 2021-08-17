using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class EventManager
    {
        private StringBuilder _query = new StringBuilder();
        private DBContext _accountDb;

        public EventManager(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task<List<ProtoEvent>> GetEventList()
        {
            _query.Clear();
            var utcNow = DateTime.UtcNow;
            _query.Append("SELECT startDateTime, endDateTime, eventType FROM _events " +
                "WHERE endDateTime >= '").Append(utcNow.ToTimeString())
                .Append("' AND startDateTime <= '").Append(utcNow.AddHours(10).ToTimeString()).Append("';");

            List<ProtoEvent> eventList = new List<ProtoEvent>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    eventList.Add(new ProtoEvent
                    {
                        EventType = (EventType)(int)cursor["eventType"],
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks
                    });
                }
            }
            return eventList;
        }

        public async Task<bool> IsInEventProgress(EventType eventType)
        {
            _query.Clear();
            var utcNow = DateTime.UtcNow.ToTimeString();
            _query.Append("SELECT startDateTime, endDateTime, eventType FROM _events " +
                "WHERE endDateTime >= '").Append(utcNow)
                .Append("' AND startDateTime <= '").Append(utcNow)
                .Append("' AND eventType=").Append((int)eventType).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }
    }
}
