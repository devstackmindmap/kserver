using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;
using Common.Entities.Season;
using CommonProtocol;

namespace WebServer.Controller.Rank
{
    public class WebGetRankingBoard : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoRankingBoard;
            var res = new ProtoOnRankingBoard();
            
            using (var db = new DBContext("AccountDBSetting"))
            {
                var serverSeason = new ServerSeason(db);
                var serverSeasonInfo = await  serverSeason.GetKnightLeagueSeasonInfo();

                var redis = AkaRedis.AkaRedis.GetDatabase();

                var redisRankInfo 
                    = await AkaRedisLogic.GameBattleRankRedisJob.GetTopRanksKnightLeagueUserAsync(redis, serverSeasonInfo.CurrentSeason, req.CountryCode);

                if (redisRankInfo.Length == 0)
                    return res;

                List<int> userIds = new List<int>();
                for (int i = 0; i < redisRankInfo.Length; i++)
                {
                    int userId;

                    if (redisRankInfo[i].Element.TryParse(out userId))
                    {
                        userIds.Add(userId);
                        res.RankingBoard.Add((uint)userId, new ProtoRankInfo { RankPoint = (int)redisRankInfo[i].Score });
                    }
                }

                var query = new StringBuilder();
                query.Append("SELECT userId, nickName, profileIconId, clanName " +
                    "FROM accounts WHERE userId IN (").Append(string.Join(",", userIds)).Append(");");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    while (cursor.Read())
                    {
                        var userId = (uint)cursor["userId"];

                        res.RankingBoard[userId].UserId = userId;
                        res.RankingBoard[userId].Nickname = (string)cursor["nickname"];
                        res.RankingBoard[userId].ProfileIconId = (uint)cursor["profileIconId"];
                        res.RankingBoard[userId].ClanName = (string)cursor["clanName"];
                    }
                }
            }
            return res;
        }
    }
}
