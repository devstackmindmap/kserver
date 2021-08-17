using AkaDB.MySql;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebChallengeRewardReset : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoChallengeParam;
            using (DBContext accountDb = new DBContext("AccountDBSetting"), userDb = new DBContext(req.UserId))
            {
                var manager 
                    = ChallengeFactory.CreateChallengeManager
                    (accountDb, userDb, req.UserId, req.Season, req.Day, req.DifficultLevel);

                if (await manager.ResetReward())
                    return new ProtoResult { ResultType = AkaEnum.ResultType.Success };
                else
                    return new ProtoResult { ResultType = AkaEnum.ResultType.Fail };
            }
        }
    }
}
