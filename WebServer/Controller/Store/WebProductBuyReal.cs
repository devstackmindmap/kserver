using System.Collections.Generic;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.PayRewardedCheck;
using Common.Entities.Product;
using CommonProtocol;
using WebLogic.Store;

namespace WebServer.Controller.Store
{
    public class WebProductBuyReal : BaseController
    {
        private ProductBuy _buy;
        private ReceiptCheckResponse _receipts;
        private ProtoBuyProductReal _req;
        private ProtoOnBuyProductReal _res;
        private PayRewarded _payRewarded;

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            _req = requestInfo as ProtoBuyProductReal;
            _res = new ProtoOnBuyProductReal();

            for (var i = 0; i < _req.StoreInfos.Count; i++)
            {
                using (var accountDb = new DBContext("AccountDBSetting"))
                {
                    _payRewarded = new PayRewarded(accountDb, _req.UserId);
                    if (IsPendingRequest(_req.StoreInfos[i]))
                    {
                        if (false == await _payRewarded.GetProductInfoByTransactionId(_req.StoreInfos[i]))
                        {
                            await _payRewarded.SetIssueStoreInfo(_req.StoreInfos[i], 1);
                            _res.Results.Add(new ProtoOnBuyProductRealItem { ResultType = ResultType.CustomerService });
                            return _res;
                        }

                        if (_req.StoreInfos[i].ProductTableType == ProductTableType.SeasonPass)
                        {
                            _res.Results.Add(new ProtoOnBuyProductRealItem { ResultType = ResultType.SeasonPassPending, ProductId = (uint)i, StoreProductId = _req.StoreInfos[i].StoreProductId });
                            return _res;
                        }
                    }
                    else
                        await _payRewarded.SetStoreInfo(_req.StoreInfos[i], 1);

                    using (var userDb = new DBContext(_req.UserId))
                    {
                        if (false == await SetConditionValueAndGetReceipt(accountDb, userDb, _req.StoreInfos[i]))
                            return _res;

                        await CheckReceiptAndGiveProduct(accountDb, userDb, _req.StoreInfos[i]);
                    }
                }
            }
            return _res;
        }

        private bool IsPendingRequest(ProtoStoreInfo storeInfo)
        {
            return storeInfo.ProductId == 0;
        }

        private async Task<bool> SetConditionValueAndGetReceipt(DBContext accountDb, DBContext userDb, ProtoStoreInfo storeInfo)
        {

            _buy = ProductBuyFactory.CreateProductBuy(accountDb, userDb,
                _req.UserId, storeInfo.ProductId, storeInfo.ProductTableType, storeInfo.PlatformType);

            if (false == await _buy.SetProductBuyCondition())
            {
                _res.Results.Add(new ProtoOnBuyProductRealItem { ResultType = ResultType.Fail });
                return false;
            }

            if (false == _buy.GetReceipt(storeInfo, out var result))
            {
                _res.Results.Add(new ProtoOnBuyProductRealItem { ResultType = ResultType.PaymentServerError });
                return false;
            }

            if (result == null)
            {
                _res.Results.Add(new ProtoOnBuyProductRealItem { ResultType = ResultType.PaymentServerError });
                return false;
            }

            _receipts = result;

            return true;
        }

        private async Task CheckReceiptAndGiveProduct(DBContext accountDb, DBContext userDb, ProtoStoreInfo storeInfo)
        {
            foreach (var receipt in _receipts.receipt)
            {
                if (storeInfo.StoreProductId != receipt.productId)
                    continue;

                if (receipt.Result != "0")
                {
                    AddResult(storeInfo.ProductId, receipt.productId, receipt.Result);
                    continue;
                }

                if (await _buy.IsAlreadyGivenItems(accountDb, receipt.transactionId))
                {
                    await _payRewarded.SetStoreInfo(storeInfo, 0);
                    AddResult(storeInfo.ProductId, receipt.productId, ResultType.AlreadyCheckedReceipt);
                    continue;
                }

                var resultType = await _buy.CheckBuy(accountDb, userDb, storeInfo.ProductId);
                if (resultType != ResultType.Success)
                {
                    AddResult(storeInfo.ProductId, receipt.productId, resultType);
                    continue;
                }

                await accountDb.BeginTransactionCallback(async () =>
                {
                    await userDb.BeginTransactionCallback(async () =>
                    {
                        var itemResult = await ProductRewardManager.GiveProduct(accountDb, userDb, _req.UserId, 0,
                            storeInfo.ProductId, "ProductBuyReal");
                        if (itemResult.Count == 0)
                        {
                            AddResult(storeInfo.ProductId, receipt.productId, ResultType.Fail);
                            return false;
                        }

                        await _buy.AddPurchaseCount(userDb, storeInfo.ProductId);
                        await _buy.AddPurchased(accountDb, receipt.transactionId, receipt.productId);

                        AddResult(storeInfo.ProductId, receipt.productId, ResultType.Success, itemResult);

                        await _payRewarded.SetStoreInfo(storeInfo, 0);

                        Log.Item.ProductBuyReal(_req.UserId, storeInfo.ProductId, storeInfo.PlatformType,
                            receipt.transactionId, "ProductBuyReal");

                        return true;
                    });
                    return true;
                });
            }
        }

        private ResultType GetReceiptResultToResultType(string result)
        {
            switch (result)
            {
                case "1":
                    return ResultType.PaymentPayback;
                case "3":
                    return ResultType.PaymentNetworkError;
                case "4":
                    return ResultType.PaymentDbError;
                case "5":
                    return ResultType.PaymentInvalidProductInfo;
                default:
                    return ResultType.Fail;
            }
        }

        private void AddResult(uint productId, string storeProductId, ResultType resultType, List<List<ProtoItemResult>> itemResults = null)
        {
            _res.Results.Add(new ProtoOnBuyProductRealItem
            {
                ProductId = productId,
                StoreProductId = storeProductId,
                ResultType = resultType,
                ItemResults = itemResults
            });
        }

        private void AddResult(uint productId, string storeProductId, string receiptResult)
        {
            _res.Results.Add(new ProtoOnBuyProductRealItem
            {
                ProductId = productId,
                StoreProductId = storeProductId,
                ResultType = GetReceiptResultToResultType(receiptResult)
            });
        }
    }
}
