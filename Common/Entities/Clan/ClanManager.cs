using AkaDB.MySql;
using AkaRedisLogic;
using StackExchange.Redis;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Clan
{
    public class ClanManager
    {
        public static  async Task<uint> GetClanId(DBContext accountDb, uint userId)
        {
            var query = new StringBuilder();
            query.Append("SELECT clanId FROM clan_members WHERE userId = ").Append(userId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;

                return (uint)cursor["clanId"];
            }
        }

        public static async Task<string> GetClanInfo(DBContext accountDb, uint clanId)
        {
            var query = new StringBuilder();
            query.Append("SELECT countryCode FROM clans WHERE clanId = ").Append(clanId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return "";

                return (string)cursor["countryCode"];
            }
        }
    }
}
