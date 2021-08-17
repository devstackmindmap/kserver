using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Product;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public abstract class Mail : IMail
    {
        protected uint _userId;
        protected DBContext _accountDb;
        protected DBContext _userDb;
        protected ProtoMailInfo _mailInfo;
        protected StringBuilder _query = new StringBuilder();
        private string _tableName;

        protected Mail(DBContext accountDb, DBContext userDb, uint userId, string tableName)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _tableName = tableName;
        }

        public abstract Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead);

        public async Task<ProtoMailInfo> GetMailInfo(string languageType)
        {
            _mailInfo = new ProtoMailInfo();

            var utcNow = DateTime.UtcNow;
            var utcNowStr = utcNow.ToTimeString();
            var addUtcNowStr = utcNow.AddHours(ConstValue.SCHEDULE_BUFFER_HOUR).ToTimeString();

            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$utcNow", utcNowStr),
                new InputArg("$utcNowAddHour", addUtcNowStr)
                );

            uint maxPublicMailId = 0;
            using (var cursor = await _userDb.CallStoredProcedureAsync(StoredProcedure.GET_MAIL_INFO, paramInfo))
            {
                maxPublicMailId = GetMaxPublicMailId(cursor);
                GetUserPublicMailData(cursor);
                GetUserPrivateMailData(cursor);
                GetSystemMailData(cursor);
            }

            await GetPublicMailInfo(utcNowStr, addUtcNowStr, maxPublicMailId, languageType);

            List<uint> productIds = new List<uint>();
            productIds.AddRange(_mailInfo.PublicMailDatas.Select(PublicMailData => PublicMailData.Value.ProductId));
            productIds.AddRange(_mailInfo.PrivateMailDatas.Select(PrivateMailData => PrivateMailData.ProductId));

            ProductManager productManager = new ProductManager(_accountDb, _userDb, _userId);
            _mailInfo.Products = await productManager.GetProductInfos(productIds);

            return _mailInfo;
        }

        private uint GetMaxPublicMailId(DbDataReader cursor)
        {
            if (false == cursor.Read())
                return 0;
            return (uint)cursor["maxPublicMailId"];
        }

        private void GetUserPublicMailData(DbDataReader cursor)
        {
            if (cursor.NextResult() == false)
                return;

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

        private void GetUserPrivateMailData(DbDataReader cursor)
        {
            if (cursor.NextResult() == false)
                return;

            while (cursor.Read())
            {
                _mailInfo.PrivateMailDatas.Add(new ProtoDbMailData
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

        private void GetSystemMailData(DbDataReader cursor)
        {
            if (cursor.NextResult() == false)
                return;

            while (cursor.Read())
            {
                _mailInfo.SystemMailDatas.Add(new ProtoSystemMailData
                {
                    StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                    EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                    IsRead = Convert.ToBoolean((int)cursor["isRead"]),
                    MailId = (uint)cursor["mailId"],
                    SystemMailId = (uint)cursor["systemMailId"]
                });
            }
        }

        protected async Task GetPublicMailInfo(string utcNowStr, string addUtcNowStr, uint maxPublicMailId, string languageType)
        {
            var mailIds = string.Join(",", _mailInfo.PublicMailDatas.Select(PublicMailData => PublicMailData.Value.MailId));

            _query.Clear();
            _query.Append("SELECT  a.mailId, startDateTime, endDateTime, " +
                "mailIcon, productId, b.mailTitle, b.mailText " +
                "FROM _mail_public a " +
                "LEFT OUTER JOIN _mail_public_text b ON b.mailId = a.mailId AND b.languageType = '")
                .Append(languageType).Append("' WHERE a.mailId > ").Append(maxPublicMailId);

            if (mailIds == "")
            {
                _query.Append(" AND endDateTime > '").Append(utcNowStr).Append("' AND startDateTime <= '")
                    .Append(addUtcNowStr).Append("' ORDER BY a.mailId ASC;");
            }
            else
            {
                _query.Append(" OR a.mailId IN (").Append(mailIds).Append(") AND endDateTime > '").Append(utcNowStr)
                    .Append("' AND startDateTime <= '").Append(addUtcNowStr).Append("' ORDER BY a.mailId ASC;");
            }

            var newUserPublicMails = new Dictionary<uint, ProtoDbMailData>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var mailId = (uint)cursor["mailId"];
                    if (false == _mailInfo.PublicMailDatas.ContainsKey(mailId))
                    {
                        SetNewUserPublicMail(cursor, _mailInfo.PublicMailDatas, mailId);
                        SetNewUserPublicMail(cursor, newUserPublicMails, mailId);
                        continue;
                    }
                    _mailInfo.PublicMailDatas[mailId].StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks;
                    _mailInfo.PublicMailDatas[mailId].EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks;
                    _mailInfo.PublicMailDatas[mailId].MailIcon = (string)cursor["mailIcon"];
                    _mailInfo.PublicMailDatas[mailId].ProductId = (uint)cursor["productId"];
                    _mailInfo.PublicMailDatas[mailId].MailTitle = (string)cursor["mailTitle"];
                    _mailInfo.PublicMailDatas[mailId].MailText = (string)cursor["mailText"];
                }
            }

            await InsertNewUserPublicMail(newUserPublicMails);
        }

        private void SetNewUserPublicMail(DbDataReader cursor, Dictionary<uint, ProtoDbMailData> newUserPublicMails, uint mailId)
        {
            newUserPublicMails.Add(mailId, new ProtoDbMailData
            {
                EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                MailIcon = (string)cursor["mailIcon"],
                MailId = mailId,
                ProductId = (uint)cursor["productId"],
                IsRead = false,
                MailTitle = (string)cursor["mailTitle"],
                MailText = (string)cursor["mailText"]
            });
        }

        private async Task InsertNewUserPublicMail(Dictionary<uint, ProtoDbMailData> newUserPublicMails)
        {
            if (newUserPublicMails.Count == 0)
                return;

            var query = new StringBuilder();
            var insertValues = string.Join(",",
                newUserPublicMails.Values.
                Select(data => $"({_userId}, {data.MailId}, 0, '{(new DateTime(data.StartDateTime)).ToTimeString()}', " +
                $"'{(new DateTime(data.EndDateTime)).ToTimeString()}', 0)"));

            query.Append("INSERT INTO user_mail_public (userId, mailId, isDeleted, startDateTime, endDateTime, isRead) VALUES ")
                .Append(insertValues).Append(";");

            await _userDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<ProtoMailReadAllResult> MailReadAll(List<uint> mailIds)
        {
            var mailReadResults = new ProtoMailReadAllResult();
            foreach (var mailId in mailIds)
            {
                mailReadResults.ProtoMailReadResults.Add(mailId, await MailRead(mailId, false));
            }
            return mailReadResults;
        }

        public async Task<ProtoMailDeleteAllResult> MailDeleteAll(List<uint> mailIds)
        {
            var mailReadResults = new ProtoMailDeleteAllResult();
            foreach (var mailId in mailIds)
            {
                mailReadResults.ProtoMailReadResults.Add(mailId, await MailDelete(mailId));
            }
            return mailReadResults;
        }

        private async Task<ResultType> MailDelete(uint mailId)
        {
            var mailReadResult = new ProtoMailActionResult { ResultType = ResultType.InvalidMailId };
            await GetUserMailInfo(mailId, mailReadResult);
            if (mailReadResult.ResultType == ResultType.AlreadyMailRead)
            {
                await SetDelete(mailId);
                return ResultType.Success;
            }
            else if (mailReadResult.ResultType == ResultType.Success)
            {
                return ResultType.MailUnread;
            }

            return mailReadResult.ResultType;
        }

        protected async Task<uint> GetUserMailInfo(uint mailId, ProtoMailActionResult  mailReadResult)
        {
            _query.Clear();
            _query.Append("SELECT isDeleted, isRead, startDateTime, endDateTime ");
            if (_tableName == TableName.USER_MAIL_PRIVATE)
                _query.Append(", productId");
            else if (_tableName == TableName.USER_MAIL_SYSTEM)
                _query.Append(", systemMailId");

            _query.Append(" FROM ").Append(_tableName).Append(" WHERE userId = ").Append(_userId)
                .Append(" AND mailId = ").Append(mailId).Append(";");

            uint productIdOrSystemMailId = 0;
            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                {
                    mailReadResult.ResultType = GetResultType(cursor);
                    if (_tableName == TableName.USER_MAIL_PRIVATE)
                        productIdOrSystemMailId = (uint)cursor["productId"];
                    else if (_tableName == TableName.USER_MAIL_SYSTEM)
                        productIdOrSystemMailId = (uint)cursor["systemMailId"];
                }
            }
            return productIdOrSystemMailId;
        }

        protected ResultType GetResultType(DbDataReader cursor)
        {
            if ((int)cursor["isDeleted"] == 1)
                return ResultType.AlreadyMailDeleted;

            if ((int)cursor["isRead"] == 1)
                return ResultType.AlreadyMailRead;

            var utcNow = DateTime.UtcNow;
            if ((DateTime)cursor["startDateTime"] > utcNow || (DateTime)cursor["endDateTime"] < utcNow)
                return ResultType.InvalidDateTime;

            return ResultType.Success;
        }

        protected async Task GiveProductAndSetRead(ProtoMailActionResult mailReadResult,
            uint productId, uint mailId, bool setRead, string logCategory)
        {
            if (productId != 0)
            {
                if (_tableName == TableName.USER_MAIL_SYSTEM)
                {
                    mailReadResult.ItemResults = await ProductRewardManager.GiveProduct(_userDb,
                        _userId, 0, productId, logCategory);
                }
                else
                {
                    mailReadResult.ItemResults = await ProductRewardManager.GiveProduct(_accountDb, _userDb,
                        _userId, 0, productId, logCategory);
                }

                if (ProductRewardManager.IsEmptyItem(mailReadResult.ItemResults))
                    mailReadResult.ResultType = ResultType.EmptyItem;
            }

            if (productId != 0 || setRead)
                await SetRead(mailId);
            else
                mailReadResult.ResultType = ResultType.EmptyItem;
        }

        private async Task SetRead(uint mailId)
        {
            _query.Clear();
            _query.Append("UPDATE ").Append(_tableName).Append(" SET isRead = 1 WHERE userId = ").Append(_userId).
                Append(" AND mailId = ").Append(mailId).Append(";");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        protected async Task SetDelete(uint mailId)
        {
            _query.Clear();
            _query.Append("UPDATE ").Append(_tableName).Append(" SET isDeleted = 1 WHERE userId = ").Append(_userId).
                Append(" AND mailId = ").Append(mailId).Append(";");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
