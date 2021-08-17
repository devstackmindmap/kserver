using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Mail;
using Common.Entities.Product;
using CommonProtocol;

namespace WebServer.Controller.Mail
{
    public class WebMailUpdatePublic : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoMailUpdatePublic;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var res = new ProtoPublicMailInfo();
                    await accountDb.BeginTransactionCallback(async () =>
                    {
                        await userDb.BeginTransactionCallback(async () =>
                        {
                            var mail = MailFactory.CreatePublicMail(accountDb, userDb, req.UserId);
                            res =  await mail.MailUpdate(req.LanguageType);

                            List<uint> productIds = new List<uint>();
                            productIds.AddRange(res.PublicMailDatas.Select(PublicMailDatas => PublicMailDatas.Value.ProductId));
                            ProductManager productManager = new ProductManager(accountDb, userDb, req.UserId);
                            res.Products = await productManager.GetProductInfos(productIds);

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
