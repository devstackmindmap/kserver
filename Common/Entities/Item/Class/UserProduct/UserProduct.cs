using System;
using System.Text;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;

namespace Common.Entities.Item
{
    public class UserProduct : Item
    {
        private uint _productId;
        public UserProduct(uint userId, uint classId, int count, DBContext db) : base(userId, count, db)
        {
            _productId = classId;
            //count is ProductTableType
        }

        public override async Task Get(string logCategory)
        {
            var productTableType = (ProductTableType)_count;
            var durationHour = GetSaleDurationHour(productTableType);
            var startDateTime = DateTime.UtcNow;
            var strEndDateTime = startDateTime.AddHours(durationHour).ToTimeString();
            var strStartDateTime = startDateTime.ToTimeString();

            var query = new StringBuilder();
            query.Append("INSERT INTO products_user (userId, startDateTime, endDateTime, productId, productTableType) " +
                "VALUES (").Append(_userId).Append(",'").Append(strStartDateTime).Append("','")
                .Append(strEndDateTime).Append("',").Append(_productId).Append(",").Append(_count)
                .Append(") ON DUPLICATE KEY UPDATE startDateTime = '").Append(strStartDateTime)
                .Append("', endDateTime = '").Append(strEndDateTime).Append("', productTableType = ").Append(_count).Append(";");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private int GetSaleDurationHour(ProductTableType productTableType)
        {
            if (productTableType == ProductTableType.UserDigital)
                return Data.GetDataUserProductDigital(_productId).SaleDurationHour;
            else if (productTableType == ProductTableType.UserReal)
                return Data.GetDataUserProductReal(_productId).SaleDurationHour;
            else
                throw new Exception("Wrong ProductTableType");
        }
    }
}
