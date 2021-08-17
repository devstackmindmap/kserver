using AkaConfig;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public abstract class ProductBuy
    {
        protected DBContext _accountDb;
        protected DBContext _userDb;
        protected uint _userId;
        protected uint _productId;
        protected StringBuilder _query = new StringBuilder();
        protected ProductBuyCondition _productBuyCondition;
        protected int _finalCost;

        public ProductBuy(DBContext accountDb, DBContext userDb, uint userId, uint productId)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _productId = productId;
        }

        public async Task<(uint productId, int countOfPurchases)> GetUserProduct(DBContext userDb, uint productId)
        {
            _query.Clear();
            _query.Append
                ("SELECT productId, countOfPurchases " +
                "FROM product_purchases WHERE userId = ")
                .Append(_userId).Append(" AND productId = ").Append(productId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return (0, 0);

                return ((uint)cursor["productId"], (int)cursor["countOfPurchases"]);
            }
        }

        public async Task<(uint productId, int countOfPurchases)> GetUserProductDigital(DBContext userDb)
        {
            _query.Clear();
            _query.Append
                ("SELECT productId, countOfPurchases " +
                "FROM product_purchases WHERE userId = ")
                .Append(_userId).Append(" AND productId = ").Append(_productId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return (0, 0);

                return ((uint)cursor["productId"], (int)cursor["countOfPurchases"]);
            }
        }

        public abstract Task<bool> SetProductBuyCondition();

        public abstract Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId);

        protected abstract void SetCost();

        protected abstract Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId);

        protected bool IsValidDateTime()
        {
            var now = DateTime.UtcNow;
            return _productBuyCondition.StartDateTime <= now && now < _productBuyCondition.EndDateTime.AddMinutes(2);
        }

        public bool GetReceipt(ProtoStoreInfo storeInfo
            , out ReceiptCheckResponse receiptCheckResponse)
        {
            var check = new ReceiptCheckRequest
            {
                userId = _userId,
                platform = (int)storeInfo.PlatformType,
                productID = storeInfo.StoreProductId,
                purchaseToken = storeInfo.PurchaseToken,
                transaction_id = storeInfo.TransactionId
            };

            receiptCheckResponse = null;
            string jsonString = JsonConvert.SerializeObject(check);
            if (false == JsonRequestor.Request(Config.GameServerConfig.InAppUrl, jsonString, out var result))
                return false;

            if (result == null)
                return false;

            var receiptResult = JsonConvert.DeserializeObject<ReceiptCheckResponse>(result);
            receiptCheckResponse = receiptResult;
            return true;
        }

        public abstract Task UseMaterial();

        public abstract Task AddPurchaseCount(DBContext userDb, uint productId);

        public abstract Task<(MaterialType materialType, int remainCount)> GetMaterialInfo();

        public abstract Task AddPurchased(DBContext accountDb, string puchaseToken, string storeProductId);

        public abstract Task<bool> IsAlreadyGivenItems(DBContext accountDb, string puchaseToken);
    }
}
