using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using Common.Entities.ItemExtractor;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Box
{
    public class Box : IReward
    {
        private uint _userId;
        private uint _classId;
        private DBContext _userDb;
        private List<List<DataItem>> _listItems;
        private uint _itemValue;

        public Box(DBContext userDb, uint userId, uint classId, List<List<DataItem>> listItems, uint itemValue)
        {
            _userId = userId;
            _classId = classId;
            _userDb = userDb;
            _listItems = listItems;
            _itemValue = itemValue;
        }

        public async Task<List<ProtoItemResult>> GetReward()
        {
            List<ProtoItemResult> itemResults = new List<ProtoItemResult>();
            try
            {
                foreach (var dataItmes in _listItems)
                {
                    var gettedItemResults = await GetItemResult(dataItmes, itemResults, _itemValue);
                    itemResults.AddRange(gettedItemResults);
                }
            }
            catch(Exception e)
            {
                AkaLogger.Log.Debug.Exception("GetReward:userid-" + _userId.ToString() + "RewardId-" + _classId.ToString() , e);
                throw new Exception(e.Message + "RewardId : " + _classId.ToString());
            }
            
            return itemResults;
        }

        private async Task<List<ProtoItemResult>> GetItemResult(List<DataItem> items, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var selectedDataItem = items[AkaRandom.Random.ChooseIndexRandomlyInSumOfProbability(items)];

            if (ItemExtractorFactory.IsPieceType(selectedDataItem.ItemType))
            {
                var extractor = ItemExtractorFactory.CreateItemExtractor(_userId, selectedDataItem.ItemType, _userDb);
                await extractor.Extract(selectedDataItem, alreadyAcheiveItems, itemValue);

                return extractor.GetResultItems();
            }
            else if (ItemExtractorFactory.IsCountlessType(selectedDataItem.ItemType))
            {
                List<ProtoItemResult> retItems = new List<ProtoItemResult>();

                var item = ItemFactory.CreateCountless(selectedDataItem.ItemType, _userId, selectedDataItem.ClassId, _userDb);

                if (item.IsValidData() && false == await item.IsHave())
                {
                    retItems.Add(new ProtoItemResult
                    {
                        ItemType = selectedDataItem.ItemType,
                        ClassId = selectedDataItem.ClassId
                    });
                }
                
                return retItems;
            }
            else if (ItemExtractorFactory.IsSelectableType(selectedDataItem.ItemType))
            {
                var extractor = ItemExtractorFactory.CreateSelectableExtractor(_userId, selectedDataItem.ItemType, _userDb);
                await extractor.Extract(items, alreadyAcheiveItems, itemValue);

                return extractor.GetResultItems();
            }
            else if (ItemExtractorFactory.IsUnlockContentsType(selectedDataItem.ItemType))
            {
                List<ProtoItemResult> retItems = new List<ProtoItemResult>();

                var contentType = selectedDataItem.ClassId;
                var contentId = AkaRandom.Random.Next(selectedDataItem.MinNumber, selectedDataItem.MaxNumber + 1);
                var item = ItemFactory.CreateUnlockContents(selectedDataItem.ItemType, _userId, contentType, _userDb);

                if (item.IsValidData() && false == await item.IsHave())
                {
                    retItems.Add(new ProtoItemResult
                    {
                        ItemType = selectedDataItem.ItemType,
                        ClassId = selectedDataItem.ClassId,
                        Count = contentId
                    });
                }

                return retItems;
            }
            else
            {
                List<ProtoItemResult> retItems = new List<ProtoItemResult>();
                retItems.Add(new ProtoItemResult
                {
                    ItemType = selectedDataItem.ItemType,
                    ClassId = selectedDataItem.ClassId,
                    Count = AkaRandom.Random.Next(selectedDataItem.MinNumber, selectedDataItem.MaxNumber + 1)
                });
                return retItems;
            }
        }
        
        public async Task SetReward(List<ProtoItemResult> itemResults, string logCategory)
        {
            foreach (ProtoItemResult itemResult in itemResults)
            {
                var item = ItemFactory.CreateItem(itemResult.ItemType, _userId, itemResult.ClassId, _userDb, itemResult.Count);
                await item.Get(logCategory);
            }
        }
    }
}
 