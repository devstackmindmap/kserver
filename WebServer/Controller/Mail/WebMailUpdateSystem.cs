using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Mail;
using CommonProtocol;

namespace WebServer.Controller.Mail
{
    public class WebMailUpdateSystem : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var res = new ProtoSystemMailInfo();
                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            var mail = MailFactory.CreateSystemMail(null, userDb, req.UserId);
                            res = await mail.MailUpdate();
                            return true;
                        });
                        return true;
                    });
                    return res;
                }
            }
        }
    }
}
