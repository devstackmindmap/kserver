using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using System;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class DailyGoldRewardByRankVictory : UserAdditionalInfo
    {
        DateTime _utcNow;
        public DailyGoldRewardByRankVictory(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType) : base(accountDb, userDb, userId, userInfoType)
        {
            _utcNow = DateTime.UtcNow;
        }

        public override async Task<ResultType> Change(RequestValue requestValue)
        {
            var baseDateTime = GetBaseDateTime();
            var addtionalUserInfo = await GetAdditionalUserInfo();

            if (baseDateTime > addtionalUserInfo.DailyRankVictoryGoldRewardDateTime)
            {
                addtionalUserInfo.DailyRankVictoryGoldRewardCount = 1;
                addtionalUserInfo.DailyRankVictoryGoldRewardDateTime = _utcNow;
            }
            else
            {
                var maxDailyRankVictoryGoldRewardCount = (int)Data.GetConstant(DataConstantType.LIMIT_RANK_REWARD_GOLD).Value;
                if (addtionalUserInfo.DailyRankVictoryGoldRewardCount < maxDailyRankVictoryGoldRewardCount)
                    addtionalUserInfo.DailyRankVictoryGoldRewardCount++;
                else
                    return ResultType.Fail;
            }

            await SetValue(addtionalUserInfo);
            return ResultType.Success;
        }

        private DateTime GetBaseDateTime()
        {
            var baseHour = (int)Data.GetConstant(DataConstantType.TIME_RESET_LIMIT_RANK_REWARD_GOLD).Value;
            var nowDateBaseHour = new DateTime(_utcNow.Year, _utcNow.Month, _utcNow.Day, baseHour, 0, 0);

            if (_utcNow.Hour < baseHour)
                return nowDateBaseHour.AddDays(-1);
            else
                return nowDateBaseHour;
        }

        private async Task SetValue(AdditionalUserInfo additionalUserInfo)
        {
            _query.Clear();
            _query.Append("INSERT INTO user_info (userId, dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime) " +
                "VALUES (").Append(_userId).Append(",").Append(additionalUserInfo.DailyRankVictoryGoldRewardCount)
                .Append(",'").Append(additionalUserInfo.DailyRankVictoryGoldRewardDateTime.ToTimeString())
                .Append("') ON DUPLICATE KEY UPDATE dailyRankVictoryGoldRewardCount = ")
                .Append(additionalUserInfo.DailyRankVictoryGoldRewardCount).Append(", dailyRankVictoryGoldRewardDateTime = '")
                .Append(additionalUserInfo.DailyRankVictoryGoldRewardDateTime.ToTimeString()).Append("';");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
