using System.Threading.Tasks;
using CommonProtocol;

namespace BattleServer.Controller.Controllers
{
    public class CardUse : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            RoomManager.CardUse(requestInfo as ProtoCardUse);
        }
    }
}