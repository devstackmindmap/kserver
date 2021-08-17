using AkaDB.MySql;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;
using System.Collections.Generic;
using AkaEnum;

namespace ProductUpdator
{
    public class ProductUpdate
    {
        DBContext _accountDb;
        StringBuilder _query = new StringBuilder();

        public ProductUpdate(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task Run()
        {
            _query.Clear();
            var utcNow = DateTime.UtcNow;
            var utcNowAddHours = DateTime.UtcNow.AddHours(3);
            _query.Append("SELECT productId, aosStoreProductId, iosStoreProductId, startDateTime, endDateTime, productTableType, storeType" +
                ", productType, countOfPurchases, materialType, saleCost, cost FROM _products_all_list " +
                "WHERE endDateTime > '").Append(utcNow.ToTimeString())
                .Append("' AND  startDateTime <= '").Append(utcNowAddHours.ToTimeString()).Append("';");

            List<Product> productAllList = new List<Product>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while(cursor.Read())
                {
                    productAllList.Add(new Product
                    {
                        ProductId = (uint)cursor["productId"],
                        AosStoreProductId = (string)cursor["aosStoreProductId"],
                        IosStoreProductId = (string)cursor["iosStoreProductId"],
                        StartDateTime = (DateTime)cursor["startDateTime"],
                        EndDateTime = (DateTime)cursor["endDateTime"],
                        ProductTableType = (ProductTableType)(int)cursor["productTableType"],
                        StoreType = (int)cursor["storeType"],
                        ProductType = (int)cursor["productType"],
                        CountOfPurchases = (int)cursor["countOfPurchases"],
                        MaterialType = (int)cursor["materialType"],
                        SaleCost = (int)cursor["saleCost"],
                        Cost = (int)cursor["cost"]
                    });
                }
            }

            await UpdateTables(productAllList);
        }

        private async Task UpdateTables(List<Product> products)
        {
            foreach (var product in products)
            {
                if (product.ProductTableType == ProductTableType.FixDigital)
                    await UpdateFixDigitalProduct(product);
                else if (product.ProductTableType == ProductTableType.EventDigital)
                    await UpdateEventDigitalProduct(product);
                else if (product.ProductTableType == ProductTableType.FixReal)
                    await UpdateFixRealProduct(product);
                else if (product.ProductTableType == ProductTableType.EventReal)
                    await UpdateEventRealProduct(product);
            }
        }

