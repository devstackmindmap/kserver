using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public class ProductBuyFixReal : ProductBuyReal
    {
        private PlatformType _platformType;

        public ProductBuyFixReal(DBContext accountDb, DBContext userDb, uint userId, uint productId, PlatformType platformType) 
            : base(accountDb, userDb, userId, productId)
        {
            _platformType = platformType;
        }

        public override async Task<bool> SetProductBuyCondition()
        {
            _productBuyCondition = new ProductBuyCondition
            {
                MaterialType = MaterialType.None,
                Cost = 0,
                CountOfPurchases = 0
            };
            
            return true;
        }

        public override async Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId)
        {
            return ResultType.Success;
        }

        protected override void SetCost()
        {
            _finalCost = 0;
        }

        protected override async Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId)
        {
            return false;
        }

        public override async Task AddPurchaseCount(DBContext userDb, uint productId)
        {
        }

    }
}
