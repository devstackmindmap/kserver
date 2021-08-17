using AkaEnum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkaData
{
    public static class Data
    {
        public static ICollection<TValue> GetPrimitiveValues<TKey, TValue>(DataType dataType)
            where TKey : IComparable
        {
            return DataGetter.GetPrimitiveValues<TKey, TValue>(dataType);
        }

        public static IDictionary<TKey, TValue> GetPrimitiveDict<TKey, TValue>(DataType dataType)
            where TKey : IComparable
        {
            return DataGetter.GetPrimitiveDict<TKey, TValue>(dataType);
        }

        public static IEnumerable<uint> GetAllBackgroundImageId()
        {
            var backgroundList = DataGetter.GetPrimitiveDict<uint, List<DataBackground>>(DataType.data_background);
            return backgroundList.SelectMany(backgroundDic => backgroundDic.Value.Select(background => background.BackgroundImageId)).Distinct();
        }

        public static IEnumerable<DataAiName> GetAiNames()
        {
            return GetPrimitiveValues<uint, DataAiName>(DataType.data_ai_name);
        }

        public static DataPassiveCondition GetPassive(uint passiveId)
        {
            return DataGetter.GetGameData<uint, DataPassiveCondition>(DataType.data_passive_condition, passiveId);
        }

        public static DataUserRank GetUserRank(uint userRankLevelId)
        {
            return DataGetter.GetGameData<uint, DataUserRank>(DataType.data_user_rank, userRankLevelId);
        }

        public static DataUnitRankPoint GetUnitRankPoint(uint unitRankLevelId)
        {
            return DataGetter.GetGameData<uint, DataUnitRankPoint>(DataType.data_unit_rank_point, unitRankLevelId);
        }

        public static DataDefaultDeckSet GetDefaultDeckSet(ModeType modeType)
        {
            return DataGetter.GetGameData<ModeType, DataDefaultDeckSet>(DataType.data_default_deck_set, modeType);
        }

        public static DataStageLevel GetStageLevel(uint stageLevelId)
        {
            return DataGetter.GetGameData<uint, DataStageLevel>(DataType.data_stage_level, stageLevelId);
        }

        public static DataRoguelikeSaveDeck GetRoguelikeSaveDeck(uint roguelikeSaveDeclId)
        {
            return DataGetter.GetGameData<uint, DataRoguelikeSaveDeck>(DataType.data_roguelike_save_deck, roguelikeSaveDeclId);
        }

        public static DataTreasure GetTreasure(uint treasureId)
        {
            return DataGetter.GetGameData<uint, DataTreasure>(DataType.data_treasure, treasureId);
        }

        public static DataProposalTreasure GetProposalTreasure(uint proposalTreasureId)
        {
            return DataGetter.GetGameData<uint, DataProposalTreasure>(DataType.data_proposal_treasure, proposalTreasureId);
        }

        public static DataStageRound GetStageRound(uint stageRoundId)
        {
            return DataGetter.GetGameData<uint, DataStageRound>(DataType.data_stage_round, stageRoundId);
        }

        public static DataStageRound GetStageRound(uint stageLevelid, uint round)
        {
            return DataGetter.GetGameData<uint, uint, DataStageRound>(DataType.data_stage_round_by_round_num, stageLevelid, round);
        }

        public static bool IsValidStageRound(uint stageLevelid, uint round)
        {
            return DataGetter.IsContainsKey<uint, uint, DataStageRound>(DataType.data_stage_round_by_round_num, stageLevelid, round);
        }

        public static List<DataStageRound> GetStageRoundList(uint stageLevelid)
        {
            return DataGetter.GetGameData<uint, List<DataStageRound>>(DataType.data_stage_round_for_level, stageLevelid);
        }

        public static DataMonsterGroup GetMonsterGroup(uint monsterListId)
        {
            return DataGetter.GetGameData<uint, DataMonsterGroup>(DataType.data_monster_group, monsterListId);
        }


        public static DataMonster GetMonster(uint monsterId)
        {
            return DataGetter.GetGameData<uint, DataMonster>(DataType.data_monster, monsterId);
        }

        public static DataMonsterPattern GetMonsterPattern(uint monsterPatternId)
        {
            return DataGetter.GetGameData<uint, DataMonsterPattern>(DataType.data_monster_pattern, monsterPatternId);
        }

        public static DataMonsterPatternFlow GetMonsterPatternFlow(uint monsterPatternFlowId)
        {
            return DataGetter.GetGameData<uint, DataMonsterPatternFlow>(DataType.data_monster_pattern_flow, monsterPatternFlowId);
        }

        public static DataMonsterPatternCondition GetMonsterPatternCondition(uint monsterPatternConditionId)
        {
            return DataGetter.GetGameData<uint, DataMonsterPatternCondition>(DataType.data_monster_pattern_condition, monsterPatternConditionId);
        }

        public static DataSkillOption GetSkillOption(uint skillOptionId)
        {
            return DataGetter.GetGameData<uint, DataSkillOption>(DataType.data_skill_option, skillOptionId);
        }

        public static DataConstant GetConstant(DataConstantType constantType)
        {
            return DataGetter.GetGameData<DataConstantType, DataConstant>(DataType.data_constant, constantType);
        }

        public static DataContentsConstant GetContentsConstant(AkaEnum.Battle.BattleType battleType)
        {
            return DataGetter.GetGameData<AkaEnum.Battle.BattleType, DataContentsConstant>(DataType.data_contents_constant, battleType);
        }

        public static DataRankTierMatching GetRankTierMatching(int rankTierMatchingId)
        {
            return DataGetter.GetGameData<int, DataRankTierMatching>(DataType.data_rank_tier_matching, rankTierMatchingId);
        }

        public static DataVirtualLeagueTearMatching GetVirtualLeagueTierMatching(int virtualLeagueMatchingId)
        {
            return DataGetter.GetGameData<int, DataVirtualLeagueTearMatching>(DataType.data_virtual_league_matching, virtualLeagueMatchingId);
        }

        public static int GetAnimationLengthBullet(string unitName, AnimationType animationName)
        {
            return DataGetter.GetGameData<DataAnimationLengthMap>(DataType.data_animation_length).AnimationMap[unitName].AnimationLengths[animationName.ToString()].Bullet;
        }

        public static int GetAnimationTakeDamageLength(string unitName, AnimationType animationName)
        {
            return DataGetter.GetGameData<DataAnimationLengthMap>(DataType.data_animation_length).AnimationMap[unitName].AnimationLengths[animationName.ToString()].TakeDamage;
        }

        public static DataAnimationLength GetAnimationLength(string unitName, AnimationType animationName)
        {
            if (unitName.Equals("Empty"))
                return new DataAnimationLength();

            return DataGetter.GetGameData<DataAnimationLengthMap>(DataType.data_animation_length).AnimationMap[unitName].AnimationLengths[animationName.ToString()];
        }

        public static DataUnit GetUnit(uint unitId)
        {
            return DataGetter.GetGameData<uint, DataUnit>(DataType.data_unit, unitId);
        }

        public static UnitAdditionalData GetUnitAdditionalData(uint id)
        {
            return DataGetter.GetGameData<uint, UnitAdditionalData>(DataType.additional_unit_info, id);
        }

        public static CardAdditionalData GetCardAdditionalData(uint id)
        {
            return DataGetter.GetGameData<uint, CardAdditionalData>(DataType.additional_card_info, id);
        }

        public static List<uint> GetCardLevel1(uint unitId)
        {
            return DataGetter.GetGameData<uint, List<uint>>(DataType.data_card_level_1, unitId);
        }

        public static List<uint> GetCardByUnit(uint unitId)
        {
            return DataGetter.GetGameData<uint, List<uint>>(DataType.data_card_by_unit, unitId);
        }

        public static List<CardUseLevelInfo> GetCardUnlockTypeNormal(uint unitId)
        {
            return DataGetter.GetGameData<uint, List<CardUseLevelInfo>>(DataType.data_card_unlock_type_normal, unitId);
        }

        public static WeaponAdditionalData GetWeaponAdditionalData(uint id)
        {
            return DataGetter.GetGameData<uint, WeaponAdditionalData>(DataType.additional_weapon_info, id);
        }

        public static IDictionary<TableKey<uint, uint>, DataUnitStat> GetDataUnitStat()
        {
            return DataGetter.GetGameData<uint, uint, DataUnitStat>(DataType.data_unit_stat);
        }

        public static UnitStat GetUnitStat(uint unitId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, UnitStat>(DataType.unit_stat, unitId, level);
        }

        public static DataCard GetCard(uint cardId)
        {
            return DataGetter.GetGameData<uint, DataCard>(DataType.data_card, cardId);
        }

        public static DataCardStat GetCardStat(uint cardId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataCardStat>(DataType.data_card_stat, cardId, level);
        }

        public static DataCardStat GetCardStat(uint cardStatId)
        {
            return DataGetter.GetGameData<uint, DataCardStat>(DataType.data_card_stat_all, cardStatId);
        }

        public static IDictionary<uint, DataSkill> GetDataSkill()
        {
            return DataGetter.GetGameData<uint, DataSkill>(DataType.data_skill);
        }

        public static SkillWithoutAnimationData GetSkillWithoutAnimationData(uint skillId)
        {
            return DataGetter.GetGameData<uint, SkillWithoutAnimationData>(DataType.skill, skillId);
        }

        public static Skill GetSkill(uint skillId, string unitInitial)
        {
            var skillData = DataGetter.GetGameData<uint, SkillWithoutAnimationData>(DataType.skill, skillId);
            var dataAnimationData
                = DataGetter.GetGameData<DataAnimationLengthMap>(DataType.data_animation_length)
                .AnimationMap[unitInitial].AnimationLengths[skillData.AnimationType.ToString()];

            return new Skill
            {
                SkillData = skillData,
                AnimationData = new AnimationData
                {
                    AnimationLength = dataAnimationData?.Bullet ?? 0,
                    BulletTime = dataAnimationData?.Bullet ?? 0,
                    TakeDamageTime = dataAnimationData?.TakeDamage ?? 0
                }
            };
        }

        public static DataSkill GetDataSkill(uint skillId)
        {
            return DataGetter.GetGameData<uint, DataSkill>(DataType.data_skill, skillId);
        }

        public static DataWeapon GetWeapon(uint weaponId)
        {
            return DataGetter.GetGameData<uint, DataWeapon>(DataType.data_weapon, weaponId);
        }

        public static DataWeaponStat GetWeaponStat(uint weaponId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataWeaponStat>(DataType.data_weapon_stat, weaponId, level);
        }

        public static DataProduct GetProduct(uint productId)
        {
            return DataGetter.GetGameData<uint, DataProduct>(DataType.data_product, productId);
        }

        public static DataReward GetReward(uint rewardId)
        {
            return DataGetter.GetGameData<uint, DataReward>(DataType.data_reward, rewardId);
        }

        public static List<DataItem> GetItem(uint itemId)
        {
            return DataGetter.GetGameData<uint, List<DataItem>>(DataType.data_item, itemId);
        }

        public static List<DataProbabilityGroup> GetProbabilityGroup(uint probabilityGroupId)
        {
            return DataGetter.GetGameData<uint, List<DataProbabilityGroup>>(DataType.data_probability_group, probabilityGroupId);
        }

        public static DataProbabilityGroup GetProbabilityGroup(uint probabilityGroupId, uint elementId)
        {
            return DataGetter.GetGameData<uint, uint, DataProbabilityGroup>(DataType.data_probability_element, probabilityGroupId, elementId);
        }

        public static bool IsExistNextStageLevel(uint stageLevelId)
        {
            return (DataGetter.ContainsKey<uint, DataStageLevelFlow>(DataType.data_stage_level_flow, stageLevelId));
        }

        public static IList<uint> GetOpenStageIdList(uint stageLevelId)
        {
            return DataGetter.GetGameData<uint, DataStageLevelFlow>(DataType.data_stage_level_flow, stageLevelId)?.OpenStageIdList;
        }

        public static int GetRequireUnitPieceCountForLevelUp(uint unitId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataUnitStat>(DataType.data_unit_stat, unitId, level).RequirePieceCountForNextLevelUp;
        }

        public static int GetRequireUnitGoldForLevelUp(uint unitId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataUnitStat>(DataType.data_unit_stat, unitId, level).NeedGoldForNextLevelUp;
        }

        public static int GetRequireCardPieceCountForLevelUp(uint cardId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataCardStat>(DataType.data_card_stat, cardId, level).RequirePieceCountForNextLevelUp;
        }

        public static int GetRequireCardGoldForLevelUp(uint cardId, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataCardStat>(DataType.data_card_stat, cardId, level).NeedGoldForNextLevelUp;
        }

        public static int GetRequireWeaponPieceCountForLevelUp(uint id, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataWeaponStat>(DataType.data_weapon_stat, id, level).RequirePieceCountForNextLevelUp;
        }

        public static int GetRequireWeaponGoldForLevelUp(uint id, uint level)
        {
            return DataGetter.GetGameData<uint, uint, DataWeaponStat>(DataType.data_weapon_stat, id, level).NeedGoldForNextLevelUp;
        }

        public static int GetRankTierMatchingId(int sumOfRankPoint)
        {
            var dataRankTierMatchingInMyTierRange = GetPrimitiveValues<int, DataRankTierMatching>(DataType.data_rank_tier_matching).FirstOrDefault(data =>
            {
                return data.TeamRankPointForMatching > sumOfRankPoint;
            });

            if (dataRankTierMatchingInMyTierRange == null)
            {
                dataRankTierMatchingInMyTierRange = GetPrimitiveValues<int, DataRankTierMatching>(DataType.data_rank_tier_matching).Last();
            }

            return dataRankTierMatchingInMyTierRange.RankTierMatchingId;
        }

        public static int GetVirtualLeagueTierMatchingId(int sumOfRankPoint)
        {
            var virtualTierMatchingInMyTierRange = GetPrimitiveValues<int, DataVirtualLeagueTearMatching>(DataType.data_virtual_league_matching).FirstOrDefault(data =>
            {
                return data.TeamRankPointForMatching > sumOfRankPoint;
            });

            if (virtualTierMatchingInMyTierRange == null)
            {
                virtualTierMatchingInMyTierRange = GetPrimitiveValues<int, DataVirtualLeagueTearMatching>(DataType.data_virtual_league_matching).Last();
            }

            return virtualTierMatchingInMyTierRange.VirtualLeagueMatchingId;
        }

        public static void GetUserRankLevelIdFromPoint(int point, out int nextPoint, out int minPoint)
        {
            int sumOfPoint = 0;
            var findData = GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank).FirstOrDefault(data =>
            {
                sumOfPoint += data.NeedRankPointForNextLevelUp;
                if (sumOfPoint > point)
                    return true;
                return false;
            });

            if (findData == null)
            {
                findData = GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank).Last();
                minPoint = sumOfPoint;
                nextPoint = point;
            }
            else
            {
                minPoint = sumOfPoint - findData.NeedRankPointForNextLevelUp;
                nextPoint = sumOfPoint;
            }
        }

        public static uint GetUserRankLevelByPoint(int point)
        {
            uint level = 0;
            var findData = GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank).FirstOrDefault(data =>
            {
                if (data.NeedRankPointForNextLevelUp > point)
                {
                    level = data.UserRankLevelId;
                    return true;
                }
                return false;
            });

            if (findData != null)
                return level;

            findData = GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank).Last();
            return findData.UserRankLevelId;
        }

        public static int GetUnitRankPointForZeroPoint()
        {
            return GetPrimitiveValues<uint, DataUnitRankPoint>(DataType.data_unit_rank_point).TakeWhile(data => data.LosePoint == 0)
                                                                                            .Sum(data => data.NeedRankPointForNextLevelUp);
        }

        public static DataSkillCondition GetSkillCondition(uint id)
        {
            return DataGetter.GetGameData<uint, DataSkillCondition>(DataType.data_skill_condition, id);
        }

        public static DataSkinGroup GetDataSkinGroup(uint skinGroupId)
        {
            return DataGetter.GetGameData<uint, DataSkinGroup>(DataType.data_skin_group, skinGroupId);
        }

        public static DataSkin GetDataSkin(uint skinId)
        {
            return DataGetter.GetGameData<uint, DataSkin>(DataType.data_skin, skinId);
        }

        public static DataUserLevel GetUserLevel(uint levelId)
        {
            return DataGetter.GetGameData<uint, DataUserLevel>(DataType.data_user_level, levelId);
        }

        public static List<uint> GetUnitIdsByRewardProbabilityType()
        {
            return DataGetter.GetGameData<List<uint>>(DataType.data_unit_by_reward_probability_type);
        }

        public static List<uint> GetUnitIdsByFirstType()
        {
            return DataGetter.GetGameData<List<uint>>(DataType.data_unit_by_first_type);
        }

        public static List<uint> GetAllWeaponIds()
        {
            return DataGetter.GetGameData<List<uint>>(DataType.data_weapon_ids);
        }

        public static List<DataBackground> GetBackgroundList(uint backgroundId)
        {
            return DataGetter.GetGameData<uint, List<DataBackground>>(DataType.data_background, backgroundId);
        }

        public static IEnumerable<DataEmoticon> GetAllEmoticons()
        {
            return GetPrimitiveValues<uint, DataEmoticon>(DataType.data_emoticon);
        }

        public static DataEmoticon GetEmoticon(uint emoticonId)
        {
            return DataGetter.GetGameData<uint, DataEmoticon>(DataType.data_emoticon, emoticonId);
        }

        public static uint GetUnlockEmoticonIdByUnitId(uint unitId)
        {
            return DataGetter.GetGameData<uint, uint>(DataType.data_unlock_emoticon_by_unitId, unitId);
        }

        public static DataProfileIcon GetProfileIcon(uint profileIconId)
        {
            return DataGetter.GetGameData<uint, DataProfileIcon>(DataType.data_profile_icon, profileIconId);
        }

        public static IDictionary<uint, List<DataQuest>> GetAllQuests()
        {
            return DataGetter.GetGameData<uint, List<DataQuest>>(DataType.data_quest);
        }

        public static List<DataQuest> GetQuest(uint questGroupId)
        {
            if (questGroupId == 0)
                return default;

            return DataGetter.GetGameData<uint, List<DataQuest>>(DataType.data_quest, questGroupId);
        }


        public static List<uint> GetQuestWithProcessType(QuestProcessType processType)
        {
            return DataGetter.GetGameDataNotErrLog<QuestProcessType, List<uint>>(DataType.data_quest_processtype, processType);
        }

        public static List<DataUserInitData> GetUserInitDatasList()
        {
            return DataGetter.GetGameData<List<DataUserInitData>>(DataType.data_user_init_datas_all);
        }

        public static DataUserInitData GetUserInitData(uint version)
        {
            return DataGetter.GetGameData<uint, DataUserInitData>(DataType.data_user_init_datas, version);
        }

        public static DataSquareObject GetSquareObject(uint squareObjectLevel)
        {
            return DataGetter.GetGameData<uint, DataSquareObject>(DataType.data_square_object, squareObjectLevel);
        }

        public static List<DataSquareObjectPlanetBox> GetSquareObjectPlanetBoxList(uint squareObjectLevel)
        {
            return DataGetter.GetGameData<uint, List<DataSquareObjectPlanetBox>>(DataType.data_square_object_planet_box, squareObjectLevel);
        }

        public static DataSquareObjectPlanetBox GetSquareObjectPlanetBox(uint squareObjectLevel, uint planetBoxLevel)
        {
            return DataGetter.GetGameData<uint, uint, DataSquareObjectPlanetBox>(DataType.data_square_object_planet_box_with_boxlevel, squareObjectLevel, planetBoxLevel);

        }

        public static DataSquareObjectPlanetCore GetSquareObjectPlanetCore(uint planetCoreLevel)
        {
            return DataGetter.GetGameData<uint, DataSquareObjectPlanetCore>(DataType.data_square_object_planet_core, planetCoreLevel);
        }

        public static DataSquareObjectPlanetAgency GetSquareObjectPlanetAgency(uint planetAgencyLevel)
        {
            return DataGetter.GetGameData<uint, DataSquareObjectPlanetAgency>(DataType.data_square_object_planet_agency, planetAgencyLevel);
        }

        public static DataSquareObjectMonsterInvasionTime GetSquareObjectMonsterInvasionTime(uint invasionLevel)
        {
            return DataGetter.GetGameData<uint, DataSquareObjectMonsterInvasionTime>(DataType.data_square_object_monster_invasion_time, invasionLevel);
        }

        public static List<DataSquareObjectMonsterInvasionTime> GetAllSquareObjectMonsterInvasionTimes()
        {
            return DataGetter.GetGameData<List<DataSquareObjectMonsterInvasionTime>>(DataType.data_square_object_monster_invasion_time_all);
        }

        public static DataSquareObjectMonster GetSquareObjectMonster(uint squareObjectMonsterId)
        {
            return DataGetter.GetGameData<uint, DataSquareObjectMonster>(DataType.data_square_object_monster, squareObjectMonsterId);
        }

        public static DataSeasonReward GetSeasonReward(uint userRankLevelId)
        {
            return DataGetter.GetGameData<uint, DataSeasonReward>(DataType.data_season_reward, userRankLevelId);
        }

        public static uint GetProfileIconIdByUnitUnlock(uint unitId)
        {
            return DataGetter.GetGameData<uint, uint>(DataType.data_profile_icon_by_unit, unitId);
        }

        public static List<DataSlang> GetSlang()
        {
            return DataGetter.GetGameData<List<DataSlang>>(DataType.data_slang);
        }

        public static DataUserProductDigital GetDataUserProductDigital(uint productId)
        {
            return DataGetter.GetGameData<uint, DataUserProductDigital>(DataType.data_user_product_digital, productId);
        }

        public static DataUserProductReal GetDataUserProductReal(uint productId)
        {
            return DataGetter.GetGameData<uint, DataUserProductReal>(DataType.data_user_product_real, productId);
        }

        public static List<DataMaterial> GetAllMaterials()
        {
            return DataGetter.GetGameData<List<DataMaterial>>(DataType.data_material_all);
        }

        public static DataMaterial GetMaterial(MaterialType materialType)
        {
            return DataGetter.GetGameData<MaterialType, DataMaterial>(DataType.data_material, materialType);
        }

        public static DataSeasonPass GetSeasonPass(uint seasonPassId)
        {
            return DataGetter.GetGameData<uint, DataSeasonPass>(DataType.data_season_pass, seasonPassId);
        }

        public static List<DataSeasonPass> GetSeasonPassListForSeason(uint season)
        {
            return DataGetter.GetGameData<uint, List<DataSeasonPass>>(DataType.data_season_pass_list, season);
        }

        public static DataUnderdog GetUnderdog(int MyRankInterval)
        {
            return DataGetter.GetGameData<int, DataUnderdog>(DataType.data_underdog, MyRankInterval);

        }

        public static DataMail GetDataMail(uint mailId)
        {
            return DataGetter.GetGameData<uint, DataMail>(DataType.data_mail, mailId);
        }

        public static List<DataSpendMaterial> GetSpendMaterials(BehaviorType behaviorType)
        {
            return DataGetter.GetGameData<BehaviorType, List<DataSpendMaterial>>(DataType.data_spend_materials, behaviorType);
        }

        public static DataState GetState(SkillEffectType skillEffectType)
        {
            return DataGetter.GetGameData<SkillEffectType, DataState>(DataType.data_state, skillEffectType);
        }

        public static DataFriendEvent GetFriendEvent(uint rewardNum)
        {
            return DataGetter.GetGameData<uint, DataFriendEvent>(DataType.data_friend_event, rewardNum);
        }

        public static DataChallenge GetDataChallenge(uint season, int day)
        {
            return DataGetter.GetGameData<uint, int, DataChallenge>(DataType.data_challenge, season, day);
        }

        public static DataChallengeEvent GetDataChallengeEvent(uint eventId)
        {
            return DataGetter.GetGameData<uint, DataChallengeEvent>(DataType.data_challenge_event, eventId);
        }

        public static DataChallengeAffix GetDataChallengeAffix(uint season)
        {
            return DataGetter.GetGameData<uint, DataChallengeAffix>(DataType.data_challenge_affix, season);
        }

        public static DataAffix GetDataAffix(uint affixId)
        {
            return DataGetter.GetGameData<uint, DataAffix>(DataType.data_affix, affixId);
        }
    }
}
