using System.Threading.Tasks;
using CommonProtocol;

namespace BattleServer.Controller.Controllers
{
    public class EmoticonUse : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            RoomManager.EmoticonUse(requestInfo as ProtoEmoticonUse);
        }
    }
}