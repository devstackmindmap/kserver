using AkaDB.MySql;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer.Controller.User
{
    public class WebGetUnitProfile : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            uint serverCurrentSeason = 0;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                ServerSeason serverSeason = new ServerSeason(accountDb);
                var serverInfo = await serverSeason.GetKnightLeagueSeasonInfo();
                serverCurrentSeason = serverInfo.CurrentSeason;
            }

            using (var userDb = new DBContext(req.UserId))
            {
                UnitProfile userProfile = new UnitProfile(userDb, req.UserId);
                var res = await userProfile.GetUnitProfile();
                res.CurrentSeason = serverCurrentSeason;
                return res;
            }
            
        }
    }
}
