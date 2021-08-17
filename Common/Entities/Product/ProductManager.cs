using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Product
{
    public class ProductManager
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private StringBuilder _query = new StringBuilder();
        private uint _userId;

        public ProductManager(DBContext accountDb, DBContext userDb, uint userId)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
        }

        public async Task GetStoreProducts(PlatformType platformType, string languageType, ProtoOnGetProducts products)
        {
            var productGetList = new StoreProductGetList(_accountDb, _userDb, _userId);
            var productIds = await productGetList.GetProducts(platformType, languageType, products);
            products.Products = await GetProductInfos(productIds);
        }

        public async Task<Dictionary<uint, List<ProtoReward>>> GetProductInfos(List<uint> productIds)
        {
            var products = new Dictionary<uint, List<ProtoReward>>();

            foreach (var productId in productIds)
            {
                var rewards = await GetRewards(productId);
                foreach (var reward in rewards)
                {
                    reward.Items = await GetListItemsByDb(_accountDb, reward.RewardId);
                }

                if (false == products.ContainsKey(productId))
                    products.Add(productId, rewards);
            }
            return products;
        }

        private async Task<List<ProtoReward>> GetRewards(uint productId)
        {
            var rewards = new List<ProtoReward>();
            _query.Clear();
            _query.Append("SELECT rewardId, rewardType FROM _products WHERE productId = ").Append(productId).Append(";");
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    rewards.Add(new ProtoReward
                    {
                        RewardId = (uint)cursor["rewardId"],
                        RewardType = (RewardType)(int)cursor["rewardType"]
                    });
                }
            }
            return rewards;
        }

        public async Task<List<List<ProtoItem>>> GetListItemsByDb(DBContext accountDb, uint rewardId)
        {
            List<List<ProtoItem>> _listItems = new List<List<ProtoItem>>();

            _query.Clear();
            List<uint> itemIds = new List<uint>();
            _query.Append("SELECT itemId FROM _rewards WHERE rewardId = ").Append(rewardId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    itemIds.Add((uint)cursor["itemId"]);
                }
            }

            foreach (var itemId in itemIds)
            {
                var dataItems = await GetListDataItems(accountDb, itemId);
                _listItems.Add(dataItems);
            }

            return _listItems;
        }

        private async Task<List<ProtoItem>> GetListDataItems(DBContext accountDb, uint itemId)
        {
            _query.Clear();
            List<ProtoItem> dataItems = new List<ProtoItem>();
            _query.Append("SELECT id, itemType, minCount, maxCount, probability " +
                "FROM _items WHERE itemId = ").Append(itemId).Append(";");
            using (var cursor = await accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    dataItems.Add(new ProtoItem
                    {
                        ClassId = (uint)cursor["id"],
                        ItemType = (ItemType)(int)cursor["itemType"],
                        MaxNumber = (int)cursor["maxCount"],
                        MinNumber = (int)cursor["minCount"]
                    });
                }
            }
            return dataItems;
        }
    }
}
