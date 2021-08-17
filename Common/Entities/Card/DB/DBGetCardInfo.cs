using AkaDB.MySql;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.Card
{
    public class DBGetCardInfo 
    {
        public string StrUserId;
        public string StrCardId;

        public async Task<uint> GetLevel(DBContext db)
        {
            using (var cursor = await db.ExecuteReaderAsync($"SELECT level FROM cards WHERE id = {StrCardId} AND userId = {StrUserId};"))
            {
                if (false == cursor.Read())
                    throw new System.Exception();


                var cardLevel = (uint)cursor["level"];
                return cardLevel;
            }
        }

        public async Task<ProtoCardInfoExceptCount> GetInfos(DBContext db)
        {
            using (var cursor = await db.ExecuteReaderAsync($"SELECT id, level FROM cards WHERE id = {StrCardId} AND userId = {StrUserId};"))
            {
                if (false == cursor.Read())
                    throw new System.Exception();

                return new ProtoCardInfoExceptCount {
                    Id = (uint)cursor["id"],
                    Level = (uint)cursor["level"]
                };
            }
        }
    }
}
