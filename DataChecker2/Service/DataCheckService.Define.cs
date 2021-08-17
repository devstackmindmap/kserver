using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataChecker2.Dao.Datas;
using DataChecker2.Dao.Enums;
using DataChecker2.Dao.ViewModel;
using LINQtoCSV;
using DataRow = DataChecker2.Dao.Datas.DataRow;

namespace DataChecker2.Service
{
    public class DataCheckMeta
    {
        public List<string> Keys = new List<string>();  //고유키들
        public List<string> Nullable = new List<string>(); //null 허용컬럼
        public List<string> Zeroable = new List<string>(); //0 허용컬럼
        public Dictionary<string, Dictionary<string, string>> Aggregation = new Dictionary<string, Dictionary<string, string>>();

    }

    public partial class DataCheckService
    {
        private readonly Dictionary<string, string[]> _checkFiles = new Dictionary<string, string[]>()
        {
            {
                "unit", new string[] { "data_unit", "data_unit_stat", "data_passive_condition", "data_skin_group", "data_skin" }
            },
            {
                "skill", new string[] { "data_skill", "data_card" , "data_card_stat", "data_stat", }
            },
            {
                "stage", new string[] { "data_chapter", "data_stage", "data_stage_level", "data_stage_level_flow", "data_stage_round", }
            },
            {
                "monster", new string[] {  "data_monster_group", "data_monster", "data_monster_pattern", "data_monster_pattern_flow", }
            },
            {
                "roguelike", new string[] {   "data_roguelike_save_deck", "data_proposal_treasure", "data_treasure" }
            },
            {
                "weapon", new string[] { "data_weapon", "data_weapon_stat", }
            },
            {
                "rank", new string[] {  "data_rank_tier_matching", "data_virtual_league_matching", "data_unit_rank_point", "data_user_rank", "data_season_reward", "data_user_level" }
            },
            {
                "reward", new string[] {   "data_material", "data_reward", "data_probability_group", "data_reward_notify", "data_product_reward",}
            },
            {
                "store" , new string[]{ "_items", "_rewards", "_products", "_products_all_list", "data_user_product_digital", "data_user_product_real", "_products_text" }
            },
            {
                "quest" , new string[]{ "data_quest"  }
            },
            {
                "square" , new string[]{ "data_square_object", "data_square_object_planet_box", }
            },
            {
                "profile" , new string[]{ "data_profile_icon" }
            },
            {
                "emoticon" , new string[]{ "data_emticon" }
            },
        };



        private IEnumerable<string> _AllCheckFiles => _checkFiles.Values.SelectMany(value => value).Distinct();

        private static KeyValuePair<string, Dictionary<string, string>> CreateAggregationItem(string property, params (string targetFile, string targetProperty)[] aggrTuples)
        {
            var targetAggregations = aggrTuples.ToDictionary((data) => data.targetFile, data => data.targetProperty);
            return new KeyValuePair<string, Dictionary<string, string>>(property, targetAggregations);
        }

        private static Dictionary<string, Dictionary<string, string>> CreateAggregation(
            params KeyValuePair<string, Dictionary<string, string>>[] attributes)
        {

            return attributes.GroupBy(attribute => attribute.Key, attribute => attribute.Value)
                                      .ToDictionary(group => group.Key, group => group.SelectMany( attribute => attribute).ToDictionary(pair =>pair.Key, pair=>pair.Value ));
                
        }

        private static List<string> Properties(params string[] properties) => new List<string>(properties);

