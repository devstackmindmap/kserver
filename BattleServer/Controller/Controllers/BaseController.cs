using CommonProtocol;
using System.Threading.Tasks;

namespace BattleServer
{
    public abstract class BaseController
    {
        public abstract Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo);
    }
}
