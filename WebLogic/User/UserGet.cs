using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common;
using Common.Entities.Season;
using Common.Entities.SquareObject;
using Common.Quest;
using Common.UserInfo;
using CommonProtocol;
using CommonProtocol.Login;

namespace WebLogic.User
{
    public class UserGet
    {
        private DBContext _db;
        private uint _userId;
        private ProtoOnLogin _protoOnLogin;
        private uint _challengeCurrentSeason;

        public UserGet(uint userId, ProtoOnLogin protoOnLogin, DBContext db, uint challengeCurrentSeason)
        {
            _userId = userId;
            _db = db;
            _protoOnLogin = protoOnLogin;
            _challengeCurrentSeason = challengeCurrentSeason;
        }

        public async Task SetUp()
        {
            _protoOnLogin.UserId = _userId;

            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(new InputArg("$userId", _userId),
                new InputArg("$season", _challengeCurrentSeason));
            using (var cursor = await _db.CallStoredProcedureAsync(StoredProcedure.GET_LOGIN_INFO, paramInfo))
            {
                // The calling sequence should not be changed.
                GetUserInfo(cursor);
                GetTermMaterialInfo(cursor);
                GetUnitInfo(cursor);
                GetSkillInfo(cursor);
                GetWeaponInfo(cursor);
                //GetStageInfo(cursor);
                GetInfusionBox(cursor);
                GetSkinInfo(cursor);
                GetEmoticons(cursor);
                GetUserProfiles(cursor);
                GetQuests(cursor);
                GetSquareObject(cursor);
                GetUserAdditionalInfo(cursor);
                GetPushAgreeInfo(cursor);
                GetChallengeStageList(cursor);
            }

            //await GetInprogressStageInfo();
        }

        private void GetUserInfo(DbDataReader cursor)
        {
            if (cursor.Read())
            {
                _protoOnLogin.UserInfo.MaterialInfoList.Add(new ProtoMaterialInfo
                { MaterialType = MaterialType.Gold, Count = (int)cursor[ColumnName.USER_GOLD] });

                _protoOnLogin.UserInfo.MaterialInfoList.Add(new ProtoMaterialInfo
                { MaterialType = MaterialType.Gem, Count = (int)cursor[ColumnName.USER_GEM] + (int)cursor[ColumnName.USER_GEM_PAID] });

                _protoOnLogin.UserInfo.MaterialInfoList.Add(new ProtoMaterialInfo
                { MaterialType = MaterialType.StarCoin, Count = (int)cursor[ColumnName.STAR_COIN] });

                _protoOnLogin.UserInfo.MaterialInfoList.Add(new ProtoMaterialInfo
                { MaterialType = MaterialType.SquareObjectStartTicket, Count = (int)cursor[ColumnName.SQUARE_OBJECT_START_TICKET] });

                _protoOnLogin.UserInfo.MaterialInfoList.Add(new ProtoMaterialInfo
                { MaterialType = MaterialType.ChallengeCoin, Count = (int)cursor[ColumnName.CHALLENGE_COIN] });

                _protoOnLogin.UserInfo.LevelAndExp.Level = (uint)cursor["level"];

                _protoOnLogin.UserInfo.LevelAndExp.Exp = (ulong)cursor["exp"];

            }
        }

