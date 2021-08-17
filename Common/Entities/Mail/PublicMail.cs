using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public class PublicMail : Mail
    {
        public PublicMail(DBContext accountDb, DBContext userDb, uint userId, string tableName) : base(accountDb, userDb, userId, tableName)
        {
        }

        public override async Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead)
        {
            var mailReadResult = new ProtoMailActionResult { ResultType = ResultType.InvalidMailId };
            await GetUserMailInfo(mailId, mailReadResult);
            if (mailReadResult.ResultType == ResultType.Success)
            {
                var productId = await GetProductId(mailId);
                await GiveProductAndSetRead(mailReadResult, productId, mailId, setRead, "PublicMailRead");
            }

            return mailReadResult;
        }

        private async Task<uint> GetProductId(uint mailId)
        {
            _query.Clear();
            _query.Append("SELECT productId FROM _mail_public WHERE mailId = ").Append(mailId).Append(";");
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;
                return (uint)cursor["productId"];
            }
        }

        public async Task<ProtoPublicMailInfo> MailUpdate(string languageType)
        {
            _mailInfo = new ProtoMailInfo();
            var utcNow = DateTime.UtcNow;
            var utcNowStr = utcNow.ToTimeString();
            var addUtcNowStr = utcNow.AddHours(ConstValue.SCHEDULE_BUFFER_HOUR).ToTimeString();
            
            var protoPublicMailInfo = new ProtoPublicMailInfo();
            var maxPublicMailId = await GetUserPublicMaxMailId();
            await SetUserMailPublic(protoPublicMailInfo, utcNowStr, addUtcNowStr);
            await GetPublicMailInfo(utcNowStr, addUtcNowStr, maxPublicMailId, languageType);

            return new ProtoPublicMailInfo
            {
                PublicMailDatas = _mailInfo.PublicMailDatas
            };
        }

        private async Task<uint> GetUserPublicMaxMailId()
        {
            _query.Clear();
            _query.Append("SELECT MAX(mailId) as maxPublicMailId FROM user_mail_public " +
                "WHERE userId = ").Append(_userId).Append(" GROUP BY userId;");

            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;
                return (uint)cursor["maxPublicMailId"];
            }
        }

        private async Task SetUserMailPublic(ProtoPublicMailInfo publicMailInfo, string utcNowStr, string addUtcNowStr)
        {
            _query.Clear();
            _query.Append("SELECT mailId, startDateTime, endDateTime, isRead FROM user_mail_public " +
                "WHERE userId = ").Append(_userId).Append(" AND isDeleted = 0 and endDateTime > '").Append(utcNowStr)
                .Append("' AND startDateTime <= '").Append(addUtcNowStr).Append("'; ");

            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var mailId = (uint)cursor["mailId"];

                    _mailInfo.PublicMailDatas.Add(mailId, new ProtoDbMailData
                    {
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                        MailId = mailId,
                        IsRead = Convert.ToBoolean((int)cursor["isRead"])
                    });
                }
            }
        }
    }
}
