using AkaDB.MySql;
using AkaUtility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class PushKeyManager
    {
        private DBContext _userDb;
        private uint _userId;
        private string _pushKey;
        private byte _pushAgree;
        private byte _nightPushAgree;

        public PushKeyManager(DBContext userDb, uint userId, string pushKey)
        {
            _userDb = userDb;
            _userId = userId;
            _pushKey = pushKey;
        }

        public PushKeyManager(DBContext userDb, uint userId)
        {
            _userDb = userDb;
            _userId = userId;
        }

        public PushKeyManager(DBContext userDb, uint userId, byte isPushAgree)
        {
            _userDb = userDb;
            _userId = userId;
            _pushAgree = isPushAgree;
        }

        public PushKeyManager(DBContext userDb, uint userId, string pushKey, byte isPushAgree, byte isNightPushAgree)
        {
            _userDb = userDb;
            _userId = userId;
            _pushKey = pushKey;
            _pushAgree = isPushAgree;
            _nightPushAgree = isNightPushAgree;
        }

        public async Task PushKeyInit()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("INSERT INTO pushkeys (userId, `pushKey`, pushAgree, pushAgreeDatetime, " +
                "nightPushAgree, nightPushAgreeDateTime, lastLoginDate , termsAgree, termsAgreeDatetime) " +
                "VALUE(").Append(_userId).Append(", '").Append(_pushKey).Append("', ").Append(_pushAgree).Append(", '")
                .Append(strUtcNow).Append("', ").Append(_nightPushAgree).Append(", '")
                .Append(strUtcNow).Append("', '").Append(strUtcNow)
                .Append("', 1, '").Append(strUtcNow).Append("') " +
                "ON DUPLICATE KEY UPDATE pushKey = '").Append(_pushKey)
                .Append("', pushAgree = ").Append(_pushAgree).Append(", pushAgreeDatetime ='").Append(strUtcNow)
                .Append("', nightPushAgree = ").Append(_nightPushAgree)
                .Append(", nightPushAgreeDateTime = '").Append(strUtcNow)
                .Append("', lastLoginDate = '").Append(strUtcNow)
                .Append("', termsAgree = 1, termsAgreeDatetime = '").Append(strUtcNow)
                .Append("';");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task UpdatePushAgree()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("UPDATE pushkeys SET pushAgree = ").Append(_pushAgree)
                .Append(", pushAgreeDatetime = '").Append(strUtcNow)
                .Append("' WHERE userId = ").Append(_userId).Append(";");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task UpdateNightPushAgree()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("UPDATE pushkeys SET nightPushAgree = ").Append(_pushAgree)
                .Append(", nightPushAgreeDateTime = '").Append(strUtcNow)
                .Append("' WHERE userId = ").Append(_userId).Append(";");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task UpdateLoginDateTime()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("UPDATE pushkeys SET lastLoginDate = '").Append(strUtcNow)
                .Append("' WHERE userId = ").Append(_userId).Append(";");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task UpdatePushKey()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("UPDATE pushkeys SET pushKey = '").Append(_pushKey)
                .Append("' WHERE userId = ").Append(_userId).Append(";");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task UpdateTermsAgree()
        {
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();

            query.Append("UPDATE pushkeys SET termsAgree = 1 WHERE userId = ").Append(_userId.ToString()).Append(";");
            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<bool> IsAgreeTerms()
        {
            var query = new StringBuilder();
            query.Append("SELECT termsAgree FROM pushkeys  WHERE userId = ").Append(_userId.ToString()).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                if (cursor.Read())
                    return 1 == (UInt64)cursor["termsAgree"];
            }
            return false;
        }
    }
}
