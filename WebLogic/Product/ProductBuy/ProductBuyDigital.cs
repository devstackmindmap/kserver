using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using System.Threading.Tasks;

namespace WebLogic.Store
{
    public abstract class ProductBuyDigital : ProductBuy
    {
        private Material _material;

        public ProductBuyDigital(DBContext accountDb, DBContext userDb, uint userId, uint productId) 
            : base (accountDb, userDb, userId, productId)
        {
        }

        public override abstract Task<bool> SetProductBuyCondition();

        public override async Task<ResultType> CheckBuy(DBContext accountDb, DBContext userDb, uint productId)
        {
            SetCost();

            var resultType = await CheckEnoughCountAndSetMaterial(userDb);
            if (resultType != ResultType.Success)
                return resultType;

            return ResultType.Success;
        }

        protected override abstract void SetCost();

        protected override abstract Task<bool> IsInvalidPurchaseCount(DBContext userDb, uint productId);

        private async Task<ResultType> CheckEnoughCountAndSetMaterial(DBContext userDb)
        {
            _material = MaterialFactory.CreateMaterial(_productBuyCondition.MaterialType, _userId, _finalCost, userDb);
            return  await _material.IsEnoughCount() ? ResultType.Success : ResultType.NeedMaterial;
        }

        public override async Task UseMaterial()
        {
            await _material.Use("ProductBuyDigital");
        }

        public override async Task<(MaterialType materialType, int remainCount)> GetMaterialInfo()
        {
            return (_material.GetMaterialType(), await _material.GetRemainCount());
        }

        public override async Task AddPurchased(DBContext accountDb, string transactionId, string storeProductId)
        {

        }

        public override async Task<bool> IsAlreadyGivenItems(DBContext accountDb, string transactionId)
        {
            return true;
        }
    }
}
