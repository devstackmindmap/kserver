using AkaEnum;
using AkaLogger;
using AkaUtility;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;


namespace AkaData
{
    public class DataSetter
    {
        public void DataSet(List<ProtoFileInfo> fileList)
        {
            foreach (var fileInfo in fileList)
            {
                try
                {
                    var dataType = ParseDataType(fileInfo.Name);
                    if (dataType == null)
                        continue;

                    DataSet(dataType.Value, fileInfo.Url);
                }
                catch (Exception e)
                {
                    Log.Debug.Exception("DataSet:" + fileInfo.Name + " Url:" + fileInfo.Url, e);
                    throw new Exception("Name: " + fileInfo.Name + ", Url: " + fileInfo.Url);
                }
            }

            var maker = new AdditionalDataMaker();
            maker.AllMake();
        }
        
        private DataType? ParseDataType(string fileName)
        {
            var isSuccess = Enum.TryParse<DataType>(fileName, out var dataType);
            if (!isSuccess)
                return null;

            return dataType;
        }

        private void DataSet(DataType dataType, string url)
        {
            switch (dataType)
            {
                case DataType.data_ai_name:
                    RegisterTableAsDict<uint, DataAiName>(url, dataType, "AiNameId");
                    break;

                case DataType.data_animation_length:
                    RegisterTableAsJson<DataAnimationLengthMap>(url, dataType);
                    break;

                case DataType.data_rank_tier_matching:
                    RegisterTableAsDictWithMapping<int, DataRankTierMatching, MappingDataRankTierMatching>(url, dataType, "RankTierMatchingId");
                    break;

                case DataType.data_virtual_league_matching:
                    RegisterTableAsDictWithMapping<int, DataVirtualLeagueTearMatching, MappingDataVirtualLeagueTearMatching>(url, dataType, "VirtualLeagueMatchingId");
                    break;

                case DataType.data_unit_rank_point:
                    RegisterTableAsDict<uint, DataUnitRankPoint>(url, dataType, "UnitRankLevelId");
                    break;

                case DataType.data_unit:
                    RegisterTableAsDict<uint, DataUnit>(url, dataType, "UnitId");
                    break;

                case DataType.data_unit_stat:
                    RegisterTableAsDictWithMapping<uint, uint, DataUnitStat, MappingDataUnitStat>(url, dataType, "UnitId", "Level");
                    RegisterTableAsDictWithMapping<uint, DataUnitStat, MappingDataUnitStat>(url, DataType.data_unit_stat_all, "UnitStatId");
                    break;

                case DataType.data_constant:
                    RegisterTableAsDict<DataConstantType, DataConstant>(url, dataType, "Type");
                    break;

                case DataType.data_contents_constant:
                    RegisterTableAsDict<AkaEnum.Battle.BattleType, DataContentsConstant>(url, dataType, "BattleType");
                    break;

                case DataType.data_skill:
                    RegisterTableAsDictWithMapping<uint, DataSkill, MappingDataSkill>(url, dataType, "SkillId");
                    break;

                case DataType.data_card_stat:
                    RegisterTableAsDictWithMapping<uint, uint, DataCardStat, MappingDataCardStat>(url, DataType.data_card_stat, "CardId", "Level");
                    RegisterTableByInjectionOfOtherTable<uint, uint, uint, DataCardStat>(url, DataType.data_card_stat_all, DataType.data_card_stat, "CardStatId");
                    break;

                case DataType.data_card:
                    RegisterTableAsDict<uint, DataCard>(url, dataType, "CardId");
                    break;

                case DataType.data_skill_option:
                    RegisterTableAsDict<uint, DataSkillOption>(url, dataType, "SkillOptionId");
                    break;

                case DataType.data_monster:
                    RegisterTableAsDictWithMapping<uint, DataMonster, MappingDataMonster>(url, dataType, "MonsterId");
                    break;

                case DataType.data_monster_pattern:
                    RegisterTableAsDictWithMapping<uint, DataMonsterPattern, MappingDataMonsterPattern>(url, dataType, "MonsterPatternId");
                    break;

                case DataType.data_monster_pattern_condition:
                    RegisterTableAsDict<uint, DataMonsterPatternCondition>(url, dataType, "MonsterPatternConditionId");
                    break;

                case DataType.data_monster_pattern_flow:
                    RegisterTableAsDictWithMapping<uint, DataMonsterPatternFlow, MappingDataMonsterPatternFlow>(url, dataType, "MonsterPatternFlowId");
                    break;

                case DataType.data_stage_round:
                    RegisterTableAsDictWithMapping<uint, DataStageRound, MappingDataStageRound>(url, dataType, "StageRoundId");

                    RegisterTableAsDictListWithMapping<uint, DataStageRound, MappingDataStageRound>
                        (url, DataType.data_stage_round_for_level, "StageLevelId");

                    RegisterTableAsDictWithMapping<uint, uint, DataStageRound, MappingDataStageRound>
                        (url, DataType.data_stage_round_by_round_num, "StageLevelId", "Round");
                    break;

                case DataType.data_monster_group:
                    RegisterTableAsDictWithMapping<uint, DataMonsterGroup, MappingDataMonsterGroup>(url, dataType, "MonsterGroupId");
                    break;

                case DataType.data_product:
                    RegisterTableAsDictWithMapping<uint, DataProduct, MappingDataProduct>(url, dataType, "ProductId");
                    break;

                case DataType.data_reward:
                    RegisterTableAsDictWithMapping<uint, DataReward, MappingDataReward>(url, dataType, "RewardId");
                    break;

                case DataType.data_probability_group:
                    RegisterTableAsDictList<uint, DataProbabilityGroup>(url, dataType, "ProbabilityGroupId");
                    RegisterTableAsDict<uint, uint, DataProbabilityGroup>(url, DataType.data_probability_element, "ProbabilityGroupId", "ElementId");
                    break;

                case DataType.data_item:
                    RegisterTableAsDictList<uint, DataItem>(url, dataType, "ItemId");
                    break;

                case DataType.data_passive_condition:
                    RegisterTableAsDict<uint, DataPassiveCondition>(url, dataType, "PassiveConditionId");
                    break;

                case DataType.data_weapon_stat:
                    RegisterTableAsDict<uint, uint, DataWeaponStat>(url, dataType, "WeaponId", "Level");
                    RegisterTableAsDict<uint, DataWeaponStat>(url, DataType.data_weapon_stat_all, "WeaponStatId");
                    break;

                case DataType.data_weapon:
                    RegisterTableAsDict<uint, DataWeapon>(url, dataType, "WeaponId");
                    break;

                case DataType.data_stage_level:
                    RegisterTableAsDict<uint, DataStageLevel>(url, dataType, "StageLevelId");
                    break;

                case DataType.data_stage_level_flow:
                    RegisterTableAsDictWithMapping<uint, DataStageLevelFlow, MappingDataStageLevelFlow>(url, dataType, "StageLevelId");
                    break;

                case DataType.data_roguelike_save_deck:
                    RegisterTableAsDictWithMapping<uint, DataRoguelikeSaveDeck, MappingDataRoguelikeSaveDeck>(url, dataType, "RoguelikeSaveDeckId");
                    break;

                case DataType.data_treasure:
                    RegisterTableAsDict<uint, DataTreasure>(url, dataType, "TreasureId");
                    break;

                case DataType.data_proposal_treasure:
                    RegisterTableAsDictWithMapping<uint, DataProposalTreasure, MappingDataProposalTreasure>(url, dataType, "ProposalTreasureId");
                    break;

                case DataType.data_default_deck_set:
                    RegisterTableAsDictWithMapping<ModeType, DataDefaultDeckSet, MappingDataDefaultDeckSet>(url, DataType.data_default_deck_set, "ModeType");
                    break;

                case DataType.data_user_rank:
                    RegisterTableAsDict<uint, DataUserRank>(url, dataType, "UserRankLevelId");
                    break;

                case DataType.data_skill_condition:
                    RegisterTableAsDictWithMapping<uint, DataSkillCondition, MappingDataSkillCondition>(url, dataType, "SkillConditionId");
                    break;

                case DataType.data_skin_group:
                    RegisterTableAsDictWithMapping<uint, DataSkinGroup, MappingDataSkinGroup>(url, dataType, "SkinGroupId");
                    break;

                case DataType.data_skin:
                    RegisterTableAsDict<uint, DataSkin>(url, dataType, "SkinId");
                    break;

                case DataType.data_user_level:
                    RegisterTableAsDict<uint, DataUserLevel>(url, dataType, "UserLevelId");
                    break;
                case DataType.data_background:
                    RegisterTableAsDictList<uint, DataBackground>(url, dataType, "BackgroundId");
                    break;
                case DataType.data_emoticon:
                    RegisterTableAsDict<uint, DataEmoticon>(url, dataType, "EmoticonId");
                    break;
                case DataType.data_profile_icon:
                    RegisterTableAsDict<uint, DataProfileIcon>(url, dataType, "ProfileIconId");
                    break;
                case DataType.data_quest:
                    RegisterTableAsDictListWithMapping<uint, DataQuest, MappingDataQuest>(url, dataType, "QuestGroupId");
                    break;

                case DataType.data_user_init_datas:
                    RegisterTableAsDict<uint, DataUserInitData>(url, dataType, "Version");
                    break;

                case DataType.data_square_object:
                    RegisterTableAsDictWithMapping<uint, DataSquareObject, MappingDataSquareObject>(url, dataType, "SquareObjectLevel");
                    break;
                case DataType.data_square_object_planet_core:
                    RegisterTableAsDict<uint, DataSquareObjectPlanetCore>(url, dataType, "PlanetCoreLevel");
                    break;
                case DataType.data_square_object_planet_box:
                    RegisterTableAsDictList<uint, DataSquareObjectPlanetBox>(url, dataType, "SquareObjectLevel");
                    RegisterTableAsDict<uint, uint, DataSquareObjectPlanetBox>(url, DataType.data_square_object_planet_box_with_boxlevel, "SquareObjectLevel", "PlanetBoxLevel");
                    break;
                case DataType.data_square_object_planet_agency:
                    RegisterTableAsDict<uint, DataSquareObjectPlanetAgency>(url, dataType, "PlanetAgencyLevel");
                    break;
                case DataType.data_square_object_monster:
                    RegisterTableAsDict<uint, DataSquareObjectMonster>(url, dataType, "SquareObjectMonsterId");
                    break;
                case DataType.data_square_object_monster_invasion_time:
                    RegisterTableAsDict<uint, DataSquareObjectMonsterInvasionTime>(url, dataType, "MonsterInvasionLevel");
                    break;
                case DataType.data_season_reward:
                    RegisterTableAsDict<uint, DataSeasonReward>(url, dataType, "UserRankLevelId");
                    break;
                case DataType.data_slang:
                    RegisterTable<DataSlang>(url, dataType);
                    break;
                case DataType.data_user_product_digital:
                    RegisterTableAsDict<uint, DataUserProductDigital>(url, dataType, "ProductId");
                    break;
                case DataType.data_user_product_real:
                    RegisterTableAsDict<uint, DataUserProductReal>(url, dataType, "ProductId");
                    break;
                case DataType.data_material:
                    RegisterTableAsDict<MaterialType, DataMaterial>(url, dataType, "MaterialType");
                    RegisterTable<DataMaterial>(url, DataType.data_material_all);
                    break;
                case DataType.data_season_pass:
                    RegisterTableAsDict<uint, DataSeasonPass>(url, dataType, "SeasonPassId");
                    break;
                case DataType.data_underdog:
                    RegisterTableAsDict<int, DataUnderdog>(url, dataType, "MyRankInterval");
                    break;

                case DataType.data_mail:
                    RegisterTableAsDict<uint, DataMail>(url, dataType, "MailId");
                    break;
                case DataType.data_spend_materials:
                    RegisterTableAsDictList<BehaviorType, DataSpendMaterial>(url, dataType, "BehaviorType");
                    break;
                case DataType.data_state:
                    RegisterTableAsDict<SkillEffectType, DataState>(url, dataType, "SkillEffectType");
                    break;
                case DataType.data_friend_event:
                    RegisterTableAsDict<uint, DataFriendEvent>(url, dataType, "RewardNum");
                    break;
                case DataType.data_challenge:
                    RegisterTableAsDictWithMapping<uint, int, DataChallenge, MappingDataChallenge>(url, dataType, "Season", "Day");
                    break;
                case DataType.data_challenge_event:
                    RegisterTableAsDictWithMapping<uint, DataChallengeEvent, MappingDataChallengeEvent>(url, dataType, "ChallengeEventNum");
                    break;
                case DataType.data_challenge_affix:
                    RegisterTableAsDictWithMapping<uint, DataChallengeAffix, MappingDataChallengeAffix>(url, dataType, "Season");
                    break;
                case DataType.data_affix:
                    RegisterTableAsDict<uint, DataAffix>(url, dataType, "AffixId");
                    break;
                default:
                    throw new Exception($"You need to check DataType:{dataType}");
            }
        }

        

