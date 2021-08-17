using AkaDB.MySql;
using AkaEnum;
using System;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public abstract class UserAdditionalInfo : IUserInfoChanger
    {
        protected DBContext _accountDb;
        protected DBContext _userDb;
        protected uint _userId;
        protected AdditionalUserInfo _additionalUserInfo;
        protected UserAdditionalInfoType _userInfoType;
        protected StringBuilder _query = new StringBuilder();

        public UserAdditionalInfo(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _userInfoType = userInfoType;
        }

        public abstract Task<ResultType> Change(RequestValue requestValue);

        protected async Task<DbDataReader> GetAccountInfo()
        {
            _query.Clear();
            _query.Append("SELECT currentSeason, currentSeasonRankPoint, nextSeasonRankPoint, countryCode, nickname " +
            "FROM accounts WHERE userId = ").Append(_userId).Append(";");

            return await _accountDb.ExecuteReaderAsync(_query.ToString());
        }

        protected async Task<AdditionalUserInfo> GetAdditionalUserInfo()
        {
            if (_additionalUserInfo != null)
                return _additionalUserInfo;

            var query = new StringBuilder();
            query.Append("SELECT isAlreadyFreeNicknameChange, recentDateTimeCountryChange, " +
                "dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime, rewardedRankSeason " +
                "FROM user_info " +
                "WHERE userid = ").Append(_userId).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                {
                    _additionalUserInfo =  new AdditionalUserInfo
                    {
                        IsAlreadyFreeNicknameChange = false,
                        RecentDateTimeCountryChange = DateTime.UtcNow.AddDays(-100),
                        DailyRankVictoryGoldRewardCount = 0,
                        DailyRankVictoryGoldRewardDateTime = DateTime.UtcNow.AddDays(-100),
                        RewardedRankSeason = 0
                    };
                }
                else
                {
                    _additionalUserInfo = new AdditionalUserInfo
                    {
                        IsAlreadyFreeNicknameChange = (sbyte)cursor["isAlreadyFreeNicknameChange"] == 1 ? true : false,
                        RecentDateTimeCountryChange = (DateTime)cursor["recentDateTimeCountryChange"],
                        DailyRankVictoryGoldRewardCount = (sbyte)cursor["dailyRankVictoryGoldRewardCount"],
                        DailyRankVictoryGoldRewardDateTime = (DateTime)cursor["dailyRankVictoryGoldRewardDateTime"],
                        RewardedRankSeason = (int)cursor["rewardedRankSeason"]
                    };
                }
            }

            return _additionalUserInfo;
        }

        public class AdditionalUserInfo
        {
            public bool IsAlreadyFreeNicknameChange;
            public DateTime RecentDateTimeCountryChange;
            public sbyte DailyRankVictoryGoldRewardCount;
            public DateTime DailyRankVictoryGoldRewardDateTime;
            public int RewardedRankSeason;
        }
    }
}
