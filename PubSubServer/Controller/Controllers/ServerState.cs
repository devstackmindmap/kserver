using System.Threading.Tasks;
using AkaSerializer;
using CommonProtocol;

namespace PubSubServer
{
    public class ServerState : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            networkSession.Send(MessageType.OnGetServerState, AkaSerializer<ProtoOnServerState>.Serialize(new ProtoOnServerState
            {
                MessageType = MessageType.OnGetServerState,
                Sessions = MainServer.Instance.SessionCount
            }));
        }
    }
}