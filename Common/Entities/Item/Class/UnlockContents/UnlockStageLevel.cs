using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using AkaUtility;
using Common.Entities.Stage;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UnlockStageLevel : Item, IUnlockContents
    {
        private uint _stageLevelId;
        private StageUnlocker _stageUnlocker;

        public UnlockStageLevel(uint userId, uint classId, DBContext db) : base(userId, 0, db)
        {
            _stageLevelId = classId;
            _stageUnlocker = new StageUnlocker(db, userId, _stageLevelId);
        }

        public override async Task Get(string logCategory)
        {
            await Update(logCategory);
        }

        private IContents GetContentUnlocker()
        {
            if (IsValidData())
            {
                return _stageUnlocker;
            }
            return null;
        }

        private async Task<int> Update(string logCategory)
        {
            var contentsUnlocker = GetContentUnlocker();
            if (await contentsUnlocker.UnlockContents(_userId))
            {
                Log.Item.StageUnlock(_userId, _stageLevelId, logCategory);
                return 1;
            }
            return 0;
        }

        public async Task<bool> IsHave()
        {
            var query = new StringBuilder();
            query.Append("SELECT stageLevelId  FROM stage_levels WHERE userId =")
                 .Append(_strUserId)
                 .Append(" AND stageLevelId = ")
                 .Append(_stageLevelId.ToString() )
                 .Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        public bool IsValidData()
        {
            return null != Data.GetStageLevel(_stageLevelId);
        }
    }
}
