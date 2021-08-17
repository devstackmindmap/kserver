using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using System;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public abstract class ProductBuyReal : ProductBuy
    {
        public ProductBuyReal(DBContext accountDb, DBContext userDb, uint userId, uint productId) 
            : base(accountDb, userDb, userId, productId)
        {
        }

        public override abstract Task<bool> SetProductBuyCondition();

        protected override abstract void SetCost();

        protected override abstract Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId);

        public override async Task UseMaterial()
        {
        }

        public override async Task<(MaterialType materialType, int remainCount)> GetMaterialInfo()
        {
            return (MaterialType.None, 0);
        }

        public override async Task AddPurchased(DBContext accountDb, string transactionId, string storeProductId)
        {
            _query.Clear();
            _query.Append("INSERT INTO purchased (userId, transactionId, storeProductId, addDateTime) VALUES (")
                .Append(_userId).Append(", '").Append(transactionId).Append("', '").Append(storeProductId)
                .Append("', '").Append(DateTime.UtcNow.ToTimeString()).Append("');");

            await accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public override async Task<bool> IsAlreadyGivenItems(DBContext accountDb, string transactionId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM purchased WHERE transactionId = '").Append(transactionId).Append("';");

            using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }
    }
}
