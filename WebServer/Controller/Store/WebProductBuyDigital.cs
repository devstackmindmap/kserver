using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Product;
using CommonProtocol;

namespace WebServer.Controller.Store
{
    public class WebProductBuyDigital : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoBuyProductDigital;
            var res = new ProtoOnBuyProductDigital();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var buy = WebLogic.Store.ProductBuyFactory.CreateProductBuy(accountDb, userDb, req.UserId, 
                        req.ProductId, req.ProductTableType);

                    if (false == await buy.SetProductBuyCondition())
                    {
                        res.ResultType = ResultType.Fail;
                        return res;
                    }

                    var resultType = await buy.CheckBuy(accountDb, userDb, 0);
                    if (resultType != ResultType.Success)
                    {
                        res.ResultType = resultType;
                        return res;
                    } 

                    await userDb.BeginTransactionCallback(async () =>
                    {
                        res.ItemResults = await ProductRewardManager.GiveProduct(accountDb, userDb, req.UserId, req.ItemValue, req.ProductId, "ProductBuyDigital");
                        if (res.ItemResults.Count == 0)
                            return false;

                        await buy.UseMaterial();
                        await buy.AddPurchaseCount(userDb, req.ProductId);
                        var (materialType, remainCount) = await buy.GetMaterialInfo();
                        res.MaterialInfo = new ProtoMaterialInfo
                        {
                            MaterialType = materialType,
                            Count = remainCount
                        };
                        return true;
                    });
                }
            }

            if (res.ItemResults.Count == 0)
                res.ResultType = ResultType.Fail;
            else
                res.ResultType = ResultType.Success;

            Log.Item.ProductBuyDigital(req.UserId, req.ProductId, req.ItemValue, "ProductBuyDigital");
            return res;
        }
    }
}