        private readonly Dictionary<string, DataCheckMeta> _checkMeta = new Dictionary<string, DataCheckMeta>()
        {
            #region Unit
            {
                "data_unit", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_unit_kr", "TextUnitId"))
                                                                    ,CreateAggregationItem("SkinGroupId", ("data_skin_group", "SkinGroupId")))
                }
            },
            {
                "data_unit_stat", new DataCheckMeta
                {
                    Zeroable =  Properties( "PassiveConditionId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("UnitId", ("data_unit", "UnitId"))
                                                                    ,CreateAggregationItem("PassiveConditionId", ("data_passive_condition", "PassiveConditionId"))
                                                                    ,CreateAggregationItem("ConditionPassiveTextId", ("text_passive_condition_kr", "TextPassiveConditonId")))
                }
            },
            {
                "data_passive_condition", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("CardStatId", ("data_card_stat", "CardStatId")))
                }
            },
            {
                "data_skin_group", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("BaseSkinId", ("data_skin", "SkinId"))
                        ,CreateAggregationItem("SkinIdList", ("data_skin", "SkinId")))
                }
            },
            {
                "data_skin", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_skin_kr", "TextSkinId"))
                        ,CreateAggregationItem("VoiceSkinTextId", ("text_skin_voice_kr", "VoiceSkinTextId")))
                }
            },

            #endregion
            
            #region Skill
            {
                "data_skill", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("SkillOptionList", ("data_skill_option", "SkillOptionId")))
                }
            },
            {
                "data_card", new DataCheckMeta
                {
                    Zeroable =  Properties( "UnitId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("UnitId", ("data_unit", "UnitId")))
                }
            },
            {
                "data_card_stat", new DataCheckMeta
                {
                    Zeroable =  Properties( "SkillConditionId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_card_kr", "TextCardId"))
                        ,CreateAggregationItem("SkillConditionId", ("data_skill_condition", "SkillConditionId"))
                        ,CreateAggregationItem("SkillIdList", ("data_skill", "SkillId")))
                }
            },
            {
                "data_stat", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_state_kr", "TextStateId")))
                }
            },
            #endregion

            #region Stage
            {
                "data_chapter", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_chapter_kr", "TextChapterId")))
                }
            },
            {
                "data_stage", new DataCheckMeta
                {
                    Zeroable =  Properties( "ChapterId","TextId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("ChapterId", ("data_chapter", "ChapterId"))
                        ,CreateAggregationItem("TextId", ("text_stage_kr", "TextStageId")))
                }
            },
            {
                "data_stage_level", new DataCheckMeta
                {
                    Zeroable =  Properties( "RewardId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("StageId", ("data_stage", "StageId"))
                        ,CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },
            {
                "data_stage_level_flow", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("StageLevelId", ("data_stage_level", "StageLevelId"))
                        ,CreateAggregationItem("OpenStageIdList", ("data_stage_level", "StageLevelId")))
                }
            },
            {
                "data_stage_round", new DataCheckMeta
                {
                    /////////////////////
                    Aggregation = CreateAggregation(CreateAggregationItem("StageLevelId", ("data_stage_level", "StageLevelId"))
                        ,CreateAggregationItem("MonsterGroupIdList", ("data_monster_group", "MonsterGroupId")))
                  //      ,CreateAggregationItem("BackgroundImageId", ("data_background", "BackgroundId")))
                }
            },

            #endregion
            
            #region Monster
           {
               "data_monster_group", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("MonsterIdList", ("data_monster", "MonsterId")))
               }
           },
           {
               "data_monster", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("BaseUnitId", ("data_unit", "UnitId"))
                    ,CreateAggregationItem("MonsterPatternIdList", ("data_monster_pattern", "MonsterPatternId")))
               }
           },
           {
               "data_monster_pattern", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("MonsterPatternConditionId", ("data_monster_pattern_condition", "MonsterPatternConditionId"))
                       ,CreateAggregationItem("CardStatId", ("data_card", "CardStatId"))
                       ,CreateAggregationItem("MonsterPatternFlowIdList", ("data_monster_pattern_flow", "MonsterPatternFlowId")))
               }
           },
           {
               "data_monster_pattern_flow", new DataCheckMeta
               {
                   Zeroable =  Properties( "MonsterPatternConditionIdList","TransMonsterPatternId" ),
                   Aggregation = CreateAggregation(CreateAggregationItem("MonsterPatternConditionIdList", ("data_monster_pattern_condition", "MonsterPatternConditionId"))
                       ,CreateAggregationItem("TransMonsterPatternId", ("data_monster_pattern", "MonsterPatternId")))
               }
           },

            #endregion
            
            #region Roguelike
           {
               "data_roguelike_save_deck", new DataCheckMeta
               {
                   Zeroable =  Properties( "ProposalCardStatList0","ProposalCardStatList1","ProposalCardStatList2","ProposalCardStatList3","ProposalCardStatList4","ProposalCardStatList5","ProposalCardStatList6","ProposalCardStatList7","ProposalCardStatList8" ),
                   Aggregation = CreateAggregation(CreateAggregationItem("UnitIdList", ("data_unit", "UnitId"))
                       ,CreateAggregationItem("ProposalTreasureId", ("data_proposal_treasure", "ProposalTreasureId"))
                       ,CreateAggregationItem("CardStatIdList", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList0", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList1", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList2", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList3", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList4", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList5", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList6", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList7", ("data_card_stat", "CardStatId"))
                       ,CreateAggregationItem("ProposalCardStatList8", ("data_card_stat", "CardStatId")))
               }
           },
           {
               "data_proposal_treasure", new DataCheckMeta
               {
                   Zeroable =  Properties( "TreasureIdList0","TreasureIdList1","TreasureIdList2","TreasureIdList3","TreasureIdList4","TreasureIdList5","TreasureIdList6","TreasureIdList7","TreasureIdList8" ),
                   Aggregation = CreateAggregation(
                       CreateAggregationItem("TreasureIdList0", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList1", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList2", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList3", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList4", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList5", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList6", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList7", ("data_treasure", "TreasureId"))
                       ,CreateAggregationItem("TreasureIdList8", ("data_treasure", "TreasureId")))
               }
           },
           {
               "data_treasure", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("PassiveConditionId", ("data_passive_condition", "PassiveConditionId"))
                       ,CreateAggregationItem("TextId", ("text_treasure_kr", "TextTreasureId")))
               }
           },
            #endregion
            
            #region Weapon
           {
               "data_weapon", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_weapon_kr", "TextWeaponId")))
               }
           },
           {
               "data_weapon_stat", new DataCheckMeta
               {
                   Aggregation = CreateAggregation(CreateAggregationItem("WeaponId", ("data_weapon", "WeaponId"))
                       ,CreateAggregationItem("PassiveConditionId", ("data_passive_condition", "PassiveConditionId"))
                       ,CreateAggregationItem("ConditionPassiveTextId", ("text_passive_condition_kr", "TextPassiveConditonId")))
               }
           },
            #endregion
         
            #region Rank
            {
                "data_rank_tier_matching", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_rank_tier_kr", "TextRankTierId"))
                        ,CreateAggregationItem("StageRoundIdList", ("data_stage_round", "StageRoundId")))
                }
            },
            {
                "data_virtual_league_matching", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("StageRoundIdList", ("data_stage_round", "StageRoundId")))
                }
            },
            {
                "data_unit_rank_point", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },
            {
                "data_user_rank", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_user_rank_kr", "TextUserRankId"))
                        ,CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },
            {
                "data_season_reward", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },
            {
                "data_user_level", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },

            #endregion
            
            #region Reward
            {
                "data_material", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_material_kr", "TextMaterialId")))
                }
            },
            {
                "data_reward", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_reward_kr", "TextRewardId"))
                        ,CreateAggregationItem("ItemIdList", ("data_item", "ItemId"))
                        ,CreateAggregationItem("RewardNotifyGroupId", ("data_reward_notify", "RewardNotifyId")))
                }
            },
            {
                "data_probability_group", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("ElementId", ("data_reward", "RewardId")))
                }
            },
            {
                "data_reward_notify", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_reward_notify_kr", "TextRewardNotify")))
                }
            },
            {
                "data_product_reward", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_product_reward_kr", "ProductRewardTextId")))
                }
            },

            #endregion
            
            #region Store
            {
                "_items", new DataCheckMeta
                {
              //      Keys = Properties("ItemId")
                }
            },
            {
                "_rewards", new DataCheckMeta
                {
               //     Keys =  Properties( "RewardId"),
                    Aggregation = CreateAggregation(CreateAggregationItem("ItemId", ("_items", "ItemId")))
                }
            },
            {
                "_products", new DataCheckMeta
                {
              //      Keys = Properties( "ProductId"),
                    Aggregation = CreateAggregation(CreateAggregationItem("RewardId", ("_rewards", "RewardId")))
                }
            },
            {
                "_products_all_list", new DataCheckMeta
                {
                    Keys =  Properties( "ProductId", "AosStoreProductId", "IosStoreProductId" ),
                    Nullable =  Properties( "AosStoreProductId", "IosStoreProductId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("ProductId", ("_products", "ProductId")))
                }
            },
            {
                "data_user_product_digital", new DataCheckMeta
                {
                    Keys =  Properties("ProductId"),
                    Aggregation = CreateAggregation(CreateAggregationItem("ProductId", ("_products", "ProductId")))
                }
            },
            {
                "data_user_product_real", new DataCheckMeta
                {
                    Keys = Properties("ProductId"),
                    Aggregation = CreateAggregation(CreateAggregationItem("ProductId", ("_products", "ProductId")))
                }
            },
            {
                "_products_text", new DataCheckMeta
                {
                    Keys = Properties( "ProductId"),
                    Aggregation = CreateAggregation(CreateAggregationItem("ProductId", ("_products", "ProductId")))
                }
            },

#endregion

            #region Quest
            {
                "data_quest", new DataCheckMeta
                {
                    Zeroable =  Properties( "TextId","RewardId" ),
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_quest_kr", "TextQuestId"))
                        ,CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },

            #endregion
            
            #region Square
            {
                "data_square_object", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("InvasionLv1List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv2List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv3List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv4List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv5List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv6List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv7List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv8List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv9List", ("data_square_object_monster", "SquareObjectMonsterId"))
                        ,CreateAggregationItem("InvasionLv10List", ("data_square_object_monster", "SquareObjectMonsterId")))
                }
            },
            {
                "data_square_object_planet_box", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("RewardId", ("data_reward", "RewardId")))
                }
            },

            #endregion

            #region Profile
            {
                "data_profile_icon", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("TextId", ("text_profile_icon", "TextNoticeId")))
                }
            },
            #endregion
            
            #region Emoticon
            {
                "data_emticon", new DataCheckMeta
                {
                    Aggregation = CreateAggregation(CreateAggregationItem("UnitId", ("data_unit", "UnitId")))
                }
            },
            #endregion
        };
    }
}