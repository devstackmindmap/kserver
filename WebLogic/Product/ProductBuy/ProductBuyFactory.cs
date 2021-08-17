using AkaDB.MySql;
using AkaEnum;

namespace WebLogic.Store
{
    public static class ProductBuyFactory
    {
        public static ProductBuy CreateProductBuy(DBContext accountDb, DBContext userDb, uint userId, uint productId, 
            ProductTableType productTableType, PlatformType platformType = PlatformType.None)
        {
            switch (productTableType)
            {
                case ProductTableType.EventDigital:
                    return new ProductBuyEventDigital(accountDb, userDb, userId, productId);
                case ProductTableType.EventReal:
                    return new ProductBuyEventReal(accountDb, userDb, userId, productId, platformType);
                case ProductTableType.FixDigital:
                    return new ProductBuyFixDigital(accountDb, userDb, userId, productId);
                case ProductTableType.FixReal:
                    return new ProductBuyFixReal(accountDb, userDb, userId, productId, platformType);
                case ProductTableType.UserDigital:
                    return new ProductBuyUserDigital(accountDb, userDb, userId, productId);
                case ProductTableType.UserReal:
                    return new ProductBuyUserReal(accountDb, userDb, userId, productId, platformType);
                case ProductTableType.SeasonPass:
                    return new ProductBuyUserRealSeasonPass(accountDb, userDb, userId, productId);
                default:
                    throw new System.Exception("Invalid ProductTableType");
            }
        }
    }
}
