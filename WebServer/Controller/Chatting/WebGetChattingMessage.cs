using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;

namespace WebServer.Controller.Chatting
{
    public class WebGetChattingMessage : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetChattingMessage;
            var res = new ProtoOnGetChattingMessage();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                await GetChattingMessage(accountDb, req, res);
            }

            return res;
        }

        public async Task GetChattingMessage(DBContext accountDb, ProtoGetChattingMessage req, ProtoOnGetChattingMessage res)
        {
            var query = new StringBuilder();
            query.Clear();
            query.Append("SELECT seq, chattingKey, chattingDateTime, chattingType, chattingMessage FROM chatting_messages " +
                "WHERE chattingKey = '").Append(req.ChattingKey).Append("' AND chattingDateTime >= '")
                .Append(DateTime.UtcNow.AddDays(-5).ToTimeString()).Append("' ORDER BY seq DESC LIMIT 100;");

            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    res.ChattingMessages.Add(new ProtoChattingMessage
                    {
                        Seq = (uint)cursor["seq"],
                        ChattingDateTime = ((DateTime)cursor["chattingDateTime"]).Ticks,
                        ChattingMessage = (string)cursor["chattingMessage"],
                        ChattingType = (ChattingType)(int)cursor["chattingType"]
                    });
                }
            }
        }
    }
}
