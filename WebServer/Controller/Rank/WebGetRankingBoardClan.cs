using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Clan;
using Common.Entities.Season;
using CommonProtocol;

namespace WebServer.Controller.Rank
{
    public class WebGetRankingBoardClan : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoRankingBoard;
            var res = new ProtoOnRankingBoardClan();
            
            using (var db = new DBContext("AccountDBSetting"))
            {
                var serverSeason = new ServerSeason(db);
                var serverSeasonInfo = await  serverSeason.GetKnightLeagueSeasonInfo();
                var clanRankBoardSeason = await serverSeason.GetCommonValue1(ServerCommonTable.ClanRankBoardServerSeason);

                if (serverSeasonInfo.CurrentSeason != clanRankBoardSeason)
                {
                    res.ResultType = AkaEnum.ResultType.ClanRankBoardCalculating;
                    return res;
                }

                var redis = AkaRedis.AkaRedis.GetDatabase();
                var redisRankInfo 
                    = await AkaRedisLogic.GameBattleRankRedisJob.GetTopRanksKnightLeagueClanAsync(redis, serverSeasonInfo.CurrentSeason, req.CountryCode);

                if (redisRankInfo.Length == 0)
                    return res;

                List<int> clanIds = new List<int>();
                for (int i = 0; i < redisRankInfo.Length; i++)
                {
                    int clanId;

                    if (redisRankInfo[i].Element.TryParse(out clanId))
                    {
                        clanIds.Add(clanId);
                        res.RankingBoard.Add((uint)clanId, new ProtoRankInfoClan { RankPoint = (int)redisRankInfo[i].Score});
                    }
                }

                var query = new StringBuilder();
                query.Append("SELECT clanId, clanName, clanSymbolId, memberCount " +
                    "FROM clans WHERE clanId IN (").Append(string.Join(",", clanIds)).Append(");");

                using (var cursor = await db.ExecuteReaderAsync(query.ToString()))
                {
                    while (cursor.Read())
                    {
                        var clanId = (uint)cursor["clanId"];

                        res.RankingBoard[clanId].ClanId = clanId;
                        res.RankingBoard[clanId].ClanName = (string)cursor["clanName"];
                        res.RankingBoard[clanId].ClanSymbolId = (uint)cursor["clanSymbolId"];
                        res.RankingBoard[clanId].MemberCount = (int)cursor["memberCount"];
                    }
                }
            }
            res.ResultType = AkaEnum.ResultType.Success;
            return res;
        }
    }
}
