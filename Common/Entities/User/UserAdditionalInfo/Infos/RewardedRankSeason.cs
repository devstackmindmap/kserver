using AkaDB.MySql;
using AkaEnum;
using System;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class RewardedRankSeason : UserAdditionalInfo
    {
        public RewardedRankSeason(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType) : base(accountDb, userDb, userId, userInfoType)
        {

        }

        public override async Task<ResultType> Change(RequestValue requestValue)
        {
            await SetValue(Int32.Parse(requestValue.StringValue));
            return ResultType.Success;
        }

        protected async Task SetValue(int season)
        {
            _query.Clear();
            _query.Append("INSERT INTO user_info (userId, rewardedRankSeason) VALUES (")
                .Append(_userId).Append(",").Append(season)
                .Append(") ON DUPLICATE KEY UPDATE rewardedRankSeason = ").Append(season).Append(";");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
