using System.Collections.Generic;
using AkaEnum;
using System.Linq;

namespace AkaData
{
    public class AdditionalDataMaker
    {
        public void AllMake()
        {
            DataGetter.AddGameData(DataType.skill, GetSkill());
            DataGetter.AddGameData(DataType.unit_stat, GetUnitStat());
            DataGetter.AddGameData(DataType.data_unit_by_reward_probability_type, GetUnitsByRewardProbabilityType());
            DataGetter.AddGameData(DataType.data_unit_by_first_type, GetUnitsByFirstType());
            DataGetter.AddGameData(DataType.additional_unit_info, GetUnits());
            DataGetter.AddGameData(DataType.additional_card_info, GetCards());
            DataGetter.AddGameData(DataType.additional_weapon_info, GetWeapons());
            DataGetter.AddGameData(DataType.data_weapon_ids, GetAllWeaponIds());
            var cards = GetCardByUnitId();
            DataGetter.AddGameData(DataType.data_card_level_1, cards.level1Cards);
            DataGetter.AddGameData(DataType.data_card_by_unit, cards.allCards);
            DataGetter.AddGameData(DataType.data_card_unlock_type_normal, GetUnlockTypeNormalCard());
            DataGetter.AddGameData(DataType.data_user_init_datas_all, GetAllUserInitDatas());
            DataGetter.AddGameData(DataType.data_quest_processtype, GetQuestsForProcessType());
            DataGetter.AddGameData(DataType.data_square_object_monster_invasion_time_all, GetAllSquareObjectMonsterInvasionTimes());
            DataGetter.AddGameData(DataType.data_profile_icon_by_unit, GetProfileIconByUnitUnlock());
            DataGetter.AddGameData(DataType.data_unlock_emoticon_by_unitId, GetUnlockEmoticonByUnitId());
            DataGetter.AddGameData(DataType.data_season_pass_list, GetSeasonPassList());

        }

        public Dictionary<uint, SkillWithoutAnimationData> GetSkill()
        {
            Dictionary<uint, SkillWithoutAnimationData> datas = new Dictionary<uint, SkillWithoutAnimationData>();

            var dicDataSkills = Data.GetDataSkill();

            foreach (var dataSkill in dicDataSkills)
            {
                var skillOptions = new List<DataSkillOption>();
                foreach (var skillOptionId in dataSkill.Value.SkillOptionList)
                {
                    skillOptions.Add(Data.GetSkillOption(skillOptionId));
                }

                datas.Add(dataSkill.Key, new SkillWithoutAnimationData
                {
                    SkillId = dataSkill.Value.SkillId,
                    SkillType = dataSkill.Value.SkillType,
                    SkillOptions = skillOptions,
                    AnimationType = dataSkill.Value.AnimationType
                });
            }
            return datas;
        }

        public Dictionary<TableKey<uint, uint>, UnitStat> GetUnitStat()
        {
            Dictionary<TableKey<uint, uint>, UnitStat> datas = new Dictionary<TableKey<uint, uint>, UnitStat>();

            var dicDataUnitStats = Data.GetDataUnitStat();
            foreach (var dataUnitStat in dicDataUnitStats)
            {
                datas.Add(new TableKey<uint, uint>(dataUnitStat.Value.UnitId, dataUnitStat.Value.Level), new UnitStat
                {
                    Aggro = dataUnitStat.Value.Aggro,
                    Atk = dataUnitStat.Value.Atk,
                    AtkSpd = dataUnitStat.Value.AtkSpd,
                    Hp = dataUnitStat.Value.Hp,
                    Level = dataUnitStat.Value.Level,
                    NeedGoldForNextLevelUp = dataUnitStat.Value.NeedGoldForNextLevelUp,
                    UnitId = dataUnitStat.Value.UnitId,
                    RequirePieceCountForNextLevelUp = dataUnitStat.Value.RequirePieceCountForNextLevelUp,
                    CriRate = dataUnitStat.Value.CriRate,
                    CriDmgRate = dataUnitStat.Value.CriDmgRate,
                    PassiveConditionId = dataUnitStat.Value.PassiveConditionId
                });
            }
            return datas;
        }

        public List<uint> GetUnitsByRewardProbabilityType()
        {
            return Data.GetPrimitiveDict<uint, DataUnit>(DataType.data_unit).Values
                .Where(data => data.UserType == UserType.User && data.GetUnitType == GetUnitType.RewardProbability)
                .Select(data => data.UnitId).ToList();
        }

        public List<uint> GetUnitsByFirstType()
        {
            return Data.GetPrimitiveDict<uint, DataUnit>(DataType.data_unit).Values
                .Where(data => data.UserType == UserType.User && data.GetUnitType == GetUnitType.IsFirst)
                .Select(data => data.UnitId).ToList();
        }

        public List<uint> GetAllWeaponIds()
        {
            return Data.GetPrimitiveDict<uint, DataWeapon>(DataType.data_weapon).Values
                .Select(data => data.WeaponId).ToList();
        }

        public Dictionary<uint, UnitAdditionalData> GetUnits()
        {
            var additinalData = new Dictionary<uint, UnitAdditionalData>();
            var maxUnitsStat = Data.GetPrimitiveDict<uint, DataUnitStat>(DataType.data_unit_stat_all)
                .Values.Where(data => data.RequirePieceCountForNextLevelUp == 0);

            foreach (var unit in maxUnitsStat)
            {
                additinalData.Add(unit.UnitId, new UnitAdditionalData
                {
                    Id = unit.UnitId,
                    MaxLevel = unit.Level
                });
            }
            return additinalData;
        }

