
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AkaUtility;
using AkaEnum.Battle;
using AkaSerializer;
using System;
using System.Text;
using AkaData;
using System.Data.Common;
using Common.Entities.Reward;
using Common.Pass;

namespace Common.Quest
{
    public class QuestIO
    {
        private DBContext _db;
        private uint _userId;
        private uint _currentSeason;

        public QuestIO(uint userId, DBContext db)
        {
            _db = db;
            _userId = userId;
        }

        public QuestIO(uint userId, uint currentSeason, DBContext db = null) : this(userId, db)
        {
            _currentSeason = currentSeason;
        }

        public QuestIO(uint userId) : this(userId, null)
        {
        }

        private async Task<List<(uint questGroupId, uint receivedOrder, uint dynamicQuestGroupId)>> SelectRewardList(Dictionary<uint, uint> questGroupIdWithItemValueList)
        {
            var rewardTargetIds = new List<(uint questId, uint receivedOrder, uint dynamicQuestGroupId)>();
            if (false == questGroupIdWithItemValueList.Any())
                return rewardTargetIds;


            var seasonPassQuests = questGroupIdWithItemValueList.Select(questGroupWithItem => Data.GetQuest(questGroupWithItem.Key)?.FirstOrDefault())
                                                                .Where(quest => (quest?.QuestType ?? QuestType.None) == QuestType.SeasonPass)
                                                                .Select(quest => quest.QuestGroupId);
            int enableReceiveOrder = int.MaxValue;
            if (true == seasonPassQuests.Any())
            {
                var seasonPassManager = new SeasonPassManager(_userId, 0, _db);
                var enablePassList = await seasonPassManager.GetEnablePassList();

                var enableSeasonQuests = enablePassList.Select(seasonPassId => Data.GetSeasonPass(seasonPassId).QuestGroupId);
                if (false == seasonPassQuests.All(enableSeasonQuests.Contains))
                {
                    return rewardTargetIds;
                }

                //seasonPassManager.IsPurchasedCurrentSeasonPass()
                if (false == enablePassList.Any(seasonPassId => Data.GetSeasonPass(seasonPassId).SeasonPassType == SeasonPassType.SeasonPassPremium))
                {
                    var currentSeasonInfo = await Entities.Season.ServerSeason.GetCurrentSeasonPassInfo();
                    enableReceiveOrder = currentSeasonInfo.currentSeasonInterval;
                }
            }

            var query = new StringBuilder();
            var findIds = string.Join(",", questGroupIdWithItemValueList.Keys);

            query.Append("SELECT id, receivedOrder, dynamicQuestId FROM quests WHERE userId = ")
                 .Append(_userId)
                 .Append(" AND receivedOrder < completedOrder AND id IN (")
                 .Append(findIds)
                 .Append(");");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                rewardTargetIds.AddRange(cursor.Cast<System.Data.IDataRecord>()
                                               .Select(record =>
                                               {
                                                   return (questGroupId: (uint)record["id"], receivedOrder: 1 + (uint)record["receivedOrder"], dynamicQuestGroupId: (uint)record["dynamicQuestId"]);
                                               })
                                               .Where(rewardTarget => rewardTarget.receivedOrder <= enableReceiveOrder)
                                               );
            }
            return rewardTargetIds;
        }

        public async Task<(RewardResultType result, List<ProtoItemResult> rewardItems)> GetRewards(Dictionary<uint, uint> questGroupIdWithItemValueList)
        {
            var itemResult = new List<ProtoItemResult>();
            var rewardTargetList = await SelectRewardList(questGroupIdWithItemValueList);

            var query = new StringBuilder();
            foreach (var questStatus in rewardTargetList)
            {
                var targetQuestGroupId = questStatus.questGroupId;
                var itemValue = questGroupIdWithItemValueList[targetQuestGroupId];
                if (questStatus.dynamicQuestGroupId != 0)
                {
                    targetQuestGroupId = questStatus.dynamicQuestGroupId;
                }

                var rewardTargetQuest = Data.GetQuest(targetQuestGroupId)?.FirstOrDefault(quest => quest.Order == questStatus.receivedOrder);
                if (rewardTargetQuest != null)
                {
                    var rewards = await Reward.GetRewards(_db, _userId, rewardTargetQuest.RewardId, "Quest", itemValue);
                    itemResult.AddRange(rewards);
                }

                if (questStatus.dynamicQuestGroupId == 0)
                    query.Append("UPDATE quests SET receivedOrder = receivedOrder + 1 WHERE userId = ").Append(_userId)
                         .Append(" AND id = ").Append(questStatus.questGroupId)
                         .Append(";");
                else
                    query.Append("UPDATE quests SET receivedOrder = 0, performCount =0, completedOrder =0, dynamicQuestId =0 WHERE userId = ").Append(_userId)
                         .Append(" AND id = ").Append(questStatus.questGroupId)
                         .Append(";");
                await _db.ExecuteNonQueryAsync(query.ToString());

            }

            var result = itemResult.Any() ? RewardResultType.Success : RewardResultType.EmptyReward;
            return (result: result, rewardItems: itemResult);
        }

