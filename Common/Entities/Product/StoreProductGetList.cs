using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Product
{
    public class StoreProductGetList
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private uint _userId;
        private StringBuilder _query = new StringBuilder();

        public StoreProductGetList(DBContext accountDb, DBContext userDb, uint userId)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
        }

        public async Task<List<uint>> GetProducts(PlatformType platformType, string languageType, ProtoOnGetProducts products)
        {
            List<uint> productIds = new List<uint>();
            var utcNow = DateTime.UtcNow.ToTimeString();
            var addUtcNow = DateTime.UtcNow.AddHours(4).ToTimeString();
            await GetCustomProducts(platformType, languageType, utcNow, addUtcNow, productIds, products);
            await GetCommonProducts(platformType, languageType, utcNow, addUtcNow, productIds, products);

            await GetUserProducts(products, productIds);
            return productIds;
        }

        private async Task GetCustomProducts(PlatformType platformType, string languageType, string utcNow, string addUtcNow, 
            List<uint> productIds, ProtoOnGetProducts products)
        {
            var digitalProductIds = new List<uint>();
            var realProductIds = new List<uint>();
            _query.Clear();
            _query.Append("SELECT productId, startDateTime, endDateTime, productTableType " +
                "FROM products_user WHERE userId = ").Append(_userId).Append(" AND endDateTime > '")
                .Append(utcNow).Append("' AND startDateTime <= '").Append(addUtcNow).Append("';");

            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var productTableType = (ProductTableType)(int)cursor["productTableType"];
                    var productId = (uint)cursor["productId"];
                    productIds.Add(productId);
                    if (productTableType == ProductTableType.UserDigital)
                    {
                        digitalProductIds.Add(productId);
                        products.ProductUserDigitals.Add(new ProtoProductEventDigital
                        {
                            ProductId = productId,
                            StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                            EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks
                        });
                    }
                    else if (productTableType == ProductTableType.UserReal)
                    {
                        realProductIds.Add(productId);
                        products.ProductUserReals.Add(new ProtoProductEventReal
                        {
                            ProductId = productId,
                            StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                            EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks
                        });
                    }
                    else
                    {
                        throw new Exception("Wrong ProductTableType");
                    }
                }
            }

            if (digitalProductIds.Count > 0)
                await GetDigitalUserProducts(digitalProductIds, languageType, products);

            if (realProductIds.Count > 0)
                await GetRealUserProducts(realProductIds, platformType, languageType, products);
        }

        private async Task GetDigitalUserProducts(List<uint> productIds, string languageType, ProtoOnGetProducts products)
        {
            _query.Clear();
            _query.Append("SELECT a.productId, a.storeType, a.productType, a.materialType, " +
                "a.saleCost, a.cost, a.countOfPurchases, ifnull(b.productText, '') AS productText " +
                "FROM _products_user_digital a " +
                "LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = '")
                .Append(languageType).Append("' WHERE a.productId IN (").Append(string.Join(",", productIds))
                .Append(");");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var productId = (uint)cursor["productId"];
                    foreach(var product in products.ProductUserDigitals)
                    {
                        if (product.ProductId == productId)
                        {
                            product.StoreType = (StoreType)(int)cursor["storeType"];
                            product.ProductType = (ProductType)(int)cursor["productType"];
                            product.MaterialType = (MaterialType)(int)cursor["materialType"];
                            product.SaleCost = (int)cursor["saleCost"];
                            product.Cost = (int)cursor["cost"];
                            product.CountOfPurchases = (int)cursor["countOfPurchases"];
                            product.ProductText = (string)cursor["productText"];
                        }
                    }
                }
            }
        }

        private async Task GetRealUserProducts(List<uint> productIds, PlatformType platformType, string languageType, ProtoOnGetProducts products)
        {
            _query.Clear();
            _query.Append("SELECT a.productId, a.storeProductId, a.storeType, a.productType, " +
                "a.saleCost, a.cost, a.countOfPurchases, ifnull(b.productText, '') AS productText " +
                "FROM _products_user_real a " +
                "LEFT OUTER JOIN _products_text b ON b.productId = a.productId AND b.languageType = '")
                .Append(languageType).Append("' WHERE a.productId IN (").Append(string.Join(",", productIds))
                .Append(") AND a.platform = ").Append((int)platformType).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    var productId = (uint)cursor["productId"];
                    foreach (var product in products.ProductUserReals)
                    {
                        if (product.ProductId == productId)
                        {
                            product.StoreProductId = (string)cursor["storeProductId"];
                            product.StoreType = (StoreType)(int)cursor["storeType"];
                            product.ProductType = (ProductType)(int)cursor["productType"];
                            product.SaleCost = (int)cursor["saleCost"];
                            product.Cost = (int)cursor["cost"];
                            product.CountOfPurchases = (int)cursor["countOfPurchases"];
                            product.ProductText = (string)cursor["productText"];
                        }
                    }
                }
            }
        }

        private async Task GetCommonProducts(PlatformType platformType, string languageType, string utcNow, string addUtcNow, 
            List<uint> productIds, ProtoOnGetProducts products)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$platform", (int)platformType),
                new InputArg("$utcNow", utcNow),
                new InputArg("$utcNowAddHour", addUtcNow),
                new InputArg("$languageType", languageType));
            using (var cursor = await _accountDb.CallStoredProcedureAsync(StoredProcedure.GET_PRODUCTS, paramInfo))
            {
                GetEventDigital(cursor, products, productIds);
                GetEventReal(cursor, products, productIds);
                GetFixDigital(cursor, products, productIds);
                GetFixReal(cursor, products, productIds);
            }
        }

        private void GetEventDigital(DbDataReader cursor, ProtoOnGetProducts products, List<uint> productIds)
        {
            while (cursor.Read())
            {
                uint productId = (uint)cursor["productId"];
                products.ProductEventDigitals.Add(new ProtoProductEventDigital
                {
                    ProductId = productId,
                    StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                    EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                    StoreType = (StoreType)(int)cursor["storeType"],
                    ProductType = (ProductType)(int)cursor["productType"],
                    ProductText = (string)cursor["productText"],
                    MaterialType = (MaterialType)(int)cursor["materialType"],
                    SaleCost = (int)cursor["saleCost"],
                    Cost = (int)cursor["cost"],
                    CountOfPurchases = (int)cursor["countOfPurchases"]
                });
                productIds.Add(productId);
            }
        }

        private void GetEventReal(DbDataReader cursor, ProtoOnGetProducts products, List<uint> productIds)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    uint productId = (uint)cursor["productId"];
                    products.ProductEventReals.Add(new ProtoProductEventReal
                    {
                        ProductId = productId,
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                        StoreProductId = (string)cursor["storeProductId"],
                        StoreType = (StoreType)(int)cursor["storeType"],
                        ProductType = (ProductType)(int)cursor["productType"],
                        ProductText = (string)cursor["productText"],
                        SaleCost = (int)cursor["saleCost"],
                        Cost = (int)cursor["cost"],
                        CountOfPurchases = (int)cursor["countOfPurchases"]
                    });
                    productIds.Add(productId);
                }
            }
        }

        private void GetFixDigital(DbDataReader cursor, ProtoOnGetProducts products, List<uint> productIds)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    uint productId = (uint)cursor["productId"];
                    products.ProductFixDigitals.Add(new ProtoProductFixDigital
                    {
                        ProductId = productId,
                        StoreType = (StoreType)(int)cursor["storeType"],
                        ProductType = (ProductType)(int)cursor["productType"],
                        ProductText = (string)cursor["productText"],
                        MaterialType = (MaterialType)(int)cursor["materialType"],
                        Cost = (int)cursor["cost"]
                    });
                    productIds.Add(productId);
                }
            }
        }

        private void GetFixReal(DbDataReader cursor, ProtoOnGetProducts products, List<uint> productIds)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    uint productId = (uint)cursor["productId"];
                    products.ProductFixReals.Add(new ProtoProductFixReal
                    {
                        ProductId = productId,
                        StoreProductId = (string)cursor["storeProductId"],
                        StoreType = (StoreType)(int)cursor["storeType"],
                        ProductType = (ProductType)(int)cursor["productType"],
                        ProductText = (string)cursor["productText"],
                        Cost = (int)cursor["cost"]
                    });
                    productIds.Add(productId);
                }
            }
        }

        private async Task GetUserProducts(ProtoOnGetProducts products, List<uint> productIds)
        {
            if (productIds.Count == 0)
                return;

            var strProductIds = string.Join(",", productIds);

            _query.Clear();
            _query.Append("SELECT productId, countOfPurchases FROM product_purchases WHERE userId = ").Append(_userId)
                .Append(" AND productId IN (").Append(strProductIds).Append(");");

            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    products.Purchases.Add(new ProtoPurchase
                    {
                        ProductId = (uint)cursor["productId"],
                        CountOfPurchases = (int)cursor["countOfPurchases"]
                    });
                }
            }
        }
    }
}
