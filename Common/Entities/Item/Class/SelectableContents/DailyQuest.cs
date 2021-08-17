using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using AkaUtility;
using Common.UserInfo;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Item
{
    public class DailyQuest
    {
        private DataQuest _targetQuest;
        private uint _userId;
        private DBContext _db;

        public DailyQuest(uint userId, uint classId, DBContext db)
        {
            _targetQuest = Data.GetQuest(classId)?.FirstOrDefault();
            _userId = userId;
            _db = db;
        }

        public async Task<ProtoOnNewQuest> CreateNew()
        {
            ResultType result = ResultType.InvalidClassId;
            if (_targetQuest != null && _targetQuest.QuestType != QuestType.Daily)
                return new ProtoOnNewQuest { ResultType = result };

            return await CreateNewWithUseMaterials();
        }

        private async Task<List<(uint questGroupId, uint dynamicQuestId)>> GetDailyQuest()
        {
            var query = new StringBuilder();
            query.Append("SELECT id, dynamicQuestId, receivedOrder, completedOrder FROM quests WHERE userId = ").Append(_userId)
                 .Append(" AND questType = ").Append((int)QuestType.Daily);

            List<(uint questGroupId, uint dynamicQuestId)> dailyQuestList = new List<(uint questGroupId, uint dynamicQuestId)>();
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (true == cursor.Read())
                {
                    var receivedOrder = (uint)cursor["receivedOrder"];
                    var completedOrder = (uint)cursor["completedOrder"];
                    if (receivedOrder == completedOrder)
                    {
                        dailyQuestList.Add((questGroupId: (uint)cursor["id"], dynamicQuestId: (uint)cursor["dynamicQuestId"]));
                    }
                }
            }
            return dailyQuestList;
        }

        private async Task<ProtoOnNewQuest> CreateNewWithUseMaterials()
        {
            var result = new ProtoOnNewQuest() { QuestGroupAndDynamicList = new Dictionary<uint, uint>() , ResultType = ResultType.Success };
            var isNew = _targetQuest == null;
            var dailyQuests = await GetDailyQuest();
            var emptyQuests = dailyQuests.Where(quest => quest.dynamicQuestId == 0);        //할당되지않은 퀘스트
            var targetQuests = dailyQuests.Where(quest => quest.dynamicQuestId != 0 && quest.questGroupId == _targetQuest.QuestGroupId);    //할당된 퀘스트

            var dayIo = new DayIO(_userId, _db);
            await dayIo.Select();

            var emptyQuestCount = emptyQuests.Count();

            if (dayIo.DailyQuestAddcount <= -emptyQuestCount)
                dayIo.DailyQuestAddcount = 1 - emptyQuestCount;

            if (isNew && emptyQuestCount == 0)
                result.ResultType = ResultType.AlreadyFullQuest;
            else if (false == isNew && false == targetQuests.Any())
                result.ResultType = ResultType.InvalidClassId;

            if (result.ResultType != ResultType.Success)
            {
                await dayIo.Update();
                result.RefreshCount = dayIo.DailyQuestRefreshCount;
                result.AddCount = dayIo.DailyQuestAddcount;

                return result;
            }
                        
            var materialType = GetMaterialValue(dayIo, isNew, out var needValues);
            var material = MaterialFactory.CreateMaterial(materialType, _userId, needValues, _db);
            if (material != null && needValues != 0 && false == await material.IsEnoughCount())
                return new ProtoOnNewQuest { ResultType = NotEnoughtMaterial(materialType) };

            var targetDailyQuestList = new List<DataQuest>();
            if (isNew)
            {
                foreach (var quest in emptyQuests)
                {
                    var targetDailyQuest = Data.GetQuest(quest.questGroupId)?.FirstOrDefault();
                    if (targetDailyQuest != null)
                    {
                        targetDailyQuestList.Add(targetDailyQuest);
                        dayIo.DailyQuestAddcount++;
                        if (dayIo.DailyQuestAddcount > 0)
                            break;
                    }
                }
            }
            else
                targetDailyQuestList.AddRange(targetQuests.Select(quest => Data.GetQuest(quest.questGroupId)?.FirstOrDefault()).Where(quest => quest != null));

            foreach (var targetDailyquest in targetDailyQuestList)
            {
                var rewards = await Reward.Reward.GetRewards(_db, _userId, targetDailyquest.RewardId, "DailyQuest", targetDailyquest.QuestGroupId);
                if (rewards.Any())
                {
                    result.QuestGroupAndDynamicList.Add((uint)rewards.First().Count, rewards.First().ClassId);
                }
                else
                {
                    result.ResultType = ResultType.EmptyItem;
                }
            }
            await dayIo.Update();

            if (material != null && needValues != 0)
                await material.Use("DailyQuest");
            result.RemainedMaterials = await material.GetRemainCount();
            result.MaterialType = materialType;
            result.ResultType = ResultType.Success;
            result.RefreshCount = dayIo.DailyQuestRefreshCount;
            result.AddCount = dayIo.DailyQuestAddcount;
            return result;
        }


        private MaterialType GetMaterialValue(DayIO dayIo, bool isNew , out int needValues)
        {
            BehaviorType behaviorType = BehaviorType.DailyQuestRefresh;
            int tryCount = 1;
            if (isNew)
            {
                behaviorType = BehaviorType.DailyQuestForcedAdd;
                tryCount += dayIo.DailyQuestAddcount;
            }
            else
            {
                tryCount += dayIo.DailyQuestRefreshCount++;
            }

            var needMaterials = Data.GetSpendMaterials(behaviorType);
            var needMaterial = needMaterials.FirstOrDefault(materialInfo => materialInfo.Order >= tryCount);
            if (needMaterial == null && needMaterials.Count > 0)
                needMaterial = needMaterials.LastEx();
            if (needMaterial == null)
            {
                needValues = 0;
                return MaterialType.None;
            }

            needValues = needMaterial.Value;
            return needMaterial.MaterialType;
        }

        private ResultType NotEnoughtMaterial(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Gold:
                    return ResultType.NeedGold;
                case MaterialType.Gem:
                    return ResultType.NeedGem;
                case MaterialType.StarCoin:
                    return ResultType.NeedStarCoin;
                default:
                    return ResultType.NeedMaterial;
            }
        }

    }
}
