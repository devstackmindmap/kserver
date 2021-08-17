using AkaDB.MySql;
using System.Threading.Tasks;

namespace Common.Entities.User
{

    public class DBGetUserNickname
    {
        public string StrUserId;

        public async Task<(string nickName, uint profileIconId)> ExecuteAsync(DBContext db)
        {
            using (var cursor = await db.ExecuteReaderAsync("SELECT nickName , profileIconId FROM accounts WHERE userId = {0};", StrUserId))
            {
                cursor.Read();
                return  ((string)cursor["nickName"], (uint)cursor["profileIconId"]);
            }
        }
    }
}
