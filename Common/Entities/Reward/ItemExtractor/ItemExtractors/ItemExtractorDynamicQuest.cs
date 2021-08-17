using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;

namespace Common.Entities.ItemExtractor
{
    public class ItemExtractorDynamicQuest : ItemExtractorPieceUnlockRandom
    {
        public ItemExtractorDynamicQuest(uint userId, DBContext db) : base(userId, null, db)
        {
        }

        public override async Task Extract(DataItem dataItem, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
        }

        public override async Task Extract(List<DataItem> dataItems, List<ProtoItemResult> alreadyAcheiveItems, uint itemValue)
        {
            var targetQuestGroupId = itemValue;
            var targetQuest = Data.GetQuest(targetQuestGroupId)?.FirstOrDefault();
            if (false == targetQuest?.QuestType.In(QuestType.Daily, QuestType.Weekly, QuestType.Monthly))
                return;

            var haveDynamicQuests = await GetItems();

            var currentContents = haveDynamicQuests.Where(quest => quest.questType == (int)targetQuest.QuestType && quest.dynamicQuestId != 0)
                                                   .Select(quest => quest.dynamicQuestId);
            var nextQuests = dataItems.Where(item => item.ItemType == ItemType.DynamicQuest 
                                                  && false == item.ClassId.In(currentContents) 
                                                  && Data.GetQuest(item.ClassId) != null);

            //check unit, card behavior quest
            nextQuests = await GetFilteredNextQuests(nextQuests);

            var nextQuestList = nextQuests.ToList();
            if (nextQuestList.Any())
            {
                var selectedDataItem = nextQuestList[AkaRandom.Random.ChooseIndexRandomlyInSumOfProbability(nextQuestList)];
                AddReturnItem(ItemType.DynamicQuest, selectedDataItem.ClassId, (int)targetQuest.QuestGroupId);
            }
        }

        private async Task<IEnumerable<DataItem>> GetFilteredNextQuests(IEnumerable<DataItem> nextQuests)
        {
            var haveUnitQuest = nextQuests.Any(questItem => Data.GetQuest(questItem.ClassId).First().QuestProcessType.In(QuestProcessType.UnitActionDealing, QuestProcessType.UnitActionUseCard, QuestProcessType.UnitActionVictory));
            var haveCardQuest = nextQuests.Any(questItem => Data.GetQuest(questItem.ClassId).First().QuestProcessType.In(QuestProcessType.CardActionUse));

            var units = haveUnitQuest ? await GetUnits() : Enumerable.Empty<uint>();
            var cards = haveUnitQuest ? await GetCards() : Enumerable.Empty<uint>();

            nextQuests = nextQuests.Where(questItem =>
            {
                var quest = Data.GetQuest(questItem.ClassId).First();
                if (true == quest.QuestProcessType.In(QuestProcessType.UnitActionDealing, QuestProcessType.UnitActionUseCard, QuestProcessType.UnitActionVictory))
                {
                    return quest.ClassId.In(units);
                }
                else if (true == quest.QuestProcessType.In(QuestProcessType.CardActionUse))
                {
                    return quest.ClassId.In(cards);
                }
                return true;
            });
            return nextQuests;
        }

        private async Task<List<uint>> GetUnits()
        {
            var units = new List<uint>();
            var query = "SELECT id FROM units WHERE userId = " + _userId.ToString() + ";";
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    units.Add((uint)cursor.GetInt32(0));
                }
            }
            return units;
        }

        private async Task<List<uint>> GetCards()
        {
            var units = new List<uint>();
            var query = "SELECT id FROM cards WHERE userId = " + _userId.ToString() + ";";
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    units.Add((uint)cursor.GetInt32(0));
                }
            }
            return units;
        }

        protected new async Task<List<(uint questType, uint dynamicQuestId, uint questGroupId )>> GetItems()
        {
            var haveItems = new List<(uint questType, uint dynamicQuestId, uint questGroupId)>();
            var query = "SELECT questType, dynamicQuestId, id FROM quests WHERE userId =" + _userId.ToString() + ";";
            
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    haveItems.Add( ((uint)cursor.GetInt32(0), (uint)cursor.GetInt32(1), (uint)cursor.GetInt32(2)));
                }
            }
            return haveItems;
        }
    }
}
