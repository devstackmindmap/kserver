using System.Threading.Tasks;
using AkaDB.MySql;
using CommonProtocol;

namespace WebServer.Controller.Matching
{
    public class WebGetTeamRankPoint : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoRankPoint;
            var protoOnTeamRankPoint = new ProtoOnRankPoint();
            
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var rank = new Common.Entities.Battle.RankResult(accountDb, userDb, req.UserId, req.DeckNum, req.DeckModeType, null, req.RankType);
                    protoOnTeamRankPoint.TeamRankPoint = await rank.GetSumOfUnitsRankPoint();

                    var userRankInfo = await rank.GetUserRankPoint();
                    protoOnTeamRankPoint.UserRankPoint = userRankInfo.rankPoint;
                    protoOnTeamRankPoint.Wins = userRankInfo.winsCount;
                }
            }
            
            return protoOnTeamRankPoint;
        }
    }
}
