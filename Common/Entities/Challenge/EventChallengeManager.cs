using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.CommonType;
using CommonProtocol;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Challenge
{
    public class EventChallengeManager : CommonChallengeManager
    {
        private EventChallengeParam _param;
        private uint _challengeEventNum;

        public EventChallengeManager(DbConnects dbCon, EventChallengeParam param, uint userId) 
            : base(TableName.CHALLENGE_EVENT_STAGE, dbCon, userId)
        {
            _param = param;
        }

        public async Task<List<ProtoItemResult>> Clear(byte deckNum)
        {
            var rewards = await GetReward();
            await ClearSetDb(rewards == null ? false : true);
            if (rewards != null)
            {
                await IncrementChallengeClearCount();
                await SaveFirstClear(deckNum);
            }
            return rewards;
        }

        private new async Task<UserDbChallengeStage> GetChallengeStageInfo()
        {
            _query.Clear();
            _query.Append("SELECT clearCount, isRewarded, rewardResetCount, inprogressRound FROM " +
                TableName.CHALLENGE_EVENT_STAGE + " WHERE userId=")
                .Append(_userId).Append(" AND challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel).Append(";");

            return await base.GetChallengeStageInfo();
        }

        protected override async Task InitChallengeStage()
        {
            _query.Clear();
            _query.Append("INSERT IGNORE INTO challenge_event_stage (userId, challengeEventId, difficultLevel, clearCount) " +
                "VALUES (").Append(_userId).Append(",").Append(_param.ChallengeEventId).Append(",")
                .Append(_param.DifficultLevel).Append(", 0);");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<List<ProtoItemResult>> GetReward()
        {
            if (false == await IsCanGetReward())
                return null;

            var stageInfo = await GetChallengeStageInfo();

            var challengeEventNum = await GetChallengeEventNum();
            if (challengeEventNum == 0)
                return null;

            var rewardId = stageInfo.RewardResetCount == 0
                ? Data.GetDataChallengeEvent(challengeEventNum).RewardIdList1[_param.DifficultLevel - 1]
                : Data.GetDataChallengeEvent(challengeEventNum).RewardIdList2[_param.DifficultLevel - 1];

            return await Reward.Reward.GetRewards(_dbCon.UserDb, _userId, rewardId, "EventChallengeReward");
        }

        private async Task<bool> IsCanGetReward()
        {
            var stageInfo = await GetChallengeStageInfo();
            return stageInfo.IsRewarded == 0;
        }

        public async Task ClearSetDb(bool isReward)
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_event_stage " +
                "(userId, challengeEventId, difficultLevel, clearCount, isRewarded, inprogressRound)" +
                " VALUES (").Append(_userId).Append(",").Append(_param.ChallengeEventId).Append(",")
                .Append(_param.DifficultLevel).Append(",1, 1, 0) " +
                "ON DUPLICATE KEY UPDATE clearCount = clearCount + 1, inprogressRound = 0");

            if (isReward)
                _query.Append(", isRewarded = isRewarded + 1;");
            else
                _query.Append(";");
            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<bool> IsInEvent(bool withId = false)
        {
            _query.Clear();

            var utcNow = DateTime.UtcNow.ToTimeString();
            if (withId)
            {
                _query.Append("SELECT startDateTime, endDateTime FROM _challenge_events " +
                "WHERE endDateTime >= '").Append(utcNow)
                .Append("' AND startDateTime <= '").Append(utcNow).Append("' AND challengeEventId=")
                .Append(_param.ChallengeEventId).Append(";");
            }
            else
            {
                _query.Append("SELECT startDateTime, endDateTime FROM _challenge_events " +
                "WHERE endDateTime >= '").Append(utcNow)
                .Append("' AND startDateTime <= '").Append(utcNow)
                .Append("';");
            }

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }

        public async Task<bool> ResetReward()
        {
            if (false == await IsCanResetReward())
                return false;

            _query.Clear();
            _query.Append("UPDATE " + _tableName + " SET isRewarded = 0, rewardResetCount = rewardResetCount + 1" +
                " WHERE userId=").Append(_userId)
                .Append(" AND challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
            return true;
        }

        private async Task<bool> IsCanResetReward()
        {
            var stageInfo = await GetChallengeStageInfo();
            return await IsCanResetReward(stageInfo);
        }

        public async Task<EventInfo> GetEventInfo()
        {
            _query.Clear();
            var utcNow = DateTime.UtcNow.ToTimeString();
            _query.Append("SELECT startDateTime, endDateTime FROM _challenge_events " +
                "WHERE endDateTime >= '").Append(utcNow)
                .Append("' AND startDateTime <= '").Append(utcNow)
                .Append("';");

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return new EventInfo { IsInEvent = true, StartDateTime = (DateTime)cursor["startDateTime"] };
                else
                    return new EventInfo { IsInEvent = false };
            }
        }

        public async Task<List<ProtoEventChallengeDate>> GetEventList()
        {
            _query.Clear();
            var utcNow = DateTime.UtcNow;
            _query.Append("SELECT challengeEventId, challengeEventNum, startDateTime, endDateTime FROM _challenge_events " +
                "WHERE endDateTime >= '").Append(utcNow.ToTimeString())
                .Append("' AND startDateTime <= '").Append(utcNow.AddHours(10).ToTimeString()).Append("';");

            List<ProtoEventChallengeDate> eventList = new List<ProtoEventChallengeDate>();
            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    eventList.Add(new ProtoEventChallengeDate
                    {
                        ChallengeEventId = (uint)cursor["challengeEventId"],
                        StartDateTime = ((DateTime)cursor["startDateTime"]).Ticks,
                        EndDateTime = ((DateTime)cursor["endDateTime"]).Ticks,
                        ChallengeEventNum = (uint)cursor["challengeEventNum"]
                    });
                }
            }
            return eventList;
        }

        public async Task<bool> IsValidFlow()
        {
            if (_param.DifficultLevel == 1)
                return true;

            return await IsBeforeStageClear();
        }

        private async Task<bool> IsBeforeStageClear()
        {
            _query.Clear();
            _query.Append("SELECT clearCount FROM challenge_event_stage WHERE userId =")
                .Append(_userId).Append(" AND challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel - 1).Append(";");

            using (var cursor = await _dbCon.UserDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                return cursor.GetInt32(0) > 0;
            }
        }

        private async Task IncrementChallengeClearCount()
        {
            var stageInfo = await GetChallengeStageInfo();
            if (stageInfo.ClearCount != 0)
                return;

            _query.Clear();
            _query.Append("INSERT INTO user_info (userId, challengeClearCount) VALUES (")
                .Append(_userId)
                .Append(", 1) ON DUPLICATE KEY UPDATE challengeClearCount = challengeClearCount + 1;");
            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task SaveFirstClear(byte deckNum)
        {
            if (false == await IsAlreadyFirstClear())
            {
                await SaveFirstClear();
                await SaveFirstClearDeck(deckNum);
            }
        }

        private async Task<bool> IsAlreadyFirstClear()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM challenge_event_stage_first_clear_user " +
                "WHERE challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel).Append(" LIMIT 1;");

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }

        private async Task SaveFirstClear()
        {
            _query.Clear();
            _query.Append("INSERT IGNORE INTO challenge_event_stage_first_clear_user " +
                "(userId, challengeEventId, difficultLevel, insertDateTime) VALUES (")
                .Append(_userId).Append(",").Append(_param.ChallengeEventId)
                .Append(",").Append(_param.DifficultLevel).Append(",'")
                .Append(DateTime.UtcNow.ToTimeString()).Append("');");

            await _dbCon.AccountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task SaveFirstClearDeck(byte deckNum)
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_first_clear_decks_event " +
                "(challengeEventId, difficultLevel, userId, modeType, slotType, deckNum, orderNum, classId) " +
                "SELECT ").Append(_param.ChallengeEventId).Append(",")
                .Append(_param.DifficultLevel)
                .Append(", userId, modeType, slotType, deckNum, orderNum, classId FROM decks " +
                "WHERE userId=").Append(_userId).Append(" AND modeType=").Append((int)ModeType.PVP)
                .Append(" AND deckNum=").Append(deckNum).Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<uint> GetChallengeEventNum()
        {
            if (_challengeEventNum != 0)
                return _challengeEventNum;

            _query.Clear();
            _query.Append("SELECT challengeEventNum FROM _challenge_events WHERE challengeEventId = ")
                .Append(_param.ChallengeEventId).Append(";");

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return (uint)cursor["challengeEventNum"];
                else
                    return _challengeEventNum;
            }
        }

        public async Task<int> GetCurrentRound()
        {
            var challengeStageInfo = await GetChallengeStageInfo();
            return challengeStageInfo.Round;
        }

        public async Task RoundClear()
        {
            _query.Clear();
            _query.Append("UPDATE challenge_event_stage SET inprogressRound = inprogressRound + 1" +
                " WHERE userId=").Append(_userId)
                .Append(" AND challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task StartSetDb()
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_event_stage " +
                "(userId, challengeEventId, difficultLevel, clearCount, inprogressRound)" +
                " VALUES (").Append(_userId).Append(",").Append(_param.ChallengeEventId).Append(",")
                .Append(_param.DifficultLevel).Append(",0, 0) " +
                "ON DUPLICATE KEY UPDATE inprogressRound = 0;");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<uint?> GetFirstClearUserId()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM challenge_event_stage_first_clear_user" +
                " WHERE challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(";");

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return null;

                return (uint)cursor["userId"];
            }
        }
        public async Task<ProtoOnGetDeck> GetFirstClearUser(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT modeType, slotType, deckNum, orderNum, classId " +
                "FROM challenge_first_clear_decks_event" +
                " WHERE challengeEventId=").Append(_param.ChallengeEventId)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(" AND userId=").Append(userId)
                .Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                var deckInfo = new ProtoOnGetDeck();
                while (cursor.Read())
                {
                    deckInfo.DeckElements.Add(new ProtoDeckElement
                    {
                        ClassId = (uint)cursor["classId"],
                        DeckNum = (byte)cursor["deckNum"],
                        ModeType = (ModeType)(byte)cursor["modeType"],
                        OrderNum = (byte)cursor["orderNum"],
                        SlotType = (SlotType)(byte)cursor["slotType"]
                    });
                }
                return deckInfo;
            }
        }

        public async Task<ProtoEventChallengeStageList> GetStageList()
        {
            _query.Clear();
            _query.Append("SELECT difficultLevel, clearCount, isRewarded, rewardResetCount" +
                " FROM challenge_event_stage WHERE userId=").Append(_userId)
                .Append(" AND challengeEventId=").Append(_param.ChallengeEventId).Append(";");

            using (var cursor = await _dbCon.UserDb.ExecuteReaderAsync(_query.ToString()))
            {
                var stages = new ProtoEventChallengeStageList();
                while (cursor.Read())
                {
                    stages.stages.Add(new ProtoEventChallengeStage
                    {
                        DifficultLevel = (int)cursor["difficultLevel"],
                        ClearCount = (int)cursor["clearCount"],
                        IsRewarded = Convert.ToBoolean((int)cursor["isRewarded"]),
                        RewardResetCount = (int)cursor["rewardResetCount"]
                    });
                }
                return stages;
            }
        }
    }
}
