using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Box;
using CommonProtocol;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Reward
{
    public class Reward
    {
        public static async Task<List<ProtoItemResult>> GetRewards(DBContext userDb, uint userId, uint rewardId, string logCategory, uint itemValue = 0)
        {
            var listItems = GetListItems(rewardId);

            var box = RewardFactory.CreateBox(userDb, userId, rewardId, listItems, itemValue);
            var rewards = await box.GetReward();
            await box.SetReward(rewards, logCategory);
            return rewards;
        }

        public static async Task<List<ProtoItemResult>> GetRewardsByDb(uint userId, uint rewardId, DBContext accountDb, DBContext db, string logCategory, uint itemValue)
        {
            var listItems = await GetListItemsByDb(accountDb, rewardId);

            var box = RewardFactory.CreateBox(db, userId, rewardId, listItems, itemValue);
            var rewards = await box.GetReward();
            await box.SetReward(rewards, logCategory);
            return rewards;
        }

        public static Box.Box GetBox(uint userId, uint rewardId, DBContext db)
        {
            var listItems = GetListItems(rewardId);

            return new Box.Box(db, userId, rewardId, listItems, 0);
        }


        private static List<List<DataItem>> GetListItems(uint rewardId)
        {
            List<List<DataItem>> _listItems = new List<List<DataItem>>();
            var reward = Data.GetReward(rewardId);

            if (reward != null)
            {
                foreach (var items in reward.ItemIdList)
                {
                    var listItems = Data.GetItem(items).Select(item => Utility.CopyFrom(item)).ToList();
                    _listItems.Add(listItems);
                }
            }
            return _listItems;
        }

        public static async Task<List<List<DataItem>>> GetListItemsByDb(DBContext accountDb, uint rewardId)
        {
            List<List<DataItem>> _listItems = new List<List<DataItem>>();
            
            var query = new StringBuilder();
            List<uint> itemIds = new List<uint>();
            query.Append("SELECT itemId FROM _rewards WHERE rewardId = ").Append(rewardId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                while(cursor.Read())
                {
                    itemIds.Add((uint)cursor["itemId"]);
                }
            }
            
            foreach(var itemId in itemIds)
            {
                var dataItems = await GetListDataItems(accountDb, itemId);
                _listItems.Add(dataItems);
            }

            return _listItems;
        }

        private static async Task<List<DataItem>> GetListDataItems(DBContext accountDb, uint itemId)
        {
            var query = new StringBuilder();
            List<DataItem> dataItems = new List<DataItem>();
            query.Append("SELECT id, itemType, minCount, maxCount, probability " +
                "FROM _items WHERE itemId = ").Append(itemId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    dataItems.Add(new DataItem
                    {
                        ClassId = (uint)cursor["id"],
                        ItemType = (ItemType)(int)cursor["itemType"],
                        MaxNumber = (int)cursor["maxCount"],
                        MinNumber = (int)cursor["minCount"],
                        Probability = (int)cursor["probability"]
                    });
                }
            }
            return dataItems;
        }
    }
}
