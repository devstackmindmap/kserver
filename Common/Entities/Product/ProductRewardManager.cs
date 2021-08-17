using AkaData;
using AkaDB.MySql;
using CommonProtocol;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Product
{
    public class ProductRewardManager
    {
        public static async Task<List<List<ProtoItemResult>>> GiveProduct(DBContext accountDb, DBContext userDb, 
            uint userId, uint itemValue, uint productId, string logCategory)
        {
            var itemResults = new List<List<ProtoItemResult>>();
            var rewardIds = await GetRewards(accountDb, productId);
            foreach (var rewardId in rewardIds)
            {
                var items = await Reward.Reward.GetRewardsByDb(userId, rewardId, accountDb, userDb, logCategory, itemValue);
                if (items.Count > 0)
                    itemResults.Add(items);
            }

            return itemResults;
        }

        public static async Task<List<List<ProtoItemResult>>> GiveProduct(DBContext userDb, uint userId, uint itemValue, uint productId, string logCategory)
        {
            var itemResults = new List<List<ProtoItemResult>>();
            var rewardIds = Data.GetProduct(productId).RewardIdList;

            foreach (var rewardId in rewardIds)
            {
                var items = await Reward.Reward.GetRewards(userDb, userId, rewardId, logCategory, itemValue);
                if (items.Count > 0)
                    itemResults.Add(items);
            }

            return itemResults;
        }

        private static async Task<List<uint>> GetRewards(DBContext accountDb, uint productId)
        {
            var rewardIds = new List<uint>();
            var query = new StringBuilder();
            query.Append("SELECT rewardId FROM _products WHERE productId = ").Append(productId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    rewardIds.Add((uint)cursor["rewardId"]);
                }
            }
            return rewardIds;
        }

        public static bool IsEmptyItem(List<List<ProtoItemResult>> itemResults)
        {
            return (itemResults == null ||
                itemResults.Count == 0 || 
                (itemResults.Count == 1 && itemResults[0].Count == 0));
        }
    }
}
