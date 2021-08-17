using System.Threading.Tasks;
using CommonProtocol;

namespace BattleServer.Controller.Controllers
{
    public class BattleReady : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            RoomManager.BattleReady(requestInfo as ProtoBattleReady);
        }
    }
}