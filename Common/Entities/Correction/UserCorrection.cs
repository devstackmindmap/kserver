using AkaDB.MySql;
using AkaUtility;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Correction
{
    public class UserCorrection
    {
        private string _strUserId;
        private string _strType;
        private DBContext _db;

        public UserCorrection(string strUserId, string strType, DBContext db)
        {
            _strUserId = strUserId;
            _strType = strType;
            _db = db;
        }

        public async Task<Dictionary<uint, int>> GetCorrections()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT id, correction FROM corrections WHERE userId=").Append(_strUserId)
                .Append(" AND type = ").Append(_strType).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                Dictionary<uint, int> corrections = new Dictionary<uint, int>();

                while (cursor.Read())
                    corrections.Add((uint)cursor["id"], (int)cursor["correction"]);

                return corrections;
            }
        }

        public async Task SetCorrections(IDictionary<uint, int> corrections)
        {
            StringBuilder query = new StringBuilder();
            foreach (var correction in corrections)
            {
                var strCorrectionKey = correction.Key.ToString();
                var strCorrectionValue = correction.Value.ToString();
                query.Append("INSERT INTO `corrections` (`userId`, `type`, `id`, `correction`) ")
                    .Append("VALUES (").Append(_strUserId).Append(", ").Append(_strType).Append(", ")
                    .Append(strCorrectionKey).Append(", ").Append(strCorrectionValue).Append(") ")
                    .Append("ON DUPLICATE KEY UPDATE correction = ").Append(strCorrectionValue).Append(";");
            }
            await _db.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
