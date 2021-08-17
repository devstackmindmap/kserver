using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Challenge
{
    public class WebStartChallenge : BaseController
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

                    if (false == await manager.IsValidSeason())
                        return new ProtoOnChallenge { ResultType = ResultType.InvalidSeason };

                    if (false == await manager.IsValidDay())
                        return new ProtoOnChallenge { ResultType = ResultType.InvalidDay };

                    if (false == await manager.IsBeforeStageClear())
                        return new ProtoOnChallenge { ResultType = ResultType.InvalidFlow };

                    var dataChallenge
                        = Data.GetDataChallenge(await manager.GetSeason(), req.Day);

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