        private void GetTermMaterialInfo(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.UserInfo.TermMaterialInfoList.Add(new ProtoTermMaterialInfo
                    {
                        TermMaterialType = (TermMaterialType)(int)cursor["termMaterialType"],
                        Count = (int)cursor[ColumnName.EVENT_COIN],
                        RecentUpdateDateTime = ((DateTime)cursor["recentUpdateDateTime"]).Ticks
                    });
                }
            }
        }

        private void GetUnitInfo(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.UnitInfoList.Add(
                        new ProtoUnitInfo
                        {
                            Id = (uint)cursor["id"],
                            Level = (uint)cursor["level"],
                            Count = (int)cursor["count"],
                            MaxRankLevel = (uint)cursor["maxRankLevel"],
                            CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                            NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                            SkinId = (uint)cursor["skinId"],
                            MaxVirtualRankLevel = (uint)cursor["maxVirtualRankLevel"],
                            VirtualCurrentRankLevel = (uint)cursor["currentVirtualRankLevel"],
                            VirtualRankPoint = (int)cursor["currentVirtualRankPoint"],
                        }
                    );
                }
            }
        }

        private void GetSkillInfo(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.CardInfoList.Add(
                        new ProtoCardInfo
                        {
                            Id = (uint)cursor["id"],
                            Level = (uint)cursor["level"],
                            Count = (int)cursor["count"]
                        }
                    );
                }
            }
        }

        private void GetWeaponInfo(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.WeaponInfoList.Add(
                        new ProtoWeaponInfo
                        {
                            Id = (uint)cursor["id"],
                            Level = (uint)cursor["level"],
                            Count = (int)cursor["count"]
                        }
                    );
                }
            }
        }

        private void GetInfusionBox(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.InfusionBoxList.Add(
                        new ProtoInfusionBox
                        {
                            Id = (uint)cursor["id"],
                            BoxEnergy = (int)cursor["boxEnergy"],
                            UserEnergy = (int)cursor["userEnergy"],
                            UserBonusEnergy = (int)cursor["userBonusEnergy"],
                            UserEnergyRecentUpdateDatetime = ((DateTime)cursor["userEnergyRecentUpdateDatetime"]).Ticks
                        }
                    );
                }
            }
        }

        private void GetSkinInfo(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.SkinList.Add((uint)cursor["skinId"]);
                }
            }
        }

        private void GetEmoticons(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.EmoticonList.Add(
                        new ProtoEmoticonInfo
                        {
                            UnitId = (uint)cursor["unitId"],
                            EmoticonId = (uint)cursor["Id"],
                            OrderNum = (int)cursor["orderNum"]
                        }
                    );
                }
            }
        }

        private void GetUserProfiles(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.UserProfileList.Add(
                        new ProtoUserProfileInfo
                        {
                            UserProfileId = (uint)cursor["id"]
                        }
                    );
                }
            }
        }

        private void GetQuests(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                _protoOnLogin.QuestList.AddRange(new QuestIO(_userId, _protoOnLogin.CurrentSeasonPass).Select(cursor));

                //       Console.WriteLine($"Quests {string.Join(",",_protoOnLogin.QuestList.Where(quest => AkaData.Data.GetQuest(quest.QuestGroupId).First().QuestType == QuestType.SeasonPass).Select(quest => quest.QuestGroupId))}");

            }
        }

        private void GetSquareObject(DbDataReader cursor)
        {
            if (cursor.NextResult() && cursor.Read())
            {
                _protoOnLogin.SquareObjectInfo = new SquareObjectIO().GetSquareObject(cursor);
            }
        }

        private void GetUserAdditionalInfo(DbDataReader cursor)
        {
            if (cursor.NextResult() && cursor.Read())
            {
                var dayIo = new DayIO();
                dayIo.Select(cursor, false);
                if (dayIo.DailyQuestAddcount <= 0)
                {
                    var enableDailyQuestCount = _protoOnLogin.QuestList.Count(questInfo => questInfo.DynamicQuestGroupId == 0 && Data.GetQuest(questInfo.QuestGroupId)?.FirstOrDefault()?.QuestType == QuestType.Daily);
                    if (dayIo.DailyQuestAddcount <= -enableDailyQuestCount)
                        dayIo.DailyQuestAddcount = 1 - enableDailyQuestCount;
                }

                _protoOnLogin.AddtionalUserInfo = new ProtoAdditionalUserInfo
                {
                    IsAlreadyFreeNicknameChange = (sbyte)cursor["isAlreadyFreeNicknameChange"] == 1 ? true : false,
                    RecentDateTimeCountryChange = ((DateTime)cursor["recentDateTimeCountryChange"]).Ticks,
                    DailyRankVictoryGoldRewardCount = (sbyte)cursor["dailyRankVictoryGoldRewardCount"],
                    DailyRankVictoryGoldRewardDateTime = ((DateTime)cursor["dailyRankVictoryGoldRewardDateTime"]).Ticks,
                    RewardedRankSeason = (int)cursor["rewardedRankSeason"],
                    UnlockContents = ((string)cursor["unlockContents"]).Split('/')
                        .Select(contentsTypeId =>
                        {
                            if (Enum.TryParse<ContentsType>(contentsTypeId, out var contentsType))
                            {
                                return contentsType;
                            }
                            return ContentsType.None;
                        })
                        .Where(contentsType => ContentsType.None != contentsType)
                        .ToList(),
                    MaxVirtualRankLevel = (uint)cursor["maxVirtualRankLevel"],
                    MaxVirtualRankPoint = (int)cursor["maxVirtualRankPoint"],
                    CurrentVirtualRankPoint = (int)cursor["currentVirtualRankPoint"],
                    EnablePassList = ((string)cursor["enablePassList"]).Split('/')
                        .Select(seasonPassId =>
                        {
                            if (uint.TryParse(seasonPassId, out var passId))
                            {
                                return passId;
                            }
                            return (uint)0;
                        })
                        .Where(seasonPassId => seasonPassId != 0)
                        .ToList(),
                    DailyQuestAddCount = dayIo.DailyQuestAddcount,
                    DailyQuestRefreshCount = dayIo.DailyQuestRefreshCount,
                    AddDeck = (int)cursor["addDeck"]
                };
            }
            else
            {
                _protoOnLogin.AddtionalUserInfo = new ProtoAdditionalUserInfo
                {
                    IsAlreadyFreeNicknameChange = false,
                    RecentDateTimeCountryChange = 0,
                    DailyRankVictoryGoldRewardCount = 0,
                    DailyRankVictoryGoldRewardDateTime = 0,
                    RewardedRankSeason = 0,
                    UnlockContents = new List<ContentsType>() { ContentsType.None },
                    MaxVirtualRankLevel = 1,
                    MaxVirtualRankPoint = 0,
                    CurrentVirtualRankPoint = 0,
                };
            }
        }

        private void GetPushAgreeInfo(DbDataReader cursor)
        {
            if (cursor.NextResult() && cursor.Read())
            {
                _protoOnLogin.PushAgree = (int)cursor["pushAgree"];
                _protoOnLogin.NightPushAgree = (int)cursor["nightPushAgree"];
            }
        }

        private void GetChallengeStageList(DbDataReader cursor)
        {
            if (cursor.NextResult())
            {
                while (cursor.Read())
                {
                    _protoOnLogin.ChallengeStageList.stages.Add(new ProtoChallengeStage
                    {
                        Day = (int)cursor["day"],
                        DifficultLevel = (int)cursor["difficultLevel"],
                        ClearCount = (int)cursor["clearCount"],
                        IsRewarded = Convert.ToBoolean((int)cursor["isRewarded"]),
                        RewardResetCount = (int)cursor["rewardResetCount"]
                    });
                }
            }
        }
    }
}
