using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using CommonProtocol;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.ItemExtractor
{
    public abstract  class ItemExtractor : IItemExtractor
    {
        protected uint _userId;
        protected string _tableName;
        protected readonly DBContext _db;
        protected List<ProtoItemResult> retItems = new List<ProtoItemResult>();
        private List<uint> haveItems = null;

        public abstract Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue);

        public virtual async Task Extract(List<DataItem> dataItems, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {           
        }

        public ItemExtractor(uint userId, string tableName, DBContext db)
        {
            _userId = userId;
            _tableName = tableName;
            _db = db;
        }

        public List<ProtoItemResult> GetResultItems()
        {
            return retItems;
        }

        protected ItemType GetPieceUnlockRandomByPieceRandom(ItemType itemType)
        {
            switch(itemType)
            {
                case ItemType.UnitPieceRandom:
                    return ItemType.UnitPieceUnlockRandom;
                case ItemType.CardPieceRandom:
                    return ItemType.CardPieceUnlockRandom;
                case ItemType.WeaponPieceRandom:
                    return ItemType.WeaponPieceUnlockRandom;
                default:
                    throw new System.Exception("Wrong ItemType :" + itemType.ToString());
            }
        }

        protected async Task<List<uint>> GetItems()
        {
            if (haveItems != null)
                return haveItems;

            haveItems =  new List<uint>();
            var strUserId = _userId;
            var query = new StringBuilder();
            query.Append("SELECT id FROM ").Append(_tableName).Append(" WHERE userId = ").Append(strUserId).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    haveItems.Add((uint)cursor.GetInt32(0));
                }
            }
            return haveItems;
        }

        protected async Task<bool> IsHave(uint id)
        {
            var query = new StringBuilder();
            query.Append("SELECT id FROM ").Append(_tableName).Append(" WHERE userId = ").Append(_userId)
                .Append(" AND id=").Append(id).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }

        protected bool IsNeedToChangePieceToGold(int addCount, int needPieceCountToMaxLevel)
        {
            return addCount > needPieceCountToMaxLevel;
        }

        protected void AddReturnItem(ItemType itemType, uint id, int count)
        {
            retItems.Add(new ProtoItemResult
            {
                ClassId = id,
                Count = count,
                ItemType = itemType
            });
        }

        protected void AddLevel1Card(uint unitId)
        {
            var level1CardIds = Data.GetCardLevel1(unitId);
            foreach (var cardId in level1CardIds)
            {
                AddReturnItem(ItemType.CardPiece, cardId, 0);
            }
        }

        protected void AddUserProfileByUnit(uint unitId)
        {
            var userProfileId = Data.GetProfileIconIdByUnitUnlock(unitId);
            AddReturnItem(ItemType.UserProfile, userProfileId, 0);
        }

        protected void AddEmoticonByUnit(uint unitId)
        {
            var emoticonId = Data.GetUnlockEmoticonIdByUnitId(unitId);
            AddReturnItem(ItemType.Emoticon, emoticonId, 0);
        }

        protected async Task PieceRandomExtract(DataItem dataItem, ItemType extractItemType, ItemType resultItemType, List<ProtoItemResult> alreadyAcheiveItems)
        {
            var extractor = ItemExtractorFactory.CreateItemExtractor(_userId, extractItemType, _db);
            await extractor.Extract(dataItem, alreadyAcheiveItems, 0);
            var resultItems = extractor.GetResultItems();
            foreach (var resultItem in resultItems)
            {
                AddReturnItem(resultItem.ItemType, resultItem.ClassId, resultItem.Count);
            }
        }

        protected async Task AddUserProductByUnlockCard(uint cardId)
        {
            var card = Data.GetCard(cardId);
            if (card.RewardId == 0)
                return;

            var reward = Data.GetReward(card.RewardId);
            foreach (var itemId in reward.ItemIdList)
            {
                var items = Data.GetItem(itemId);
                foreach (var item in items)
                {
                    if (item.ItemType != ItemType.UserProduct)
                        continue;

                    UserProduct userProduct = new UserProduct(_userId, item.ClassId, item.MinNumber, _db);
                    await userProduct.Get("AddUserProductByUnlockCard");
                    AddReturnItem(ItemType.UserProduct, item.ClassId, item.MinNumber);
                }
            }
        }
    }
}

