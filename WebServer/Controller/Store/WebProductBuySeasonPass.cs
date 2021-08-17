using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Charger;
using Common.Entities.PayRewardedCheck;
using Common.Entities.Product;
using Common.Entities.Season;
using Common.Pass;
using CommonProtocol;
using WebLogic.Store;

namespace WebServer.Controller.Store
{
    public class WebProductBuySeasonPass : BaseController
    {
        private ProductBuy _buy;
        private ReceiptCheckResponse _receipts;
        private ProtoBuySeasonPass _req;
        private ProtoOnBuySeasonPass _res;
        private PayRewarded _payRewarded;
        private ServerSeasonInfo _seasonPassInfo;

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            _req = requestInfo as ProtoBuySeasonPass;
            _res = new ProtoOnBuySeasonPass();

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var serverInfo = new ServerSeason(accountDb);
                _seasonPassInfo = await serverInfo.GetSeasonPassInfo();

                var seasonPassData = Data.GetSeasonPassListForSeason(_seasonPassInfo.CurrentSeason);
                var seasonPass = seasonPassData.Where(data => data.SeasonPassType == _req.SeasonPassType).First();
                var storeProductId = _req.PlatformType == PlatformType.Google ? seasonPass.AosStoreProductId : seasonPass.IosStoreProductId;

                var storeInfo = new ProtoStoreInfo
                {
                    PlatformType = _req.PlatformType,
                    ProductId = seasonPass.ProductId,
                    ProductTableType = ProductTableType.SeasonPass,
                    PurchaseToken = _req.PurchaseToken,
                    StoreProductId = storeProductId,
                    TransactionId = _req.TransactionId
                };

                _payRewarded = new PayRewarded(accountDb, _req.UserId);
                if (_req.SeasonPassSeason != _seasonPassInfo.CurrentSeason)
                {
                    await _payRewarded.SetStoreInfo(storeInfo, 1);
                    _res.ResultType = ResultType.InvalidSeason;
                    _res.StoreProductId = storeInfo.StoreProductId;
                    return _res;
                }

                if (_req.IsPending)
                {
                    if (false == await _payRewarded.GetProductInfoByTransactionId(storeInfo))
                    {
                        await _payRewarded.SetIssueStoreInfo(storeInfo, 1);
                        _res.ResultType = ResultType.CustomerService;
                        return _res;
                    }
                }
                else
                {
                    await _payRewarded.SetStoreInfo(storeInfo, 1);
                }

                using (var userDb = new DBContext(_req.UserId))
                {
                    if (false == await GetReceipt(accountDb, userDb, storeInfo))
                        return _res;

                    await CheckReceiptAndGiveProduct(accountDb, userDb, storeInfo);
                }
            }

            return _res;
        }

        private async Task<bool> GetReceipt(DBContext accountDb, DBContext userDb, ProtoStoreInfo storeInfo)
        {

            _buy = ProductBuyFactory.CreateProductBuy(accountDb, userDb,
                _req.UserId, storeInfo.ProductId, storeInfo.ProductTableType, storeInfo.PlatformType);

            if (false == _buy.GetReceipt(storeInfo, out var result))
            {
                _res.ResultType = ResultType.PaymentServerError;
                return false;
            }

            if (result == null)
            {
                _res.ResultType = ResultType.PaymentServerError;
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
                    _res.ResultType = GetReceiptResultToResultType(receipt.Result);
                    continue;
                }

                if (await _buy.IsAlreadyGivenItems(accountDb, receipt.transactionId))
                {
                    await _payRewarded.SetStoreInfo(storeInfo, 0);
                    _res.ResultType = ResultType.AlreadyCheckedReceipt;
                    _res.StoreProductId = receipt.productId;
                    continue;
                }

                var seasonPassManager = new SeasonPassManager(_req.UserId, _req.SeasonPassSeason, userDb);
                if (await seasonPassManager.IsPurchasedCurrentSeasonPass())
                {
                    _res.ResultType = ResultType.AlreadyPurchased;
                    _res.StoreProductId = receipt.productId;
                    continue;
                }

                await accountDb.BeginTransactionCallback(async () =>
                {
                    await userDb.BeginTransactionCallback(async () =>
                    {
                        _res.InfusionBox = await EnergyUpdate(accountDb, userDb, _req.UserId);
                        var itemResult = await ProductRewardManager.GiveProduct(accountDb, userDb, _req.UserId, 0,
                            storeInfo.ProductId, "ProductBuyReal");
                        if (itemResult.Count == 0)
                        {
                            _res.ResultType = ResultType.Fail;
                            return false;
                        }

                        await _buy.AddPurchaseCount(userDb, storeInfo.ProductId);
                        await _buy.AddPurchased(accountDb, receipt.transactionId, receipt.productId);

                        _res.ResultType = ResultType.Success;

                        await _payRewarded.SetStoreInfo(storeInfo, 0);

                        Log.Item.ProductBuyReal(_req.UserId, storeInfo.ProductId, storeInfo.PlatformType,
                            receipt.transactionId, "ProductBuySeasonPass");

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

        private async Task<ProtoInfusionBox> EnergyUpdate(DBContext accountDb, DBContext userDb, uint userId)
        {
            var purchasedSeasons
                        = await (new SeasonPassManager(userId, _seasonPassInfo.CurrentSeason, userDb))
                        .GetBeforeAndCurrentPurchasedSeasonPassList();

            var charger = ChargerFactory.CreateCharger(userId, InfusionBoxType.LeagueBox, userDb, accountDb,
                _seasonPassInfo, purchasedSeasons);

            await charger.Update();
            var infusionBox = await GetInfusionBoxInfo(userDb);
            if (infusionBox.UserEnergy == (int)Data.GetConstant(DataConstantType.MAX_ENERGY).Value)
            {
                var utcNow = DateTime.UtcNow;
                await charger.UpdateChargerDataNowDateTime(utcNow);
                infusionBox.UserEnergyRecentUpdateDatetime = utcNow.Ticks;
            }
            return infusionBox;
        }

        private async Task<ProtoInfusionBox> GetInfusionBoxInfo(DBContext userDb)
        {
            var query = new StringBuilder();
            query.Append("SELECT id, boxEnergy, userEnergy, userBonusEnergy, userEnergyRecentUpdateDatetime " +
                "FROM infusion_boxes WHERE userId =").Append(_req.UserId).Append(" AND type = 1;");

            var infusionBox = new ProtoInfusionBox();
            using (var cursor = await userDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return infusionBox;

                infusionBox.Id = (uint)cursor["id"];
                infusionBox.BoxEnergy = (int)cursor["boxEnergy"];
                infusionBox.UserEnergy = (int)cursor["userEnergy"];
                infusionBox.UserBonusEnergy = (int)cursor["userBonusEnergy"];
                infusionBox.UserEnergyRecentUpdateDatetime = ((DateTime)cursor["userEnergyRecentUpdateDatetime"]).Ticks;
            }
            return infusionBox;
        }
    }
}
