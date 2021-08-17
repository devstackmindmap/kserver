using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.CommonType;
using Common.Entities.Season;
using Common.Quest;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Challenge
{
    public class ChallengeManager : CommonChallengeManager
    {
        private ChallengeParam _param;
        private ServerSeasonInfo _seasonInfo;
        private int _currentDay = -1;

        public ChallengeManager(DbConnects dbCon, ChallengeParam param, uint userId) 
            : base(TableName.CHALLENGE_STAGE, dbCon, userId)
        {
            _param = param;
        }

        public async Task<uint> GetSeason()
        {
            var seasonInfo = await GetSeasonInfo();
            return seasonInfo.CurrentSeason;
        }

        public async Task<bool> IsValidSeason()
        {
            var seasonInfo = await GetSeasonInfo();
            return seasonInfo.CurrentSeason == _param.Season;
        }

        public async Task<bool> IsValidDay()
        {
            var seasonInfo = await GetSeasonInfo();
            return (DateTime.UtcNow - seasonInfo.CurrentSeasonStartDateTime).Days + 1 >= _param.Day;
        }

        public async Task<bool> IsBeforeStageClear()
        {
            if (_param.DifficultLevel == 1)
                return true;

            _query.Clear();
            _query.Append("SELECT clearCount FROM challenge_stage WHERE userId=")
                .Append(_userId).Append(" AND season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day).Append(" AND difficultLevel=")
                .Append(_param.DifficultLevel - 1).Append(";");

            using (var cursor = await _dbCon.UserDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return false;

                return (int)cursor["clearCount"] > 0;
            }
        }

        private async Task<ServerSeasonInfo> GetSeasonInfo()
        {
            if (_seasonInfo == null)
            {
                var serverSeason = new ServerSeason(_dbCon.AccountDb);
                _seasonInfo = await serverSeason.GetChallengeSeasonInfo();
                _currentDay = Convert.ToInt32((DateTime.UtcNow - _seasonInfo.CurrentSeasonStartDateTime).TotalDays) + 1;
            }

            return _seasonInfo;
        }

        private async Task<int> GetCurrentDay()
        {
            if (_currentDay == -1)
            {
                var seasonInfo = await GetSeasonInfo();
                _currentDay = (int)(DateTime.UtcNow - seasonInfo.CurrentSeasonStartDateTime).TotalDays + 1;
            }
            return _currentDay;
        }

        protected new async Task<UserDbChallengeStage> GetChallengeStageInfo()
        {
            _query.Clear();
            _query.Append("SELECT clearCount, isRewarded, rewardResetCount, inprogressRound " +
                "FROM " + _tableName + " WHERE userId=")
                .Append(_userId).Append(" AND season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day).Append(" AND difficultLevel=")
                .Append(_param.DifficultLevel).Append(";");

            return await base.GetChallengeStageInfo();
        }

        protected override async Task InitChallengeStage()
        {
            _query.Clear();
            _query.Append("INSERT IGNORE INTO challenge_stage (userId, season, day, difficultLevel, clearCount) " +
                "VALUES (").Append(_userId).Append(",").Append(_param.Season).Append(",")
                .Append(_param.Day).Append(",").Append(_param.DifficultLevel).Append(", 0);");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<List<ProtoItemResult>> Clear(byte deckNum)
        {
            var rewards = await GetReward();
            await ClearSetDb(rewards == null ? false : true);
            if (rewards != null && (await GetChallengeStageInfo()).RewardResetCount == 0)
            {
                await IncrementChallengeClearCount();
                await SaveFirstClear(deckNum);
            }
            return rewards;
        }

        private async Task ClearSetDb(bool isReward)
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_stage " +
                "(userId, season, `day`, difficultLevel, clearCount, isRewarded, inprogressRound)" +
                " VALUES (").Append(_userId).Append(",").Append(_param.Season).Append(",")
                .Append(_param.Day).Append(",").Append(_param.DifficultLevel).Append(",1, 1, 0) " +
                "ON DUPLICATE KEY UPDATE clearCount = clearCount + 1, inprogressRound = 0");

            if (isReward)
                _query.Append(", isRewarded = isRewarded + 1;");
            else
                _query.Append(";");
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
            _query.Append("SELECT userId FROM challenge_stage_first_clear_user " +
                "WHERE season=").Append(_param.Season).Append(" AND day=").Append(_param.Day)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel).Append(" LIMIT 1;");

            using (var cursor = await _dbCon.AccountDb.ExecuteReaderAsync(_query.ToString()))
            {
                return cursor.Read();
            }
        }

        private async Task SaveFirstClear()
        {
            _query.Clear();
            _query.Append("INSERT IGNORE INTO challenge_stage_first_clear_user " +
                "(userId, season, day, difficultLevel, insertDateTime) VALUES (")
                .Append(_userId).Append(",").Append(_param.Season).Append(",").Append(_param.Day)
                .Append(",").Append(_param.DifficultLevel).Append(",'")
                .Append(DateTime.UtcNow.ToTimeString()).Append("');");

            await _dbCon.AccountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task SaveFirstClearDeck(byte deckNum)
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_first_clear_decks " +
                "(season, `day`, difficultLevel, userId, modeType, slotType, deckNum, orderNum, classId) " +
                "SELECT ").Append(_param.Season).Append(",").Append(_param.Day).Append(",")
                .Append(_param.DifficultLevel)
                .Append(", userId, modeType, slotType, deckNum, orderNum, classId FROM decks " +
                "WHERE userId=").Append(_userId).Append(" AND modeType=").Append((int)ModeType.PVP)
                .Append(" AND deckNum=").Append(deckNum).Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<List<ProtoItemResult>> GetReward()
        {
            if (false == await IsCanGetReward())
                return null;

            var stageInfo = await GetChallengeStageInfo();
            var rewardId = stageInfo.RewardResetCount == 0
                ? Data.GetDataChallenge(_param.Season, _param.Day).RewardIdList1[_param.DifficultLevel - 1]
                : Data.GetDataChallenge(_param.Season, _param.Day).RewardIdList2[_param.DifficultLevel - 1];

            return await Reward.Reward.GetRewards(_dbCon.UserDb, _userId, rewardId, "ChallengeReward");
        }

        private async Task<bool> IsCanGetReward()
        {
            var stageInfo = await GetChallengeStageInfo();
            return stageInfo.IsRewarded == 0;
        }

        public async Task<bool> ResetReward()
        {
            if (false == await IsCanResetReward())
                return false;

            _query.Clear();
            _query.Append("UPDATE " + _tableName + " SET isRewarded = 0, rewardResetCount = rewardResetCount + 1" +
                " WHERE userId=").Append(_userId)
                .Append(" AND season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
            return true;
        }

        private async Task<bool> IsCanResetReward()
        {
            var stageInfo = await GetChallengeStageInfo();
            return await IsCanResetReward(stageInfo) && await IsValidDayCanReset();
        }

        private async Task<bool> IsValidDayCanReset()
        {
            return await GetCurrentDay() == _param.Day;
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

        public async Task<ProtoChallengeStageList> GetStageList()
        {
            _query.Clear();
            _query.Append("SELECT day, difficultLevel, clearCount, isRewarded, rewardResetCount" +
                " FROM challenge_stage WHERE userId=").Append(_userId)
                .Append(" AND season=").Append(_param.Season).Append(";");

            using (var cursor = await _dbCon.UserDb.ExecuteReaderAsync(_query.ToString()))
            {
                var stages = new ProtoChallengeStageList { CurrentSeason = (uint)_param.Season };
                while (cursor.Read())
                {
                    stages.stages.Add(new ProtoChallengeStage
                    {
                        Day = (int)cursor["day"],
                        DifficultLevel = (int)cursor["difficultLevel"],
                        ClearCount = (int)cursor["clearCount"],
                        IsRewarded = Convert.ToBoolean((int)cursor["isRewarded"]),
                        RewardResetCount = (int)cursor["rewardResetCount"]
                    });
                }
                return stages;
            }
        }

        public async Task<uint?> GetFirstClearUserId()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM challenge_stage_first_clear_user" +
                " WHERE season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day)
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
                "FROM challenge_first_clear_decks" +
                " WHERE season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day)
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

        public async Task RoundClear()
        {
            _query.Clear();
            _query.Append("UPDATE challenge_stage SET inprogressRound = inprogressRound + 1" +
                " WHERE userId=").Append(_userId)
                .Append(" AND season=").Append(_param.Season)
                .Append(" AND `day`=").Append(_param.Day)
                .Append(" AND difficultLevel=").Append(_param.DifficultLevel)
                .Append(";");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task StartSetDb()
        {
            _query.Clear();
            _query.Append("INSERT INTO challenge_stage " +
                "(userId, season, `day`, difficultLevel, clearCount, isRewarded, inprogressRound)" +
                " VALUES (").Append(_userId).Append(",").Append(_param.Season).Append(",")
                .Append(_param.Day).Append(",").Append(_param.DifficultLevel).Append(",0, 0, 0) " +
                "ON DUPLICATE KEY UPDATE inprogressRound = 0;");

            await _dbCon.UserDb.ExecuteNonQueryAsync(_query.ToString());
        }

        public async Task<int> GetCurrentRound()
        {
            var challengeStageInfo = await GetChallengeStageInfo();
            return challengeStageInfo.Round;
        }
    }
}
