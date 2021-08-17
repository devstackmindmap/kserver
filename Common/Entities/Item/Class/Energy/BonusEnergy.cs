using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;

namespace Common.Entities.Item
{
    public class BonusEnergy : Item
    {
        public BonusEnergy(uint userId, int count, DBContext db) : base(userId, count, db)
        {
        }

        public override async Task Get(string logCategory)
        {
            if (_count == 0)
                return;

            var query = new StringBuilder();
            query.Append("UPDATE infusion_boxes SET userBonusEnergy = userBonusEnergy + ").Append(_count)
                .Append(" WHERE userId = ").Append(_userId).Append(" AND type = 1;");

            await _db.ExecuteNonQueryAsync(query.ToString());

            AkaLogger.Log.Item.BonusEnergyGet(_strUserId, _strCount, logCategory);
        }
    }
}
