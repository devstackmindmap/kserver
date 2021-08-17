using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebStartEventChallenge : BaseController
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

                    if (false == await manager.IsInEvent(true))
                        return new ProtoOnChallenge { ResultType = ResultType.InvalidDateTime };

                    if (false == await manager.IsValidFlow())
                        return new ProtoOnChallenge { ResultType = ResultType.InvalidFlow };

                    var dataChallenge
                        = Data.GetDataChallengeEvent(await manager.GetChallengeEventNum());

                    var stageLevelId = dataChallenge.StageLevelIdList[req.DifficultLevel - 1];
                    var round = (uint)(req.IsStart ? 0 : await manager.GetCurrentRound());
                    var stageRoundId 
                        = Data.IsValidStageRound(stageLevelId, round) ? Data.GetStageRound(stageLevelId, round).StageRoundId : 0;

                    if (req.IsStart)
                        await manager.StartSetDb();

                    return new ProtoOnChallenge
                    {
                        ResultType = ResultType.Success,
                        StageLevelId = stageLevelId,
                        StageRoundId = stageRoundId,
                        Round = round
                    };
                }
            }
        }
    }
}