        private async Task UpdateFixDigitalProduct(Product product)
        {
            _query.Clear();

            _query.Append("INSERT INTO _products_fix_digital (productId, storeType, productType, " +
                " materialType, cost)" +"VALUES (")
                .Append(product.ProductId).Append(",").Append(product.StoreType)
                .Append(",").Append(product.ProductType).Append(",").Append(product.MaterialType)
                .Append(",").Append(product.Cost).Append(") " +
                "ON DUPLICATE KEY UPDATE storeType = ").Append(product.StoreType)
                .Append(", productType = ").Append(product.ProductType).Append(", materialType = ")
                .Append(product.MaterialType).Append(", cost = ").Append(product.Cost).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateEventDigitalProduct(Product product)
        {
            _query.Clear();

            _query.Append("INSERT INTO _products_event_digital (productId, startDateTime, endDateTime, storeType, productType" +
                ", materialType, saleCost, cost, countOfPurchases)" +
                "VALUES (").Append(product.ProductId).Append(",'")
                .Append(product.StartDateTime.ToTimeString()).Append("', '").Append(product.EndDateTime.ToTimeString()).Append("',")
                .Append(product.StoreType).Append(",").Append(product.ProductType).Append(",").Append(product.MaterialType)
                .Append(",").Append(product.SaleCost).Append(",").Append(product.Cost).Append(",").Append(product.CountOfPurchases).Append(") " +
                "ON DUPLICATE KEY UPDATE startDateTime = '").Append(product.StartDateTime.ToTimeString())
                .Append("', endDateTime = '").Append(product.EndDateTime.ToTimeString())
                .Append("', storeType = ").Append(product.StoreType).Append(", productType = ").Append(product.ProductType)
                .Append(", materialType = ").Append(product.MaterialType).Append(", saleCost = ").Append(product.SaleCost)
                .Append(", cost = ").Append(product.Cost).Append(", countOfPurchases = ").Append(product.CountOfPurchases).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateFixRealProduct(Product product)
        {
            _query.Clear();
            _query.Append("INSERT INTO _products_fix_real (productId, platform, " +
                "storeProductId, storeType, productType, cost)" +
                "VALUES (").Append(product.ProductId).Append(",").Append((int)PlatformType.Google)
                .Append(",'").Append(product.AosStoreProductId).Append("',")
                .Append(product.StoreType).Append(",").Append(product.ProductType)
                .Append(",").Append(product.Cost).Append(") " +
                "ON DUPLICATE KEY UPDATE platform = ").Append((int)PlatformType.Google)
                .Append(", storeProductId = '").Append(product.AosStoreProductId).Append("', storeType = ").Append(product.StoreType)
                .Append(", productType = ").Append(product.ProductType).Append(", cost = ").Append(product.Cost).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());

            _query.Clear();
            _query.Append("INSERT INTO _products_fix_real (productId, platform, " +
                "storeProductId, storeType, productType, cost)" +
                "VALUES (").Append(product.ProductId).Append(",").Append((int)PlatformType.Apple)
                .Append(",'").Append(product.IosStoreProductId).Append("',")
                .Append(product.StoreType).Append(",").Append(product.ProductType)
                .Append(",").Append(product.Cost).Append(") " +
                "ON DUPLICATE KEY UPDATE platform = ").Append((int)PlatformType.Apple)
                .Append(", storeProductId = '").Append(product.IosStoreProductId).Append("', storeType = ").Append(product.StoreType)
                .Append(", productType = ").Append(product.ProductType).Append(", cost = ").Append(product.Cost).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateEventRealProduct(Product product)
        {
            _query.Clear();
            _query.Append("INSERT INTO _products_event_real (productId, platform, storeProductId, startDateTime, endDateTime, storeType, " +
                "productType, saleCost, cost, countOfPurchases)" +
                "VALUES (").Append(product.ProductId).Append(",").Append((int)PlatformType.Google).Append(",'").Append(product.AosStoreProductId).Append("', '")
                .Append(product.StartDateTime.ToTimeString()).Append("', '").Append(product.EndDateTime.ToTimeString()).Append("',")
                .Append(product.StoreType).Append(",").Append(product.ProductType)
                .Append(",").Append(product.SaleCost).Append(",").Append(product.Cost).Append(",").Append(product.CountOfPurchases).Append(") " +
                "ON DUPLICATE KEY UPDATE platform = ").Append((int)PlatformType.Google)
                .Append(", storeProductId = '").Append(product.AosStoreProductId).Append("', startDateTime = '")
                .Append(product.StartDateTime.ToTimeString()).Append("', endDateTime = '").Append(product.EndDateTime.ToTimeString())
                .Append("', storeType = ").Append(product.StoreType)
                .Append(", productType = ").Append(product.ProductType).Append(", saleCost = ").Append(product.SaleCost)
                .Append(", cost = ").Append(product.Cost).Append(", countOfPurchases = ").Append(product.CountOfPurchases).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());

            _query.Clear();
            _query.Append("INSERT INTO _products_event_real (productId, platform, storeProductId, startDateTime, endDateTime, storeType, " +
                "productType, saleCost, cost, countOfPurchases)" +
                "VALUES (").Append(product.ProductId).Append(",").Append((int)PlatformType.Apple).Append(",'").Append(product.IosStoreProductId).Append("', '")
                .Append(product.StartDateTime.ToTimeString()).Append("', '").Append(product.EndDateTime.ToTimeString()).Append("',")
                .Append(product.StoreType).Append(",").Append(product.ProductType)
                .Append(",").Append(product.SaleCost).Append(",").Append(product.Cost).Append(",").Append(product.CountOfPurchases).Append(") " +
                "ON DUPLICATE KEY UPDATE platform = ").Append((int)PlatformType.Apple)
                .Append(", storeProductId = '").Append(product.IosStoreProductId).Append("', startDateTime = '")
                .Append(product.StartDateTime.ToTimeString()).Append("', endDateTime = '").Append(product.EndDateTime.ToTimeString())
                .Append("', storeType = ").Append(product.StoreType)
                .Append(", productType = ").Append(product.ProductType).Append(", saleCost = ").Append(product.SaleCost)
                .Append(", cost = ").Append(product.Cost).Append(", countOfPurchases = ").Append(product.CountOfPurchases).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());

        }
    }
}
