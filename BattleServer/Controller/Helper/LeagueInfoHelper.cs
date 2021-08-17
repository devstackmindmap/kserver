using AkaConfig;
using AkaData;
using AkaEnum;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using Network;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    class LeagueInfoHelper : IPveInfoHelper
    {
        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            var leagueBattleInfo = battleInfo as BattleInfo;
            WebServerRequestor webServer = new WebServerRequestor();
            var protoOnTeamRankPoint = await webServer.RequestAsync<ProtoOnRankPoint>(MessageType.GetRankPoint, AkaSerializer<ProtoRankPoint>.Serialize(new ProtoRankPoint
            {
                UserId = leagueBattleInfo.UserId,
                DeckNum = leagueBattleInfo.DeckNum,
                DeckModeType = BattleEnviroment.GetDeckModeType(leagueBattleInfo.BattleType),
                RankType = RankType.AllUnitRankPoint
            }), $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/");

            var teamRankPoint = protoOnTeamRankPoint.TeamRankPoint;

            if (teamRankPoint < 0)
                return ResultType.Fail;

            var tierMatchingId = Data.GetRankTierMatchingId(teamRankPoint);
            var tierMatchingInfo = Data.GetRankTierMatching(tierMatchingId);

            return tierMatchingInfo.StageRoundIdList.Any(stageRoundId => stageRoundId == leagueBattleInfo.StageRoundId) 
                ? ResultType.Success : ResultType.Fail;
        }
    }
}
