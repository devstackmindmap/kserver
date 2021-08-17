using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public class ProductBuyUserRealSeasonPass : ProductBuyReal
    {

        public ProductBuyUserRealSeasonPass(DBContext accountDb, DBContext userDb, uint userId, uint productId) 
            : base(accountDb, userDb, userId, productId)
        {
        }

        public override async Task<bool> SetProductBuyCondition()
        {
            return true;
        }

        protected override void SetCost()
        {
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

        public override Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId)
        {
            throw new NotImplementedException();
        }
    }
}
