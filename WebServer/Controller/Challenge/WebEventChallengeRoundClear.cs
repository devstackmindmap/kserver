using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebEventChallengeRoundClear : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEventChallenge;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var manager
                        = ChallengeFactory.CreateEventChallengeManager(accountDb, userDb, req.UserId, req.ChallengeEventId, req.DifficultLevel);
                    await manager.RoundClear();
                    return new ProtoEmpty();
                }
            }
        }
    }
}
