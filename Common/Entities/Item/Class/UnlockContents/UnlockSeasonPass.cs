using AkaData;
using AkaDB.MySql;
using AkaLogger;
using Common.Pass;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class UnlockSeasonPass : Item, IUnlockContents
    {
        private uint _seasonPassId;

        public UnlockSeasonPass(uint userId, uint classId, DBContext db) : base(userId, 0, db)
        {
            _seasonPassId = classId;
        }

        public override async Task Get(string logCategory)
        {
            if (await Update(logCategory) == 1)
                await InsertSeasonPass();
        }

        private async Task<int> Update(string logCategory)
        {
            var query = new StringBuilder();
            query.Append("UPDATE user_info SET enablePasslist = CONCAT(enablePasslist,'/")
                 .Append(_seasonPassId.ToString()).Append("') WHERE userId = ")
                 .Append(_strUserId).Append(";");

            if (await _db.ExecuteNonQueryAsync(query.ToString()) > 0)
            {
                Log.Item.SeasonPassUnlock(_userId, _seasonPassId, logCategory);
                return 1;
            }
            return 0;
        }

        private async Task InsertSeasonPass()
        {
            var seasonPass = Data.GetSeasonPass(_seasonPassId);
            var query = new StringBuilder();
            query.Append("INSERT IGNORE INTO get_seasonpass (userId, seasonPassType, season) " +
                "VALUES (").Append(_userId).Append(",").Append((int)seasonPass.SeasonPassType)
                .Append(",").Append(seasonPass.Season).Append(");");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<bool> IsHave()
        {
            var seasonPassManager = new SeasonPassManager(_userId, 0, _db);
            var enablePassList = await seasonPassManager.GetEnablePassList();
            return enablePassList.Contains(_seasonPassId);
        }

        public bool IsValidData()
        {
            return Data.GetSeasonPass(_seasonPassId) != null;
        }
    }
}
