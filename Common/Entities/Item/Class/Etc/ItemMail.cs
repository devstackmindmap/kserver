using AkaData;
using AkaDB.MySql;
using AkaUtility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class ItemMail : Item
    {
        private uint _mailId;
        public ItemMail(uint userId, uint mailId, DBContext db) : base(userId, 0, db)
        {
            _mailId = mailId;
        }

        public override async Task Get(string logCategory)
        {
            var mailData = Data.GetDataMail(_mailId);
            var utcNow = DateTime.UtcNow;
            var strUtcNow = utcNow.ToTimeString();
            var strUtcEnd = utcNow.AddHours(mailData.DurationHour).ToTimeString();

            var query = new StringBuilder();
            query.Append("INSERT IGNORE INTO user_mail_system (userId, systemMailId, isDeleted, startDateTime, endDateTime, IsRead) " +
                "VALUES (").Append(_userId).Append(",").Append(_mailId).Append(",").Append(" 0, '").Append(strUtcNow).Append("', '").Append(strUtcEnd).Append("', 0);");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
