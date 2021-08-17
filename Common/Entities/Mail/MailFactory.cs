using AkaDB.MySql;
using AkaEnum;
using System;

namespace Common.Entities.Mail
{
    public class MailFactory
    {
        public static IMail CreateMail(MailType mailType, DBContext accountDb, DBContext userDb, uint userId)
        {
            switch (mailType)
            {
                case MailType.Common:
                    return new CommonMail(accountDb, userDb, userId);
                case MailType.Public:
                    return new PublicMail(accountDb, userDb, userId, TableName.USER_MAIL_PUBLIC);
                case MailType.System:
                    return new SystemMail(accountDb, userDb, userId, TableName.USER_MAIL_SYSTEM);
                case MailType.Private:
                    return new PrivateMail(accountDb, userDb, userId, TableName.USER_MAIL_PRIVATE);
                default:
                    throw new Exception("Wrong MailType");
            }
        }

        public static PublicMail CreatePublicMail(DBContext accountDb, DBContext userDb, uint userId)
        {
            return new PublicMail(accountDb, userDb, userId, TableName.USER_MAIL_PUBLIC);
        }

        public static PrivateMail CreatePrivateMail(DBContext accountDb, DBContext userDb, uint userId)
        {
            return new PrivateMail(accountDb, userDb, userId, TableName.USER_MAIL_PRIVATE);
        }

        public static SystemMail CreateSystemMail(DBContext accountDb, DBContext userDb, uint userId)
        {
            return new SystemMail(accountDb, userDb, userId, TableName.USER_MAIL_SYSTEM);
        }
    }
}
