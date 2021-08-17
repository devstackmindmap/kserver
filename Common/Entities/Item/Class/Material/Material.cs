using AkaDB.MySql;
using AkaEnum;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public abstract class Material : Item, IMaterial
    {
        private string _columnName;
        protected bool _doneCountCheck = false;
        private int? _nowCount;
        private MaterialType _materialType;

        public int NowCount => _nowCount.Value;

        public Material(uint userId, int count, string columnName, MaterialType materialType, DBContext db) : base(userId, count, db)
        {
            _columnName = columnName;
            _materialType = materialType;
        }

        public override async Task Get(string logCategory)
        {
            await GetNowCount();

            if (_count == 0)
                return;

            var query = new StringBuilder();
            query.Append("INSERT INTO users (userId, ").Append(_columnName)
                .Append(") VALUES (").Append(_strUserId).Append(", ").Append(_strCount)
                .Append(") ON DUPLICATE KEY UPDATE ").Append(_columnName).Append(" = ").Append(_columnName)
                .Append(" + ").Append(_strCount).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            await GetNowCount();
            AkaLogger.Log.Item.MaterialGet(_strUserId, _materialType, _strCount, _nowCount.Value + _count, logCategory);
        }

        public virtual async Task Use(string logCategory)
        {
            await GetNowCount();

            var query = new StringBuilder();
            query.Append("UPDATE users SET ").Append(_columnName).Append(" = ").Append(_columnName).Append(" - ").Append(_strCount)
                .Append(" WHERE userId = ").Append(_strUserId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            AkaLogger.Log.Item.MaterialUse(_strUserId, _materialType, _count, _nowCount.Value - _count, logCategory);
        }

        public virtual async Task<bool> IsEnoughCount()
        {
            await GetNowCount();

            _doneCountCheck = true;
            if (_nowCount < _count)
            {
                return false;
            }
            return true;    
        }

        private async Task GetNowCount()
        {
            if (_nowCount.HasValue)
                return;

            var query = new StringBuilder();
            query.Append("SELECT ").Append(_columnName).Append(" FROM users WHERE userId=").Append(_strUserId).Append(";");
            
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("DB Select Exception");

                _nowCount = (int)cursor[_columnName];
            }
        }

        public virtual async Task<int> GetRemainCount(int useCount = 0)
        {
            await GetNowCount();
            if (_doneCountCheck && useCount != 0)
                return _nowCount.Value - useCount;

            return _nowCount.Value - _count;
        }

        public MaterialType GetMaterialType()
        {
            return _materialType;
        }

        public (string columnName, int defaultValue) GetColumnWithValue()
        {
            return (columnName: _columnName, defaultValue: _count);
        }
    }
}
