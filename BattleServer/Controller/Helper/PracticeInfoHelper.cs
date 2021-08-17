using AkaData;
using System.Linq;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;

namespace BattleServer.Controller
{
    class PracticeInfoHelper : IPveInfoHelper
    {

        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            var practiceBattleInfo = battleInfo as BattleInfo;
            await Task.Yield();
            var stageLevel = Data.GetStageLevel(practiceBattleInfo.StageLevelId);
            if (stageLevel != null && stageLevel.StageType == AkaEnum.StageType.PRACTICE)
            {
                var stageRounds = Data.GetStageRoundList(stageLevel.StageLevelId);

                if (stageRounds.Any())
                {
                    var stageRound = AkaRandom.Random.ChooseElementRandomlyInCount(stageRounds);
                    practiceBattleInfo.StageRoundId = stageRound.StageRoundId;
                    return ResultType.Success;
                }
            }
            return ResultType.Fail;
        }
    }
}
