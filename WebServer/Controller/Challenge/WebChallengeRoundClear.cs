using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebChallengeRoundClear : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoChallenge;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    var manager
                        = ChallengeFactory.CreateChallengeManager(accountDb, userDb, req.UserId, req.Season, req.Day, req.DifficultLevel);
                    await manager.RoundClear();
                    return new ProtoEmpty();
                }
            }
        }
    }
}
