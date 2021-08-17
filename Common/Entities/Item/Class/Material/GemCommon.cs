using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class GemCommon : Material
    {
        private int? _nowCount;
        private int? _nowPaidCount;

        public GemCommon(uint userId, string columnName, DBContext db, int count = 0) : base(userId, count, columnName, MaterialType.Gem, db)
        {
        }

        public override async Task<bool> IsEnoughCount()
        {
            await Select();
            _doneCountCheck = true;
            return _nowCount + _nowPaidCount >= _count;
        }

        public override async Task<int> GetRemainCount(int useCount = 0)
        {
            if (_doneCountCheck && useCount != 0)
                return _nowCount.Value - useCount;

            await Select();

            return _nowCount.Value + _nowPaidCount.Value - _count;
            
        }

        private async Task Select()
        {
            if (_nowCount.HasValue && _nowPaidCount.HasValue)
                return;

            var query = new StringBuilder();
            query.Append("SELECT ").Append(ColumnName.USER_GEM).Append(",").Append(ColumnName.USER_GEM_PAID)
                .Append(" FROM users WHERE userId=").Append(_strUserId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if(false == cursor.Read())
                    throw new Exception("DB Select Exception");

                _nowCount = (int)cursor[ColumnName.USER_GEM];
                _nowPaidCount = (int)cursor[ColumnName.USER_GEM_PAID];
            }
                
        }

        public override async Task Use(string logCategory)
        {
            await Select();

            var spendCount = GetCalculatedUseCount();
            var query = new StringBuilder();
            query.Append("UPDATE users SET ").Append(ColumnName.USER_GEM).Append(" = ")
                .Append(ColumnName.USER_GEM).Append(" - ").Append(spendCount.spendCount).Append(",")
                .Append(ColumnName.USER_GEM_PAID).Append(" = ")
                .Append(ColumnName.USER_GEM_PAID).Append(" - ").Append(spendCount.spendPaidCount)
                .Append(" WHERE userId = ").Append(_strUserId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            if (spendCount.spendCount != 0)
                Log.Item.MaterialUse(_strUserId, MaterialType.Gem, spendCount.spendCount, _nowCount.Value - spendCount.spendCount, logCategory);

            if (spendCount.spendPaidCount != 0)
                Log.Item.MaterialUse(_strUserId, MaterialType.GemPaid, spendCount.spendPaidCount, _nowPaidCount.Value - spendCount.spendPaidCount, logCategory);
        }

        private (int spendCount, int spendPaidCount) GetCalculatedUseCount()
        {

            if ( _nowCount.Value > _count)
                return (_count, 0);

            return (_nowCount.Value, _count- _nowCount.Value);
        }
    }
}
