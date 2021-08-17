using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public class ProductBuyFixDigital : ProductBuyDigital
    {
        public ProductBuyFixDigital(DBContext accountDb, DBContext userDb, uint userId, uint productId) 
            : base(accountDb, userDb, userId, productId)
        {
        }

        public override async Task<bool> SetProductBuyCondition()
        {
            _query.Clear();
            _query.Append("SELECT materialType, cost FROM _products_fix_digital " +
                "WHERE productId = ").Append(_productId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                _productBuyCondition =  new ProductBuyCondition
                {
                    MaterialType = (MaterialType)(int)cursor["materialType"],
                    Cost = (int)cursor["cost"],
                    CountOfPurchases = 0
                };
            }
            return true;
        }

        protected override void SetCost()
        {
            _finalCost = _productBuyCondition.Cost;
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
