using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public class ProductBuyUserDigital : ProductBuyDigital
    {
        public ProductBuyUserDigital(DBContext accountDb, DBContext userDb, uint userId, uint productId) 
            : base (accountDb, userDb, userId, productId)
        {
        }

        public override async Task<bool> SetProductBuyCondition()
        {
            _productBuyCondition = await SetProductBuyConditionByCommonData();
            if (_productBuyCondition == null)
                return false;

            return await SetProductBuyConditionByUserData(_productBuyCondition);
        }
        private async Task<ProductBuyCondition> SetProductBuyConditionByCommonData()
        {
            _query.Clear();
            _query.Append("SELECT productId, materialType, saleCost, cost, countOfPurchases " +
                "FROM _products_user_digital WHERE productId = ").Append(_productId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return null;

                return  new ProductBuyCondition
                {
                    MaterialType = (MaterialType)(int)cursor["materialType"],
                    SaleCost = (int)cursor["saleCost"],
                    Cost = (int)cursor["cost"],
                    CountOfPurchases = (int)cursor["countOfPurchases"]
                };
            }
        }

        private async Task<bool> SetProductBuyConditionByUserData(ProductBuyCondition productBuyCondition)
        {
            _query.Clear();
            _query.Append("SELECT startDateTime, endDateTime FROM products_user WHERE userId = ").Append(_userId)
                .Append(" AND productId = ").Append(_productId).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                productBuyCondition.StartDateTime = (DateTime)cursor["startDateTime"];
                productBuyCondition.EndDateTime = (DateTime)cursor["endDateTime"];
            }
            return true;
        }

        public override async Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId)
        {
            var resultType = await base.CheckBuy(accountDb, userDb, productId);
            if (resultType != ResultType.Success)
                return resultType;

            if (false == IsValidDateTime())
                return ResultType.InvalidDateTime;

            if (await IsInvalidPurchaseCount(userDb, productId))
                return ResultType.InvalidPurchaseCount;

            return ResultType.Success;
        }

        protected override void SetCost()
        {
            _finalCost = _productBuyCondition.SaleCost;
        }

        protected override async Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId)
        {
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
