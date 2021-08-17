using CommonProtocol;
using System.Threading.Tasks;

namespace TriggerServer
{
    public abstract class BaseController
    {
        public abstract  Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo, MessageType msgType);
    } 
}
