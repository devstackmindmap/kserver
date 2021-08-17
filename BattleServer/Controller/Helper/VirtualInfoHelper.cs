using AkaConfig;
using AkaData;
using AkaEnum;
using AkaSerializer;
using AkaUtility;
using BattleLogic;
using CommonProtocol;
using Network;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    class VirtualInfoHelper : IPveInfoHelper
    {

        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            var virtualBattleInfo = battleInfo as BattleInfo;
            WebServerRequestor webServer = new WebServerRequestor();
            var protoOnTeamRankPoint = await webServer.RequestAsync<ProtoOnRankPoint>(MessageType.GetVirtualRankPoint, AkaSerializer<ProtoRankPoint>.Serialize(new ProtoRankPoint
            {
                UserId = virtualBattleInfo.UserId,
                DeckNum = virtualBattleInfo.DeckNum,
                DeckModeType = BattleEnviroment.GetDeckModeType(battleInfo.BattleType),
                RankType = RankType.AllUnitVirtualRankPoint
            }), $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/");

            var teamRankPoint = protoOnTeamRankPoint.TeamRankPoint;
            var tierMatchingId = Data.GetVirtualLeagueTierMatchingId(teamRankPoint);
            var tierMatchingInfo = Data.GetVirtualLeagueTierMatching(tierMatchingId);

            var stageRoundId = AkaRandom.Random.ChooseElementRandomlyInCount(tierMatchingInfo.StageRoundIdList);
            virtualBattleInfo.StageRoundId = stageRoundId;
            return ResultType.Success;
        }
    }
}
