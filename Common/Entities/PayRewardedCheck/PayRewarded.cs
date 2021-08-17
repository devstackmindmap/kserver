using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.PayRewardedCheck
{
    public class PayRewarded
    {
        private DBContext _accountDb;
        private uint _userId;

        public PayRewarded(DBContext accountDb, uint userId)
        {
            _accountDb = accountDb;
            _userId = userId;
        }

        public async Task SetStoreInfo(ProtoStoreInfo storeInfo, int isPending)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO pay_pending (userId, transactionId, storeProductId, purchasedToken" +
                ", is_pending, productId, productTableType, platformType, payedTime) ")
                .Append(" VALUES (").Append(_userId).Append(",'").Append(storeInfo.TransactionId).Append("','").Append(storeInfo.StoreProductId)
                .Append("','").Append(storeInfo.PurchaseToken).Append("',").Append(isPending).Append(",")
                .Append(storeInfo.ProductId).Append(",").Append((int)storeInfo.ProductTableType).Append(",").Append((int)storeInfo.PlatformType)
                .Append(", '").Append(DateTime.UtcNow.ToTimeString()).Append("' ")
                .Append(") ON DUPLICATE KEY UPDATE is_pending = ").Append(isPending);

            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task SetIssueStoreInfo(ProtoStoreInfo storeInfo, int isPending)
        {
            var query = new StringBuilder();
            query.Append("INSERT INTO pay_pending_issue (userId, transactionId, storeProductId, purchasedToken" +
                ", is_pending, productId, productTableType, platformType) ")
                .Append(" VALUES (").Append(_userId).Append(",'").Append(storeInfo.TransactionId).Append("','").Append(storeInfo.StoreProductId)
                .Append("','").Append(storeInfo.PurchaseToken).Append("',").Append(isPending).Append(",")
                .Append(storeInfo.ProductId).Append(",").Append((int)storeInfo.ProductTableType).Append(",").Append((int)storeInfo.PlatformType)
                .Append(") ON DUPLICATE KEY UPDATE is_pending = ").Append(isPending);

            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<List<ProtoStoreInfo>> GetPendingStoreInfos()
        {
            var query = new StringBuilder();
            query.Append("SELECT storeProductId, purchasedToken, transactionId, productId, productTableType, platformType ")
                .Append("FROM pay_pending WHERE userId = ").Append(_userId).Append(" AND is_pending = 1;");

            var pendingStoreInfos = new List<ProtoStoreInfo>();

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    pendingStoreInfos.Add(new ProtoStoreInfo
                    {
                        PurchaseToken = (string)cursor["purchasedToken"],
                        StoreProductId = (string)cursor["storeProductId"],
                        TransactionId = (string)cursor["transactionId"],
                        ProductId = 0,
                        ProductTableType = (ProductTableType)(int)cursor["productTableType"],
                        PlatformType = (PlatformType)(int)cursor["platformType"]
                    });
                }
            }
            return pendingStoreInfos;
        }

        public async Task<bool> GetProductInfoByTransactionId(ProtoStoreInfo storeInfo)
        {
            var query = new StringBuilder();
            query.Append("SELECT storeProductId, purchasedToken, platformType, " +
                " productId, productTableType, is_pending " +
                "FROM pay_pending WHERE userId = ").Append(_userId)
                .Append(" AND transactionId = '").Append(storeInfo.TransactionId).Append("';");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                storeInfo.ProductId = (uint)cursor["productId"];
                storeInfo.ProductTableType = (ProductTableType)(int)cursor["productTableType"];
                storeInfo.PlatformType = (PlatformType)(int)cursor["platformType"];
                storeInfo.StoreProductId = (string)cursor["storeProductId"];
                storeInfo.PurchaseToken = (string)cursor["purchasedToken"];

                return true;
            }
        }
    }
}
