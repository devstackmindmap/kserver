using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;

namespace WebServer.Controller.Chatting
{
    public class WebSetChattingMessage : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoSetChattingMessage;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                await SetChattingMessage(accountDb, req);

                return new ProtoResult
                {
                    ResultType = ResultType.Success
                };
            }
        }

        public async Task SetChattingMessage(DBContext accountDb, ProtoSetChattingMessage req)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO chatting_messages (chattingKey, chattingDateTime, chattingType, chattingMessage) " +
                "VALUES ('").Append(req.ChattingKey).Append("', '").Append(DateTime.UtcNow.ToTimeString()).Append("',")
                .Append((int)req.ChattingType).Append(", '").Append(MakeChattingMessage(req)).Append("');");

            await accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        private string MakeChattingMessage(ProtoSetChattingMessage req)
        {
            if (req.ChattingType.In(ChattingType.ClanMessage))
            {
                return req.UserNickname + "|" + req.ChattingMessage;
            }
            else if (req.ChattingType.In(ChattingType.ClanJoin, ChattingType.ClanOut))
            {
                return req.UserNickname;
            }
            else if (req.ChattingType.In(ChattingType.ClanBanish, ChattingType.ClanGradeUp, ChattingType.ClanGradeDown))
            {
                return req.UserNickname + "|" + req.TargetNickname;
            }
            else
            {
                throw new Exception("Wrong Chatting Type");
            }
        }
    }
}
