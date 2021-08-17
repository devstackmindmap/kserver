using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public class ProductBuyEventReal : ProductBuyReal
    {
        private PlatformType _platformType;

        public ProductBuyEventReal(DBContext accountDb, DBContext userDb, uint userId, uint productId, PlatformType platformType) 
            : base(accountDb, userDb, userId, productId)
        {
            _platformType = platformType;
        }

        public override async Task<bool> SetProductBuyCondition()
        {
            _query.Clear();
            _query.Append("SELECT startDateTime, endDateTime, countOfPurchases " +
                "FROM _products_event_real WHERE productId = ").Append(_productId)
                .Append(" AND platform = ").Append((int)_platformType).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                _productBuyCondition = new ProductBuyCondition
                {
                    StartDateTime = (DateTime)cursor["startDateTime"],
                    EndDateTime = (DateTime)cursor["endDateTime"],
                    MaterialType = MaterialType.None,
                    SaleCost = 0,
                    Cost = 0,
                    CountOfPurchases = (int)cursor["countOfPurchases"]
                };
            }
            return true;
        }

        public override async Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId)
        {
            if (false == IsValidDateTime())
                return ResultType.InvalidDateTime;

            if (await IsInvalidPurchaseCount(userDb, productId))
                return ResultType.InvalidPurchaseCount;

            return ResultType.Success;
        }

        protected override void SetCost()
        {
            _finalCost = 0;
        }

        protected override async Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId)
        {
            if (_productBuyCondition.CountOfPurchases == 0)
                return false;

            var userProduct = await GetUserProduct(userDb, productId);
            return _productBuyCondition.CountOfPurchases <= userProduct.countOfPurchases;
        }

        public override async Task AddPurchaseCount(DBContext userDb, uint productId)
        {
            _query.Clear();
            _query.Append("INSERT INTO product_purchases (userId, productId, countOfPurchases) " +
                "VALUES (").Append(_userId).Append(",").Append(productId).Append(", 1)" +
                "ON DUPLICATE KEY UPDATE countOfPurchases = countOfPurchases + 1");

            await userDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
