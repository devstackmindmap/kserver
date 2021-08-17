using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Mail;
using Common.Entities.Product;
using CommonProtocol;

namespace WebServer.Controller.Mail
{
    public class WebMailUpdatePrivate : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var res = new ProtoPrivateMailInfo();
                    await userDb.BeginTransactionCallback(async () =>
                    {
                        var mail = MailFactory.CreatePrivateMail(null, userDb, req.UserId);
                        res = await mail.MailUpdate();

                        List<uint> productIds = new List<uint>();
                        productIds.AddRange(res.PrivateMailDatas.Select(PrivateMailData => PrivateMailData.ProductId));
                        ProductManager productManager = new ProductManager(accountDb, userDb, req.UserId);
                        res.Products = await productManager.GetProductInfos(productIds);

                        return true;
                    });
                    return res;
                }
            }
        }
    }
}
