using System.Threading.Tasks;
using AkaDB.MySql;

namespace Common.Entities.Unit
{
    public class DBGetUnitSkinId
    {
        public string StrUserId;
        public string StrUnitId;

        public async Task<uint> ExecuteAsync(DBContext db)
        {
            using (var cursor = await db.ExecuteReaderAsync($"SELECT skinId FROM units WHERE userId = {StrUserId} AND id = {StrUnitId};"))
            {
                cursor.Read();

                var skinId = (uint)cursor["skinId"];

                return skinId;
            }
        }
    }
}