        public List<ProtoQuestInfo> Select(DbDataReader cursor)
        {
            var result = new List<ProtoQuestInfo>();
            var seasonQuestGroupIdList = new SeasonPassManager(_userId, _currentSeason, _db).GetActivePassList()
                                                                                            .Select(seasonPass => seasonPass.QuestGroupId) ?? Enumerable.Empty<uint>();

            var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var utcNow = DateTime.UtcNow.AddHours(-refreshBaseHour);
            var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);

            while (cursor.Read())
            {
                var questInfo = new ProtoQuestInfo
                {
                    QuestGroupId = (uint)cursor["id"],
                    PerformCount = (int)cursor["performCount"],
                    ReceivedOrder = (uint)cursor["receivedOrder"],
                    CompleteOrder = (uint)cursor["completedOrder"],
                    DynamicQuestGroupId = (uint)cursor["dynamicQuestId"],
                    ActiveTime = new DateTime(((DateTime)cursor["activeTime"]).Ticks, DateTimeKind.Utc)
                };

                var quest = Data.GetQuest(questInfo.DynamicQuestGroupId == 0 ? questInfo.QuestGroupId : questInfo.DynamicQuestGroupId)?.LastOrDefaultEx();

                if (quest != null)
                {
                    if (quest.QuestType.In(QuestType.SeasonPass) && false == quest.QuestGroupId.In(seasonQuestGroupIdList))
                    {
                        continue;
                    }

                    if (true == IsDailyRangeQuest(quest.QuestProcessType) && questInfo.ActiveTime < initDateTime)
                    {
                        questInfo.ActiveTime = DateTime.UtcNow;
                        questInfo.PerformCount = 0;
                        questInfo.ReceivedOrder = 0;
                        questInfo.CompleteOrder = 0;
                    }

                    if (quest.QuestKeeping || quest.Order > questInfo.ReceivedOrder)
                        result.Add(questInfo);
                }
            }

