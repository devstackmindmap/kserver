using CommonProtocol;
using System.Threading.Tasks;

namespace MatchingServer
{
    public abstract class BaseController
    {
        public abstract  Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo);
    } 
}
