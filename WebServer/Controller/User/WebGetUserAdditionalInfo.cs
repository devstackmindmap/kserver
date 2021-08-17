using System;
using AkaDB.MySql;
using CommonProtocol;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.User
{
    public class WebGetUserAdditionalInfo : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            // 현재쓰지않는 API 만약 쓰게 된다면 검토 필요
            var req = requestInfo as ProtoUserId;

            using (var userDb = new DBContext(req.UserId))
            {
                var query = new StringBuilder();
                query.Append("SELECT isAlreadyFreeNicknameChange, recentDateTimeCountryChange " +
                    ", dailyRankVictoryGoldRewardCount, dailyRankVictoryGoldRewardDateTime, rewardedRankSeason " +
                    ", maxVirtualRankLevel, maxVirtualRankPoint, currentVirtualRankPoint " +
                    "FROM user_info " +
                    "WHERE userid = ").Append(req.UserId).Append(";");

                using (var cursor = await userDb.ExecuteReaderAsync(query.ToString()))
                {
                    if (false == cursor.Read())
                    {
                        return new ProtoAdditionalUserInfo
                        {
                            IsAlreadyFreeNicknameChange = false,
                            RecentDateTimeCountryChange = 0,
                            DailyRankVictoryGoldRewardCount = 0,
                            DailyRankVictoryGoldRewardDateTime = 0,
                            RewardedRankSeason = 0,
                            MaxVirtualRankLevel = 1,
                            MaxVirtualRankPoint = 0,
                            CurrentVirtualRankPoint = 0,
                        };
                    }

                    return new ProtoAdditionalUserInfo
                    {
                        IsAlreadyFreeNicknameChange = (sbyte)cursor["isAlreadyFreeNicknameChange"] == 1 ? true : false,
                        RecentDateTimeCountryChange = ((DateTime)cursor["recentDateTimeCountryChange"]).Ticks,
                        DailyRankVictoryGoldRewardCount = (sbyte)cursor["dailyRankVictoryGoldRewardCount"],
                        DailyRankVictoryGoldRewardDateTime = ((DateTime)cursor["dailyRankVictoryGoldRewardDateTime"]).Ticks,
                        RewardedRankSeason = (int)cursor["rewardedRankSeason"],
                        MaxVirtualRankLevel = (uint)cursor["maxVirtualRankLevel"],
                        MaxVirtualRankPoint = (int)cursor["maxVirtualRankPoint"],
                        CurrentVirtualRankPoint = (int)cursor["currentVirtualRankPoint"],
                    };
                }
            }
        }
    }
}
