using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public class PrivateMail : Mail
    {
        public PrivateMail(DBContext accountDb, DBContext userDb, uint userId, string tableName) : base(accountDb, userDb, userId, tableName)
        {
        }

        public override async Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead)
        {
            var mailReadResult = new ProtoMailActionResult { ResultType = ResultType.InvalidMailId };
            var productId = await GetUserMailInfo(mailId, mailReadResult);
            if (mailReadResult.ResultType == ResultType.Success)
                await GiveProductAndSetRead(mailReadResult, productId, mailId, setRead, "PrivateMailRead");

            return mailReadResult;
        }

        public async Task<ProtoPrivateMailInfo> MailUpdate()
        {
            var utcNow = DateTime.UtcNow;
            var utcNowStr = utcNow.ToTimeString();
            var addUtcNowStr = utcNow.AddHours(ConstValue.SCHEDULE_BUFFER_HOUR).ToTimeString();

            _query.Clear();
            _query.Append("SELECT mailId, startDateTime, endDateTime, isRead, mailIcon, productId, mailTitle, mailText " +
                "FROM user_mail_private WHERE userId = ").Append(_userId)
                .Append(" AND isDeleted = 0 AND endDateTime > '").Append(utcNowStr)
                .Append("' AND startDateTime <= '").Append(addUtcNowStr).Append("';");

            var privateMailInfo = new ProtoPrivateMailInfo();
            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    privateMailInfo.PrivateMailDatas.Add(new ProtoDbMailData
                    {
                        MailId = (uint)cursor["mailId"],
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                        IsRead = Convert.ToBoolean((int)cursor["isRead"]),
                        MailIcon = (string)cursor["mailIcon"],
                        ProductId = (uint)cursor["productId"],
                        MailTitle = (string)cursor["mailTitle"],
                        MailText = (string)cursor["mailText"]
                    });
                }
            }
            return privateMailInfo;
        }
    }
}
