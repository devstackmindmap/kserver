using System.Threading.Tasks;
using CommonProtocol;

namespace BattleServer.Controller.Controllers
{
    public class Retreat : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            RoomManager.Retreat(requestInfo as ProtoRetreat, networkSession.UserId);
        }
    }
}