            return result;
        }

        public async Task<List<ProtoQuestInfo>> GetQuestWithType(QuestType questType)
        {
            var questTypeCondition = " AND questType = " + ((int)questType).ToString();
            return await Select(questTypeCondition);
        }


        private bool IsDailyRangeQuest(QuestProcessType questProcessType)
        {
            return questProcessType.In(QuestProcessType.DailyKnightLeagueVictory);
        }

        private async Task<DbDataReader> SelectEndQuestList(IEnumerable<uint> endSeasonPassQuestGroupIdList)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT id, performCount, receivedOrder, completedOrder, dynamicQuestId, activeTime FROM quests WHERE userId = ")
                 .Append(_userId.ToString())
                 .Append(" AND id IN (")
                 .Append(string.Join(",", endSeasonPassQuestGroupIdList))
                 .Append(");");

            var cursor = await _db.ExecuteReaderAsync(query.ToString());
            return cursor;
        }

        public async Task SendToMailAllCompletedRewards(IEnumerable<DataSeasonPass> endSeasonPassQuestGroupIdList)
        {
            var rewardList = new List<uint>();
            using (var cursor = await SelectEndQuestList(endSeasonPassQuestGroupIdList.Select(seasonPass => seasonPass.QuestGroupId)))
            {
                var forSeasonQuestList = Select(cursor);

                foreach (var questInfo in forSeasonQuestList)
                {
                    var dataQuest = Data.GetQuest(questInfo.QuestGroupId);
                    rewardList.AddRange(dataQuest.Where(data => data.Order > questInfo.ReceivedOrder && data.Order <= questInfo.CompleteOrder)
                                                  .Select(data => data.MailRewardId));
                }

                foreach (var mailRewardId in rewardList)
                {
                    await Reward.GetRewards(_db, _userId, mailRewardId, "SeasonPassMail");
                }
            }
        }

        public async Task UpdateInitQuests(IEnumerable<uint> initTargetQuestGroupIdList)
        {
            var query = new StringBuilder();

            query.Append("UPDATE quests SET performCount = 0, receivedOrder = 0, completedOrder = 0 WHERE userId = ")
                    .Append(_userId)
                    .Append(" AND id IN (")
                    .Append(string.Join(",", initTargetQuestGroupIdList))
                    .Append(");");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<List<ProtoQuestInfo>> Select()
        {
            return await Select("");
        }


        private async Task<List<ProtoQuestInfo>> Select(string extraCondition)
        {
            var query = new StringBuilder();

            query.Append("SELECT id, performCount, receivedOrder, completedOrder, dynamicQuestId, activeTime FROM quests WHERE userId = ")
                      .Append(_userId).Append(extraCondition).Append(";");
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return Select(cursor);
            }
        }

        public async Task<IEnumerable<ProtoQuestInfo>> UpdateQuests(bool publish, IEnumerable<ProtoQuestInfo> questList)
        {
            var questTaskList = await Task.WhenAll(questList.Select(async questInfo => await UpdatePerformCount(questInfo)));
            var questInfoList = questTaskList.Where(questInfo => questInfo != null).ToList();

            if (true == publish && questInfoList.Any())
            {
                var protoUpdateQuest = new CommonProtocol.PubSub.ProtoWeb2OneUpdateQuest()
                {
                    MessageType = MessageType.PubSubUpdateQuest,
                    UserId = _userId,
                    UpdatedQuestList = questInfoList
                };

                Network.PubSubConnector.Instance.Send(MessageType.PubSubUpdateQuest, AkaSerializer<CommonProtocol.PubSub.ProtoWeb2OneUpdateQuest>.Serialize(protoUpdateQuest));
            }

            return questInfoList;
        }

        public void UpdateQuest(Dictionary<QuestProcessType, int> addUpdateValues, uint classId, List<ProtoActionStatus> actionStatus)
        {
            Task.Factory.StartNew(async () =>
            {
                using (var db = new DBContext(_userId))
                {
                    _db = db;
                    if (actionStatus == null)
                        actionStatus = new List<ProtoActionStatus>();
                    try
                    {
                        await UpdateQuestAsync(addUpdateValues, classId, actionStatus);
                    }
                    catch (Exception ex)
                    {
                        var errMessage = $"UpdateQuest<{_userId}:{string.Join(",", addUpdateValues.Values)}> {ex.ToString()}";
                        AkaLogger.Log.Debug.Exception(errMessage, ex);
                        AkaLogger.Logger.Instance().Error(errMessage);
                    }
                }
            });
        }

        private void AddDynamicQuestProcessType(Dictionary<QuestProcessType, int> addUpdateValue)
        {
            addUpdateValue.Add(QuestProcessType.DynamicQuest, 0);
        }

        private void AddActionStatusQuestProcessType(Dictionary<QuestProcessType, int> addUpdateValue, List<ProtoActionStatus> actionStatus)
        {
            var actionQuestTypes = actionStatus.Select(action => (QuestProcessType)((int)QuestProcessType.BattleActionStatus + (int)action.ActionStatusType));
            foreach (var questType in actionQuestTypes)
            {
                if (false == addUpdateValue.ContainsKey(questType))
                    addUpdateValue.Add(questType, 0);
            }
        }


        public async Task UpdateQuestAsync(Dictionary<QuestProcessType, int> addUpdateValues, uint classId, List<ProtoActionStatus> actionStatus)
        {
            AddDynamicQuestProcessType(addUpdateValues);
            AddActionStatusQuestProcessType(addUpdateValues, actionStatus);

            var targetQuestGroupIdList = addUpdateValues.Keys.SelectMany(processType => Data.GetQuestWithProcessType(processType) ?? new List<uint>());
            var targetQuestGroupIdValues = string.Join(",", targetQuestGroupIdList);

            var query = new StringBuilder();
            query.Append("SELECT id, performCount, receivedOrder, completedOrder, dynamicQuestId, activeTime FROM quests WHERE userId = ")
                 .Append(_userId)
                 .Append(" AND id in (")
                 .Append(targetQuestGroupIdValues)
                 .Append(");");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                var targetQuestInfo = Select(cursor);
                var updateTargetQuestInfo = targetQuestInfo.Where(questInfo =>
                {
                    var questGroups = Data.GetQuest(questInfo.DynamicQuestGroupId == 0 ? questInfo.QuestGroupId : questInfo.DynamicQuestGroupId);
                    var firstQuestGroup = questGroups.First();
                    var processType = firstQuestGroup.QuestProcessType;

                    if (false == addUpdateValues.ContainsKey(processType))
                        return false;

                    if (processType > QuestProcessType.BattleActionStatus && processType < QuestProcessType.BattleActionStatusEnd)
                    {
                        var actionStatusType = (ActionStatusType)((int)processType - (int)QuestProcessType.BattleActionStatus);
                        var performClassId = firstQuestGroup.ClassId;

                        var updateAction = actionStatus.FirstOrDefault(action => action.ClassId == performClassId && action.ActionStatusType == actionStatusType);

                        if (updateAction != null)
                        {
                            questInfo.PerformCount += updateAction.Value;
                            return true;
                        }
                        return false;
                    }


                    if (false == IsValidQuestGroup(firstQuestGroup, classId))
                    {
                        return false;
                    }

                    if (IsAccumulatedCountProcessType(processType))
                        questInfo.PerformCount += addUpdateValues[processType];
                    else if (questInfo.PerformCount < addUpdateValues[processType])
                        questInfo.PerformCount = addUpdateValues[processType];
                    else
                        return false;
                    return true;
                });

                await UpdateQuests(true, updateTargetQuestInfo);
            }
        }

        private bool IsValidQuestGroup(DataQuest questGroup, uint classId)
        {
            if (questGroup.QuestProcessType == QuestProcessType.DynamicQuest)
                return false;

            //비교대상 아님
            if (questGroup.ClassId == 0)
                return true;

            return questGroup.ClassId == classId;
        }

        private bool IsAccumulatedCountProcessType(QuestProcessType processType)
        {
            return false == processType.In(QuestProcessType.FinalRankPoint, QuestProcessType.FinalVirtualRankPoint);
        }

        private async Task<ProtoQuestInfo> UpdatePerformCount(ProtoQuestInfo questInfo)
        {
            var questGroupList = Data.GetQuest(questInfo.DynamicQuestGroupId == 0 ? questInfo.QuestGroupId : questInfo.DynamicQuestGroupId);

            if (questGroupList == null || questInfo.CompleteOrder == questGroupList.LastOrDefaultEx().Order)
                return null;

            var nextQuest = questGroupList?.FirstOrDefault(data => data.QuestConditionValue >= questInfo.PerformCount);
            uint completeOrder = 0;
            uint questType = (uint)questGroupList.First().QuestType;

            if (nextQuest == null)
                completeOrder = questGroupList.LastEx().Order;
            else if (nextQuest.QuestConditionValue == questInfo.PerformCount)
                completeOrder = nextQuest.Order;
            else
                completeOrder = nextQuest.Order - 1;

            string receivedOrderUpdate = "";
            if (nextQuest != null && nextQuest.RewardId == 0)
                receivedOrderUpdate = ", receivedOrder = receivedOrder +1";

            var activeTime = questInfo.ActiveTime.ToTimeString();
            var query = new StringBuilder();
            query.Append("INSERT INTO quests (userId, id, performCount, completedOrder, questType, activeTime ) VALUES (")
                        .Append(_userId).Append(",").Append(questInfo.QuestGroupId).Append(",")
                        .Append(questInfo.PerformCount).Append(",").Append(completeOrder).Append(",")
                        .Append(questType).Append(", '")
                        .Append(activeTime).Append("'")
                        .Append(") ON DUPLICATE KEY UPDATE performCount = ").Append(questInfo.PerformCount)
                        .Append(", completedOrder = ").Append(completeOrder)
                        .Append(", activeTime ='").Append(activeTime).Append("'")
                        .Append(receivedOrderUpdate)
                        .Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());

            questInfo = new ProtoQuestInfo
            {
                QuestGroupId = questInfo.QuestGroupId,
                CompleteOrder = completeOrder,
                PerformCount = questInfo.PerformCount,
                ReceivedOrder = questInfo.ReceivedOrder,
                DynamicQuestGroupId = questInfo.DynamicQuestGroupId,
                ActiveTime = questInfo.ActiveTime
            };


            return questInfo; ;
        }


    }
}
