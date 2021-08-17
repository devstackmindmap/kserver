using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public class SystemMail : Mail
    {
        public SystemMail(DBContext accountDb, DBContext userDb, uint userId, string tableName) : base(accountDb, userDb, userId, tableName)
        {
        }

        public override async Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead)
        {
            var mailReadResult = new ProtoMailActionResult { ResultType = ResultType.InvalidMailId };
            var systemMailId = await GetUserMailInfo(mailId, mailReadResult);
            if (mailReadResult.ResultType == ResultType.Success)
            {
                var productId = GetProductId(systemMailId);
                await GiveProductAndSetRead(mailReadResult, productId, mailId, setRead, "SystemMailRead");
            }

            return mailReadResult;
        }

        public async Task<ProtoMailReadAllResult> MailReadAllSystem(List<uint> mailIds)
        {
            var mailReadResults = new ProtoMailReadAllResult();

            for (int i=0; i< mailIds.Count; i++)
            {
                mailReadResults.ProtoMailReadResults.Add(mailIds[i], await MailRead(mailIds[i], false));
            }
            return mailReadResults;
        }

        private uint GetProductId(uint mailId)
        {
            return Data.GetDataMail(mailId).ProductId;
        }

        public async Task<ProtoSystemMailInfo> MailUpdate()
        {
            var utcNow = DateTime.UtcNow;
            var utcNowStr = utcNow.ToTimeString();
            var addUtcNowStr = utcNow.AddHours(ConstValue.SCHEDULE_BUFFER_HOUR).ToTimeString();

            _query.Clear();
            _query.Append("SELECT mailId, systemMailId, startDateTime, endDateTime, isRead " +
                "FROM user_mail_system WHERE userId = ").Append(_userId)
                .Append(" AND isDeleted = 0 AND endDateTime > '").Append(utcNowStr)
                .Append("' AND startDateTime <= '").Append(addUtcNowStr).Append("';");

            var systemMailInfo = new ProtoSystemMailInfo();
            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    systemMailInfo.SystemMailDatas.Add(new ProtoSystemMailData
                    {
                        MailId = (uint)cursor["mailId"],
                        SystemMailId = (uint)cursor["systemMailId"],
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                        IsRead = Convert.ToBoolean((int)cursor["isRead"])
                    });
                }
            }
            return systemMailInfo;
        }
    }
}
