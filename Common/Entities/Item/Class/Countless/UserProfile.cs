using AkaData;
using AkaDB.MySql;
using AkaLogger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UserProfile : Item, ICountless
    {
        private uint _userProfileId;
        private string _strUserProfileId;
        private object _contentsType;

        public UserProfile(uint userId, uint userProfileId, DBContext db) : base(userId, 0, db)
        {
            _userProfileId = userProfileId;
            _strUserProfileId = _userProfileId.ToString();
        }

        public override async Task Get(string logCategory)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO profiles (`userId`, `id`) VALUES (")
                .Append(_strUserId).Append(", ").Append(_strUserProfileId).Append(")" +
                " ON DUPLICATE KEY UPDATE id = ").Append(_strUserProfileId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            Log.Item.ContentGet(_userId, _userProfileId, logCategory);
        }

        public async Task<bool> IsHave()
        {
            var query = new StringBuilder();
            query.Append("SELECT userId FROM profiles WHERE userId=").Append(_strUserId)
                .Append(" AND id=").Append(_strUserProfileId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        public bool IsValidData()
        {
            var userProfile = Data.GetProfileIcon(_userProfileId);
            return userProfile != null ? true : false;
        }
    }
}
