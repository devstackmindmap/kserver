using AkaDB.MySql;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Mail
{
    public class CommonMail : Mail
    {
        public CommonMail(DBContext accountDb, DBContext userDb, uint userId) : base(accountDb, userDb, userId, "")
        {
        }

        public override async Task<ProtoMailActionResult> MailRead(uint mailId, bool setRead)
        {
            return null;
        }
    }
}
