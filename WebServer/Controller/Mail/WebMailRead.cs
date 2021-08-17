using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Mail;
using CommonProtocol;

namespace WebServer.Controller.Mail
{
    public class WebMailRead : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoMailRead;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    ProtoMailActionResult readResult = new ProtoMailActionResult();
                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            var mail = MailFactory.CreateMail(req.MailType, accountDb, userDb, req.UserId);
                            readResult = await mail.MailRead(req.MailId, true);
                            return true;
                        });
                        return true;
                    });

                    return readResult;
                }
            }
        }
    }
}
