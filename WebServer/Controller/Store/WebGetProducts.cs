using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using Common.Entities.Product;

namespace WebServer.Controller.Store
{
    public class WebGetProducts : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoGetProducts;

            
            ProtoOnGetProducts protoProducts = new ProtoOnGetProducts();
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    ProductManager productManager = new ProductManager(accountDb, userDb, req.UserId);
                    await productManager.GetStoreProducts(req.PlatformType, req.LanguageType, protoProducts);
                }
            }
            return protoProducts;
        }
    }
}
