using AkaConfig;
using AkaData;
using AkaEnum;
using AkaSerializer;
using CommonProtocol;
using Network;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    class RoguelikeInfoHelper : IPveInfoHelper
    {

        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            //var roguelikeBattleInfo = battleInfo as BattleInfoRoguelike;
            var roguelikeBattleInfo = battleInfo as BattleInfo;
            if (roguelikeBattleInfo.StageRoundId != 0)
                return ResultType.Fail;

            WebServerRequestor webServer = new WebServerRequestor();
            var protoStageInfo = await webServer.RequestAsync<ProtoOnGetStageLevelRoomInfo>(MessageType.GetStageLevelRoomInfo, AkaSerializer<ProtoGetStageLevelRoomInfo>.Serialize(new ProtoGetStageLevelRoomInfo
            {
                UserId = roguelikeBattleInfo.UserId,
                StageLevelId = roguelikeBattleInfo.StageLevelId,
                BattleType = roguelikeBattleInfo.BattleType
            }), $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/");

            //교체 카드, 추가 유물 선택은 전투진입단계에선 있으면 안됨.
            var replaceCardCount = protoStageInfo.ReplaceCardStatIdList?.Count ?? 0;
            var proposalTreasureIdList = protoStageInfo.ProposalTreasureIdList?.Count ?? 0;
            if (protoStageInfo.StageLevelId != 0 && replaceCardCount == 0 && proposalTreasureIdList == 0)
            {
                if ((protoStageInfo.CardStatIdList?.Count ?? 0) > 0)
                {
                    protoStageInfo.ClearRound++;
                    //roguelikeBattleInfo.SaveDeckCardStatIdList = protoStageInfo.CardStatIdList;
                }

                //roguelikeBattleInfo.TreasureIdList = protoStageInfo.TreasureIdList;

                var stageRound = Data.GetStageRoundList(protoStageInfo.StageLevelId)?.ElementAtOrDefault((int)protoStageInfo.ClearRound);

                if (stageRound != null)
                {
                    roguelikeBattleInfo.StageLevelId = stageRound.StageLevelId;
                    roguelikeBattleInfo.StageRoundId = stageRound.StageRoundId;
                    return ResultType.Success;
                }
            }

            return ResultType.Fail;
        }
    }
}