        public Dictionary<uint, CardAdditionalData> GetCards()
        {
            var additinalData = new Dictionary<uint, CardAdditionalData>();
            var maxCardsStat = Data.GetPrimitiveDict<uint, DataCardStat>(DataType.data_card_stat_all)
                .Values.Where(data => data.RequirePieceCountForNextLevelUp == 0);

            foreach (var card in maxCardsStat)
            {
                additinalData.Add(card.CardId, new CardAdditionalData
                {
                    Id = card.CardId,
                    MaxLevel = card.Level
                });
            }
            return additinalData;
        }

        public Dictionary<uint, WeaponAdditionalData> GetWeapons()
        {
            var additinalData = new Dictionary<uint, WeaponAdditionalData>();
            var maxWeaponsStat = Data.GetPrimitiveDict<uint, DataWeaponStat>(DataType.data_weapon_stat_all)
                .Values.Where(data => data.RequirePieceCountForNextLevelUp == 0);

            foreach (var weapon in maxWeaponsStat)
            {
                additinalData.Add(weapon.WeaponId, new WeaponAdditionalData
                {
                    Id = weapon.WeaponId,
                    MaxLevel = weapon.Level
                });
            }
            return additinalData;
        }

        public List<DataUserInitData> GetAllUserInitDatas()
        {
            return Data.GetPrimitiveDict<uint, DataUserInitData>(DataType.data_user_init_datas).Values
                        .OrderBy(datas => datas.Version)
                        .ToList();
        }

        public Dictionary<uint, List<uint>> GetLevel1Card()
        {
            var additinalData = new Dictionary<uint, List<uint>>();
            var cards = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values.Where(
                data => data.UseLevel == 1 &&
                data.UnlockType == UnlockType.Level
                );

            foreach (var card in cards)
            {
                if (additinalData.ContainsKey(card.UnitId))
                {
                    additinalData[card.UnitId].Add(card.CardId);
                }
                else
                {
                    additinalData.Add(card.UnitId, new List<uint>
                    {
                        card.CardId
                    });
                }
            }
            return additinalData;
        }

        public (Dictionary<uint, List<uint>> level1Cards, Dictionary<uint, List<uint>> allCards) GetCardByUnitId()
        {
            var level1Cards = new Dictionary<uint, List<uint>>();
            var allCards = new Dictionary<uint, List<uint>>();
            var cards = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values;

            foreach (var card in cards)
            {
                if (allCards.ContainsKey(card.UnitId))
                {
                    allCards[card.UnitId].Add(card.CardId);
                }
                else
                {
                    allCards.Add(card.UnitId, new List<uint>
                    {
                        card.CardId
                    });
                }

                if (card.UseLevel == 1 && card.UnlockType == UnlockType.Level)
                {
                    if (level1Cards.ContainsKey(card.UnitId))
                    {
                        level1Cards[card.UnitId].Add(card.CardId);
                    }
                    else
                    {
                        level1Cards.Add(card.UnitId, new List<uint>
                        {
                            card.CardId
                        });
                    }
                }
            }
            return (level1Cards, allCards);
        }

        public Dictionary<uint, List<CardUseLevelInfo>> GetUnlockTypeNormalCard()
        {
            var additinalData = new Dictionary<uint, List<CardUseLevelInfo>>();
            var cards = Data.GetPrimitiveDict<uint, DataCard>(DataType.data_card).Values;

            foreach (var card in cards)
            {
                if (additinalData.ContainsKey(card.UnitId) == false)
                    additinalData.Add(card.UnitId, new List<CardUseLevelInfo>());
                if (card.UnlockType == UnlockType.Normal)
                {
                    additinalData[card.UnitId].Add(new CardUseLevelInfo
                    {
                        Id = card.CardId,
                        UseLevel = card.UseLevel
                    });
                }
            }
            return additinalData;
        }

        public Dictionary<QuestProcessType, List<uint>> GetQuestsForProcessType()
        {
            return Data.GetAllQuests().GroupBy(keyPair => keyPair.Value.First().QuestProcessType)
                               .ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Select(questGroup => questGroup.Key).ToList());
        }

        private List<DataSquareObjectMonsterInvasionTime> GetAllSquareObjectMonsterInvasionTimes()
        {
            return Data.GetPrimitiveDict<uint, DataSquareObjectMonsterInvasionTime>(DataType.data_square_object_monster_invasion_time).Values
                        .OrderBy(datas => datas.MonsterInvasionLevel)
                        .ToList();
        }

        private Dictionary<uint, uint> GetProfileIconByUnitUnlock()
        {
            var additinalData = new Dictionary<uint, uint>();
            var profileIconsByUnits = Data.GetPrimitiveDict<uint, DataProfileIcon>(DataType.data_profile_icon).Values.Where(
                data => data.ProfileIconConditionType == ProfileIconConditionType.UnitUnlock &&
                data.ProfileIconConditionValue > 0);

            foreach (var item in profileIconsByUnits)
            {
                additinalData.Add(item.ProfileIconConditionValue, item.ProfileIconId);
            }
            return additinalData;
        }

        private Dictionary<uint, uint> GetUnlockEmoticonByUnitId()
        {
            var additinalData = new Dictionary<uint, uint>();
            var emoticons = Data.GetPrimitiveDict<uint, DataEmoticon>(DataType.data_emoticon).Values.Where(
                data => data.IsFirstEmoticon == true);

            foreach (var emoticon in emoticons)
            {
                additinalData.Add(emoticon.UnitId, emoticon.EmoticonId);
            }
            return additinalData;
        }

        private Dictionary<uint, List<DataSeasonPass>> GetSeasonPassList()
        {
            return Data.GetPrimitiveDict<uint, DataSeasonPass>(DataType.data_season_pass).Values
                       .GroupBy(data => data.Season)
                       .ToDictionary(groupedData => groupedData.Key, groupedData => groupedData.ToList());
        }
    }
}
