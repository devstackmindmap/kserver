using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using Common.Entities.Reward;
using Common.Entities.Season;
using Common.Entities.User;
using Common.Pass;
using CommonProtocol;

namespace WebServer.Controller.Store
{
    public class WebGetSeasonReward : BaseController
    {
        StringBuilder _query = new StringBuilder();

        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            var res = new ProtoSeasonReward { ResultType = ResultType.Fail };

            uint serverSeason;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var seasonInfo = await (new ServerSeason(accountDb)).GetKnightLeagueSeasonInfo();
                serverSeason = seasonInfo.CurrentSeason;
            }

            using (var userDb = new DBContext(req.UserId))
            {
                var rewardedRankSeason = await RewardedRankSeason(userDb, req.UserId);

                if (serverSeason == rewardedRankSeason.RewardedSeason)
                    return res;

                res.EnableSeasonPassList = rewardedRankSeason.EnableSeasonPass;

                var redis = AkaRedis.AkaRedis.GetDatabase();
                var score = await GameBattleRankRedisJob.GetScoreRankKnightLeagueUserAsync(redis, req.UserId, serverSeason - 1);
                if (false == score.HasValue)
                {
                    var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, req.UserId,
                        UserAdditionalInfoType.RewardedRankSeason);
                    await userInfoChanger.Change(new RequestValue { StringValue = (serverSeason).ToString() });
                    return res;
                }

                res.BeforeSeasonRankPoint = (int)score.Value;
                var beforeSeasonRankLevelId = GetUserRankLevelIdFromPoint(res.BeforeSeasonRankPoint);

                var unitsId = await GetUnitsId(userDb, req.UserId);

                foreach (var unitId in unitsId)
                {
                    var unitScore = await GameBattleRankRedisJob.GetScoreRankKnightLeagueUnitAsync(redis, req.UserId, unitId, serverSeason - 1);
                    if (unitScore.HasValue)
                        res.UnitsBeforeSeasonRankPoint.Add(unitId, (int)unitScore.Value);
                }

                await userDb.BeginTransactionCallback(async () =>
                {
                    res.ItemResults = await Reward.GetRewards(userDb, req.UserId, Data.GetSeasonReward(beforeSeasonRankLevelId).RewardId, "SeasonReward");

                    var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, req.UserId, 
                        UserAdditionalInfoType.RewardedRankSeason);
                    await userInfoChanger.Change(new RequestValue { StringValue = (serverSeason).ToString() });

                    return true;
                });
            }
            res.ResultType = ResultType.Success;
            return res;
        }

        private async Task<RewardWithSeasonPass> RewardedRankSeason(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT rewardedRankSeason, enablePassList FROM user_info WHERE userid = ").Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return new RewardWithSeasonPass { RewardedSeason = 0 };

                return new RewardWithSeasonPass
                {
                    RewardedSeason = (int)cursor["rewardedRankSeason"],
                    EnableSeasonPass = new List<uint>(new SeasonPassManager().GetEnablePassList(cursor))
                };
            }
        }

        private async Task<List<uint>> GetUnitsId(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT id FROM units WHERE userId = ").Append(userId).Append(";");

            var unitsId = new List<uint>();
            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    unitsId.Add((uint)cursor["id"]);
                }
            }

            return unitsId;
        }

        private uint GetUserRankLevelIdFromPoint(int userRankPoint)
        {
            var rows = Data.GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank);

            uint userRankLevelId = 0;
            foreach (var data in rows)
            {
                userRankLevelId = data.UserRankLevelId;
                if (userRankPoint < data.NeedRankPointForNextLevelUp)
                    break;
            }
            return userRankLevelId;
        }

        private sealed class RewardWithSeasonPass
        {
            internal int RewardedSeason { get; set; }
            internal List<uint> EnableSeasonPass { get; set; }
        }
    }
}
