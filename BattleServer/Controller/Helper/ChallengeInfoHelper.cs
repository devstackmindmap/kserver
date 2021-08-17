using AkaConfig;
using AkaEnum;
using AkaSerializer;
using CommonProtocol;
using Network;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    class ChallengeInfoHelper : IPveInfoHelper
    {
        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            var challengeBattleInfo = battleInfo as BattleInfoChallenge;
            WebServerRequestor webServer = new WebServerRequestor();
            var protoOnNormalChallenge
                = await webServer.RequestAsync<ProtoOnChallenge>(MessageType.StartChallenge, 
                AkaSerializer<ProtoChallenge>.Serialize(new ProtoChallenge
                {
                    Season = challengeBattleInfo.Season,
                    Day = challengeBattleInfo.Day,
                    DifficultLevel = challengeBattleInfo.DifficultLevel,
                    UserId = challengeBattleInfo.UserId,
                    MessageType = MessageType.StartChallenge,
                    IsStart = challengeBattleInfo.IsStart
                }), $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/");


            if (protoOnNormalChallenge.ResultType != ResultType.Success)
                return protoOnNormalChallenge.ResultType;

            challengeBattleInfo.StageLevelId = protoOnNormalChallenge.StageLevelId;
            challengeBattleInfo.StageRoundId = protoOnNormalChallenge.StageRoundId;
            challengeBattleInfo.Round = protoOnNormalChallenge.Round;
            return ResultType.Success;
        }
    }
}
