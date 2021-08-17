using AkaDB.MySql;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebEventChallengeRewardReset : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEventChallengeParam;
            using (DBContext accountDb = new DBContext("AccountDBSetting"), userDb = new DBContext(req.UserId))
            {
                var manager 
                    = ChallengeFactory.CreateEventChallengeManager
                    (accountDb, userDb, req.UserId, req.ChallengeEventId, req.DifficultLevel);

                if (await manager.ResetReward())
                    return new ProtoResult { ResultType = AkaEnum.ResultType.Success };
                else
                    return new ProtoResult { ResultType = AkaEnum.ResultType.Fail };
            }
        }
    }
}
