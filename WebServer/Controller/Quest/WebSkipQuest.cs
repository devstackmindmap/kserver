using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Item;
using Common.Pass;
using Common.Quest;
using CommonProtocol;

namespace WebServer.Controller.Quest
{

    public class WebSkipQuest : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            var res = new ProtoOnSkipCurrentQuest
            {
                QuestInfoList = new List<ProtoQuestInfo>(),
                ResultType = ResultType.InvalidSeason,
                RemainedMaterial = new Dictionary<MaterialType, int>()
            };

            var currentSeasonInfo = await Common.Entities.Season.ServerSeason.GetCurrentSeasonPassInfo();
            var currentSeason = currentSeasonInfo.currentSeason;
            var currentSeasonInterval = currentSeasonInfo.currentSeasonInterval;

            var seasonpassList = Data.GetSeasonPassListForSeason(currentSeason);
            var questList = seasonpassList?.Select(seasonPass => Data.GetQuest(seasonPass.QuestGroupId)) ?? Enumerable.Empty<List<DataQuest>>();
            if (questList.Any() != true)
            {
                return res;
            }

            using (var db = new DBContext(req.UserId))
            {
                var seasonPassManager = new SeasonPassManager(req.UserId, 0, db);
                var enableSeasonPassList = await seasonPassManager.GetEnablePassList();

                var isValidSeasonPassList = seasonpassList.Any(seasonPass => enableSeasonPassList.Contains(seasonPass.SeasonPassId));
                if (false == isValidSeasonPassList)
                {
                    return res;
                }

                var dbQuestList = await GetLastCompleteOrderForQuest(req.UserId, db, seasonpassList);
                var lastCompleteOrder = dbQuestList.Any() ? dbQuestList.Max(questInfo => questInfo.CompleteOrder) : 0;
                if (lastCompleteOrder >= questList.First().Max(quest => quest.Order))
                {
                    res.ResultType = ResultType.AllCompletedQuest;
                    return res;
                }

                lastCompleteOrder++;
                if (currentSeasonInterval < lastCompleteOrder && false == enableSeasonPassList.Any(seasonPassId => Data.GetSeasonPass(seasonPassId).SeasonPassType == SeasonPassType.SeasonPassPremium))
                {
                    res.ResultType = ResultType.NeedPremiumPass;
                    return res;
                }
                await DoSkipComplete(req.UserId, db, lastCompleteOrder, seasonpassList, questList, dbQuestList, res);
            }
            return res;
        }


        private List<Material> GetNeedMaterial(uint userId, DBContext db, List<DataSeasonPass> seasonpassList)
        {
            return seasonpassList.GroupBy(seasonPass => seasonPass.MaterialType)
                          .Select(seasonPassGroup =>
                          {
                              switch (seasonPassGroup.Key)
                              {
                                  case MaterialType.Gold:
                                      return (Material)new Gold(userId, db, seasonPassGroup.Select(seasonPass => seasonPass.Value).Sum());
                                  case MaterialType.Gem:
                                      return (Material)new Gem(userId, db, seasonPassGroup.Select(seasonPass => seasonPass.Value).Sum());
                                  default:
                                      return (Material)new Gold(userId, db, int.MaxValue - 1);
                              }
                          })
                          .ToList();
        }

        private ResultType GetNotEnoughtMaterialType(Material material)
        {
            switch (material.GetMaterialType())
            {
                case MaterialType.Gold:
                    return ResultType.NeedGold;
                case MaterialType.Gem:
                    return ResultType.NeedGem;
                default:
                    return ResultType.Fail;
            }
        }

        private async Task DoSkipComplete(uint userId, DBContext db, uint lastCompleteOrder,
                                                      List<DataSeasonPass> seasonpassList, IEnumerable<List<DataQuest>> questList, List<ProtoQuestInfo> dbQuestList, ProtoOnSkipCurrentQuest refResult)
        {
            //check material
            var needMaterials = GetNeedMaterial(userId, db, seasonpassList);
            foreach (var needMaterial in needMaterials)
            {
                if (false == await needMaterial.IsEnoughCount())
                {
                    refResult.ResultType = GetNotEnoughtMaterialType(needMaterial);
                    return;
                }
            }

            await db.BeginTransactionCallback(async () =>
            {

                var completeCount = questList.First().First(questData => questData.Order == lastCompleteOrder).QuestConditionValue;

                var questIO = new QuestIO(userId, db);
                var updateQuestInfoList = questList.Select(dataQuestList =>
                {
                    var questGroupId = dataQuestList.First().QuestGroupId;
                    return new ProtoQuestInfo
                    {
                        PerformCount = completeCount,
                        QuestGroupId = questGroupId,
                        ReceivedOrder = dbQuestList.FirstOrDefault(quest => quest.QuestGroupId == questGroupId).ReceivedOrder
                    };
                });


                var updatedQuestInfoList = await questIO.UpdateQuests(true, updateQuestInfoList);

                if (updatedQuestInfoList.Any())
                {
                    refResult.QuestInfoList.AddRange(updatedQuestInfoList);
                    //spend material
                    foreach (var needMaterial in needMaterials)
                    {
                        await needMaterial.Use("SkipQuest");
                        var remainValue = await needMaterial.GetRemainCount();
                        refResult.RemainedMaterial.Add(needMaterial.GetMaterialType(), remainValue);
                    }
                    refResult.ResultType = ResultType.Success;
                }

                return true;
            });
        }

        private async Task<List<ProtoQuestInfo>> GetLastCompleteOrderForQuest(uint userId, DBContext db, List<DataSeasonPass> seasonPassList)
        {
            var targetQuestGroupIdValues = string.Join(",", seasonPassList.Select(seasonPass => seasonPass.QuestGroupId));

            var query = new StringBuilder();
            query.Append("SELECT id, performCount, receivedOrder, completedOrder, dynamicQuestId, activeTime FROM quests WHERE userId = ")
                 .Append(userId)
                 .Append(" AND id in (")
                 .Append(targetQuestGroupIdValues)
                 .Append(");");

            using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
            {
                var questIO = new QuestIO(userId, seasonPassList.First().Season, db);
                var dbQuestList = questIO.Select(cursor);
                return dbQuestList;
                //    return dbQuestList.Any() ? (dbQuestList.Max(questInfo => questInfo.CompleteOrder),  : 0;
            }
        }
    }


}
