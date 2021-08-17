using CommonProtocol;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public class EnterChallengeRoom : EnterCommonPvERoom
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEnterChallengeRoom;
            session.UserId = req.UserId;

            var battleInfo = new BattleInfoChallenge
            {
                BattleType = req.BattleType,
                UserId = req.UserId,
                DeckNum = req.DeckNum,
                Season = req.Season,
                Day = req.Day,
                DifficultLevel = req.DifficultLevel,
                BattleServerIp = req.BattleServerIp,
                SessionId = session.SessionID,
                IsStart = req.IsStart
            };
            
            await DoProcess(session, battleInfo, req.IsStart);
        }
    }
}
