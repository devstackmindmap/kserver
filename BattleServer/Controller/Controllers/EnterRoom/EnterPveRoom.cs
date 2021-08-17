using AkaLogger;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleServer.Controller.Controllers
{
    public class EnterPvERoom : EnterCommonPvERoom
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoEnterPveRoom;
            session.UserId = req.UserId;

            var battleInfo = new BattleInfo
            {
                BattleType = req.BattleType,
                UserId = req.UserId,
                DeckNum = req.DeckNum,
                StageRoundId = req.StageRoundId,
                StageLevelId = req.StageRoundId == 0 ? req.StageLevelId : 0,
                BattleServerIp = req.BattleServerIp,
                SessionId = session.SessionID
            };

            await DoProcess(session, battleInfo);
        }
    }
}
