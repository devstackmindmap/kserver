using AkaConfig;
using AkaEnum;
using AkaSerializer;
using CommonProtocol;
using Network;
using System.Threading.Tasks;

namespace BattleServer.Controller
{
    class EventChallengeInfoHelper : IPveInfoHelper
    {
        public async Task<ResultType> SetBattleInfo(IBattleInfo battleInfo)
        {
            var challengeBattleInfo = battleInfo as BattleInfoEventChallenge;
            WebServerRequestor webServer = new WebServerRequestor();
            var protoOnNormalChallenge
                = await webServer.RequestAsync<ProtoOnChallenge>(MessageType.StartEventChallenge, 
                AkaSerializer<ProtoEventChallenge>.Serialize(new ProtoEventChallenge
                {
                    ChallengeEventId = challengeBattleInfo.ChallengeEventId,
                    DifficultLevel = challengeBattleInfo.DifficultLevel,
                    UserId = challengeBattleInfo.UserId,
                    MessageType = MessageType.StartEventChallenge,
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
