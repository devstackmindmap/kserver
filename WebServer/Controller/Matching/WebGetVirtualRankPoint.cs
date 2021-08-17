using System.Threading.Tasks;
using AkaDB.MySql;
using CommonProtocol;

namespace WebServer.Controller.Matching
{
    public class WebGetVirtualRankPoint : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoRankPoint;
            var protoOnTeamRankPoint = new ProtoOnRankPoint();
            
            using (var userDb = new DBContext(req.UserId))
            {
                var rank = new Common.Entities.Battle.VirtualRank(userDb, req.UserId, req.DeckNum, req.DeckModeType, req.RankType);
                protoOnTeamRankPoint.TeamRankPoint = await rank.GetSumOfUnitsRankPoint();

                protoOnTeamRankPoint.UserRankPoint = await rank.GetUserRankPoint();
            }
            
            return protoOnTeamRankPoint;
        }
    }
}