        private void RegisterTableAsJson<T>(string url, DataType dataType)
        {
            var jsonData = DownloadJson(url);
            var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonData);

            DataGetter.AddGameData(dataType, jsonObj);
        }

        private void RegisterTableByInjectionOfOtherTable<TInjectionKey1, TInjectionKey2, TKey, TValue>(string url, DataType dataType, DataType InjectionDataType, string tableColumnKeyName)
            where TInjectionKey1 : IComparable
            where TInjectionKey2 : IComparable
            where TKey : IComparable
        {
            var mapData = DataGetter.GetPrimitiveDictWithTwoKey<TInjectionKey1, TInjectionKey2, TValue>(InjectionDataType);
            AddTableAsDict<TKey, TValue>(mapData.Values, dataType, tableColumnKeyName);
        }

        private void RegisterTableByInjectionOfOtherTable<TInjectionKey1, TInjectionKey2, TKey1, TKey2, TValue>(string url, DataType dataType, DataType InjectionDataType, string tableColumnKeyName)
            where TInjectionKey1 : IComparable
            where TInjectionKey2 : IComparable
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            var mapData = DataGetter.GetPrimitiveDictWithTwoKey<TInjectionKey1, TInjectionKey2, TValue>(InjectionDataType);
            AddTableAsDict<TKey1, TKey2, TValue>(mapData.Values, dataType, tableColumnKeyName);
        }

        private void RegisterTableByInjectionOfOtherTable<TInjectionKey, TKey, TValue>(string url, DataType dataType, DataType InjectionDataType, string tableColumnKeyName)
            where TInjectionKey : IComparable
            where TKey : IComparable
        {
            var mapData = DataGetter.GetPrimitiveDict<TInjectionKey, TValue>(InjectionDataType);

            AddTableAsDict<TKey, TValue>(mapData.Values, dataType, tableColumnKeyName);
        }

        private void RegisterTableAsDictWithMapping<TKey, TValue, TMap>(string url, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
            where TMap : ClassMap
        {

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDict<TKey, TValue>(datas, dataType, tableColumnKeyName);
        }

        private void RegisterTableAsDictWithMapping<TKey1, TKey2, TValue, TMap>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TMap : ClassMap
        {
            if (tableColumnKeyNames.Length != 2)
            {
                throw new Exception("tableColumKeyNames' Count muse be equal to 2");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDict<TKey1, TKey2, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private void RegisterTableAsDictWithMapping<TKey1, TKey2, TKey3, TValue, TMap>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
            where TMap : ClassMap
        {
            if (tableColumnKeyNames.Length != 3)
            {
                throw new Exception("tableColumKeyNames' Count muse be equal to 3");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDict<TKey1, TKey2, TKey3, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private void RegisterTableAsDictListWithMapping<TKey, TValue, TMap>(string url, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
            where TMap : ClassMap
        {

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDictList<TKey, TValue>(datas, dataType, tableColumnKeyName);
        }

        private void RegisterTableAsDictListWithMapping<TKey1, TKey2, TValue, TMap>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TMap : ClassMap
        {
            if (tableColumnKeyNames.Length != 2)
            {
                throw new Exception("tableColumKeyNames's count must be equal to 2");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDictList<TKey1, TKey2, TValue>(datas, dataType, tableColumnKeyNames);
        }


        private void RegisterTableAsDictListWithMapping<TKey1, TKey2, TKey3, TValue, TMap>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
            where TMap : ClassMap
        {
            if (tableColumnKeyNames.Length != 3)
            {
                throw new Exception("tableColumKeyNames's count must be equal to 3");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToListWithMapping<TValue, TMap>();

            AddTableAsDictList<TKey1, TKey2, TKey3, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private void RegisterTable<TValue>(string url, DataType dataType)
        {
            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();
            DataGetter.AddGameData(dataType, datas);
        }

        private void RegisterTableAsDict<TKey, TValue>(string url, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
        {

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDict<TKey, TValue>(datas, dataType, tableColumnKeyName);
        }

        private void RegisterTableAsDict<TKey1, TKey2, TValue>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            if (tableColumnKeyNames.Length != 2)
            {
                throw new Exception("tableColumnKeyNames must be equal to 2");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDict<TKey1, TKey2, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private void RegisterTableAsDict<TKey1, TKey2, TKey3, TValue>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {
            if (tableColumnKeyNames.Length != 3)
            {
                throw new Exception("tableColumnKeyNames must be equal to 3");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDict<TKey1, TKey2, TKey3, TValue>(datas, dataType, tableColumnKeyNames);
        }


        private void RegisterTableAsDictList<TKey, TValue>(string url, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
        {
            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();


            AddTableAsDictList<TKey, TValue>(datas, dataType, tableColumnKeyName);
        }

        private void RegisterTableAsDictList<TKey1, TKey2, TValue>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            if (tableColumnKeyNames.Length != 2)
            {
                throw new Exception("tableColumKeyNames's count must be equal to 2");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDictList<TKey1, TKey2, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private void RegisterTableAsDictList<TKey1, TKey2, TKey3, TValue>(string url, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {
            if (tableColumnKeyNames.Length != 3)
            {
                throw new Exception("tableColumKeyNames' Count must be equal to 3");
            }

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDictList<TKey1, TKey2, TKey3, TValue>(datas, dataType, tableColumnKeyNames);
        }

        private byte[] DownloadCsv(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var csvBytes = webClient.DownloadData(url);
                return csvBytes;
            }
        }

        private string DownloadJson(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var jsonData = webClient.DownloadString(url);

                return jsonData;
            }
        }


        private void AddTableAsDictList<TKey, TValue>(IEnumerable<TValue> datas, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey, List<TValue>>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey, TValue>(data, tableColumnKeyName);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, new List<TValue>());
                }

                storeMap[realKey].Add(data);
            }

            DataGetter.AddGameData(dataType, storeMap);
        }

        private void AddTableAsDictList<TKey1, TKey2, TValue>(IEnumerable<TValue> datas, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey1, TKey2, List<TValue>>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey1, TKey2, TValue>(data, tableColumnKeyNames);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, new List<TValue>());
                }

                storeMap[realKey].Add(data);
            }

            DataGetter.AddGameData(dataType, storeMap);
        }

        private void AddTableAsDictList<TKey1, TKey2, TKey3, TValue>(IEnumerable<TValue> datas, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey1, TKey2, TKey3, List<TValue>>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey1, TKey2, TKey3, TValue>(data, tableColumnKeyNames);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, new List<TValue>());
                }

                storeMap[realKey].Add(data);
            }

            DataGetter.AddGameData(dataType, storeMap);
        }


        private void AddTableAsDict<TKey, TValue>(IEnumerable<TValue> datas, DataType dataType, string tableColumnKeyName)
            where TKey : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey, TValue>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey, TValue>(data, tableColumnKeyName);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, data);
                }
            }

            DataGetter.AddGameData(dataType, storeMap);
        }

        private void AddTableAsDict<TKey1, TKey2, TValue>(IEnumerable<TValue> datas, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey1, TKey2, TValue>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey1, TKey2, TValue>(data, tableColumnKeyNames);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, data);
                }
            }

            DataGetter.AddGameData(dataType, storeMap);
        }

        private void AddTableAsDict<TKey1, TKey2, TKey3, TValue>(IEnumerable<TValue> datas, DataType dataType, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey1, TKey2, TKey3, TValue>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey1, TKey2, TKey3, TValue>(data, tableColumnKeyNames);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, data);
                }
            }

            DataGetter.AddGameData(dataType, storeMap);
        }


        private SortedDictionary<TKey, TValue> MakeStoreDataMap<TKey, TValue>()
            where TKey : IComparable
        {

            var storeMap = new SortedDictionary<TKey, TValue>(new CustomDataComparerWithSameKey<TKey>());

            return storeMap;
        }


        private SortedDictionary<TableKey<TKey1, TKey2>, TValue> MakeStoreDataMap<TKey1, TKey2, TValue>()
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            var storeMap = new SortedDictionary<TableKey<TKey1, TKey2>, TValue>(new CustomDataComparerWithDifferentKey<TKey1, TKey2>());

            return storeMap;
        }


        private SortedDictionary<TableKey<TKey1, TKey2, TKey3>, TValue> MakeStoreDataMap<TKey1, TKey2, TKey3, TValue>()
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {
            var storeMap = new SortedDictionary<TableKey<TKey1, TKey2, TKey3>, TValue>(new CustomDataComparerWithDifferentKey<TKey1, TKey2, TKey3>());

            return storeMap;
        }


        private TKey MakeTableKey<TKey, TValue>(TValue data, string tableColumnKeyName)
            where TKey : IComparable
        {

            var key = (TKey)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyName);


            return key;
        }

        private TableKey<TKey1, TKey2> MakeTableKey<TKey1, TKey2, TValue>(TValue data, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            var key1 = (TKey1)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyNames[0]);
            var key2 = (TKey2)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyNames[1]);

            var realKey = new TableKey<TKey1, TKey2>(key1, key2);

            return realKey;
        }

        private TableKey<TKey1, TKey2, TKey3> MakeTableKey<TKey1, TKey2, TKey3, TValue>(TValue data, params string[] tableColumnKeyNames)
            where TKey1 : IComparable
            where TKey2 : IComparable
            where TKey3 : IComparable
        {
            var key1 = (TKey1)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyNames[0]);
            var key2 = (TKey2)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyNames[1]);
            var key3 = (TKey3)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyNames[2]);

            var realKey = new TableKey<TKey1, TKey2, TKey3>(key1, key2, key3);

            return realKey;
        }

        private object GetKeyUsingPropertyName<T>(T src, string keyName)
        {
            var property = src.GetType().GetProperty(keyName);
            var key = property.GetValue(src, null);

            return key;
        }
    }

    public class TableKey<T1, T2>
        where T1 : IComparable
        where T2 : IComparable
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public TableKey(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TableKey<T1, T2>;

            return other.Item1.Equals(this.Item1) && other.Item2.Equals(this.Item2);
        }
    }

    public class TableKey<T1, T2, T3>
        where T1 : IComparable
        where T2 : IComparable
        where T3 : IComparable
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public TableKey(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TableKey<T1, T2, T3>;

            return other.Item1.Equals(this.Item1) && other.Item2.Equals(this.Item2) && other.Item3.Equals(this.Item3);
        }
    }

    public class CustomDataComparerWithDifferentKey<TKey1, TKey2> : IComparer<TableKey<TKey1, TKey2>>
        where TKey1 : IComparable
        where TKey2 : IComparable
    {
        public int Compare(TableKey<TKey1, TKey2> x, TableKey<TKey1, TKey2> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
            }

            return 0;
        }
    }

    public class CustomDataComparerWithDifferentKey<TKey1, TKey2, TKey3> : IComparer<TableKey<TKey1, TKey2, TKey3>>
        where TKey1 : IComparable
        where TKey2 : IComparable
        where TKey3 : IComparable
    {
        public int Compare(TableKey<TKey1, TKey2, TKey3> x, TableKey<TKey1, TKey2, TKey3> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
                else if (result2 == 0)
                {
                    var result3 = x.Item3.CompareTo(y.Item3);
                    if (result3 > 0)
                    {
                        return 1;
                    }
                    else if (result3 < 0)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }
    }

    public class CustomDataComparerWithSameKey<KeyType> : IComparer<TableKey<KeyType, KeyType>>, IComparer<TableKey<KeyType, KeyType, KeyType>>, IComparer<KeyType>
        where KeyType : IComparable
    {
        public int Compare(TableKey<KeyType, KeyType, KeyType> x, TableKey<KeyType, KeyType, KeyType> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
                else if (result2 == 0)
                {
                    var result3 = x.Item3.CompareTo(y.Item3);
                    if (result3 > 0)
                    {
                        return 1;
                    }
                    else if (result3 < 0)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        public int Compare(TableKey<KeyType, KeyType> x, TableKey<KeyType, KeyType> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
            }

            return 0;
        }

        public int Compare(KeyType x, KeyType y)
        {
            var result = x.CompareTo(y);
            if (result > 0)
            {
                return 1;
            }
            else if (result < 0)
            {
                return -1;
            }

            return 0;
        }
    }
}
