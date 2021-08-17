using AkaDB.MySql;
using Common.Entities.Challenge;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebGetEventChallengeStageList : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdAndId;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var manager 
                        = ChallengeFactory.CreateEventChallengeManager
                        (accountDb, userDb, req.UserId, req.Id, 0);

                    return await manager.GetStageList();
                }
            }
        }
    }
}
