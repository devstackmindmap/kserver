using System;
using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;
using CommonProtocol;

namespace WebServer.Controller.Test
{
    public class WebTestCallLockTest : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            var query = new StringBuilder();

            for (int i = 0; i < 1000; i++)
            {
                using (var db = new DBContext("AccountDBSetting"))
                {
                    query.Clear();
                    await db.BeginTransactionCallback(async () =>
                    {
                        query.Append("SELECT count FROM test WHERE seq=1 FOR UPDATE;");
                        using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                        {
                            if (false == cursor.Read())
                                throw new Exception();

                            var count = (int)cursor["count"];

                            query.Clear();
                            if (count % 2 == 0)
                                query.Append("UPDATE test SET count = count + 1, num = num + 1 WHERE seq=1");
                            else
                                query.Append("UPDATE test SET count = count + 1 WHERE seq=1");

                            await db.ExecuteNonQueryAsync(query.ToString());

                        }
                        return true;
                    });
                }

                //using (var db = new DBContext("AccountDBSetting"))
                //{
                //    query.Clear();
                //    query.Append("UPDATE test SET count = count + 1 WHERE seq=1;");
                //    await db.ExecuteNonQueryAsync(query.ToString());
                //}
            }
            
            return new ProtoUserId { UserId = req.UserId };
        }
    }
}
