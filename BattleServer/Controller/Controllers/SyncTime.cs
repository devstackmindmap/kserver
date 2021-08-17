using System;
using System.Threading.Tasks;
using AkaSerializer;
using CommonProtocol;

namespace BattleServer.Controller.Controllers
{
    public class SyncTime : BaseController
    {
        public override async Task DoPipeline(NetworkSession networkSession, BaseProtocol requestInfo)
        {
            networkSession.Send(MessageType.SyncTime, AkaSerializer<ProtoOnSyncTime>.Serialize(new ProtoOnSyncTime
            {
                ServerTime = DateTime.UtcNow.Ticks
            }));
        }
    }
}