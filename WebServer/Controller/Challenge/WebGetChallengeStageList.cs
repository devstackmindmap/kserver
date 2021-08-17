using AkaDB.MySql;
using Common.Entities.Challenge;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebGetChallengeStageList : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var serverSeason = new ServerSeason(accountDb);
                    var seasonInfo = await serverSeason.GetChallengeSeasonInfo();
                    var manager 
                        = ChallengeFactory.CreateChallengeManager
                        (accountDb, userDb, req.UserId, seasonInfo.CurrentSeason, 0, 0);

                    var res = await manager.GetStageList();
                    res.TodayKnightLeagueWinCount = await manager.GetTodayKnightLeagueWinCount();
                    var challengeSeasonInfo = await serverSeason.GetChallengeSeasonInfo();
                    res.NextChallengeStartDateTime = challengeSeasonInfo.NextSeasonStartDateTime.Ticks;
                    return res;
                }
            }
        }
    }
}
