using CommonProtocol;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public class EnterEventChallengeRoom : EnterCommonPvERoom
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEnterEventChallengeRoom;
            session.UserId = req.UserId;

            var battleInfo = new BattleInfoEventChallenge
            {
                BattleType = req.BattleType,
                UserId = req.UserId,
                DeckNum = req.DeckNum,
                ChallengeEventId = req.ChallengeEventId,
                DifficultLevel = req.DifficultLevel,
                BattleServerIp = req.BattleServerIp,
                SessionId = session.SessionID,
                IsStart = req.IsStart
            };
            
            await DoProcess(session, battleInfo, req.IsStart);
        }
    }
}
