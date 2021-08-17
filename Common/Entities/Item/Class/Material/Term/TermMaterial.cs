using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Challenge;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public abstract class TermMaterial : Item, IMaterial
    {
        private DBContext _accountDb;
        private string _columnName;
        protected bool _doneCountCheck = false;
        private int? _nowCount;
        private DateTime? _recentUpdateDateTime;
        private MaterialType _materialType;
        private TermMaterialType _termMaterialType;

        public int NowCount => _nowCount.Value;

        public TermMaterial(uint userId, int count, string columnName, MaterialType materialType, TermMaterialType termMaterialType,
            DBContext accountDb, DBContext userDb)
            : base(userId, count, userDb)
        {
            _accountDb = accountDb;
            _columnName = columnName;
            _materialType = materialType;
            _termMaterialType = termMaterialType;
        }

        public override async Task Get(string logCategory)
        {
            await GetNowCount();

            if (_count == 0)
                return;

            var query = new StringBuilder();
            var strUtcNow = DateTime.UtcNow.ToTimeString();
            query.Append("INSERT INTO term_materials (userId, termMaterialType, ").Append(_columnName)
                .Append(", recentUpdateDateTime) VALUES (").Append(_strUserId).Append(",")
                .Append((int)_termMaterialType).Append(",").Append(_strCount)
                .Append(", '").Append(strUtcNow).Append("') ON DUPLICATE KEY UPDATE ").Append(_columnName).Append(" = ")
                .Append(_nowCount + _count)
                .Append(", recentUpdateDateTime='").Append(strUtcNow)
                .Append("';");
            await _db.ExecuteNonQueryAsync(query.ToString());

            await GetNowCount();
            AkaLogger.Log.Item.MaterialGet(_strUserId, _materialType, _strCount, _nowCount.Value + _count, logCategory);
        }

        public virtual async Task Use(string logCategory)
        {
            await GetNowCount();
            var newCount = Math.Max(_nowCount.Value - _count, 0);
            var query = new StringBuilder();
            query.Append("UPDATE term_materials SET ").Append(_columnName).Append(" = ")
                .Append(newCount).Append(", recentUpdateDateTime='").Append(DateTime.UtcNow.ToTimeString())
                .Append("' WHERE userId = ").Append(_strUserId).Append(" AND termMaterialType=").Append((int)_termMaterialType).Append(";");
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
            query.Append("SELECT ").Append(_columnName)
                .Append(", recentUpdateDateTime FROM term_materials WHERE userId=").Append(_strUserId)
                .Append(" AND termMaterialType=").Append((int)_termMaterialType).Append(";");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                {
                    _nowCount = 0;
                }
                else
                {
                    _recentUpdateDateTime = (DateTime)cursor["recentUpdateDateTime"];
                    var eventChallengeManager = ChallengeFactory.CreateEventChallengeManager(_accountDb, null, _userId, 0, 0);
                    var eventInfo = await eventChallengeManager.GetEventInfo();

                    if (eventInfo.IsInEvent && eventInfo.StartDateTime > _recentUpdateDateTime)
                        _nowCount = 0;
                    else
                        _nowCount = (int)cursor[_columnName];
                }
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
