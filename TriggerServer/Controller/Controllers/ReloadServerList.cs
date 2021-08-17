
using AkaSerializer;
using Common;
using CommonProtocol;
using System;
using System.Threading.Tasks;
using TriggerServer.Managers;

namespace TriggerServer
{
    public class ReloadServerList : BaseController
    {
        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo, MessageType msgType)
        {
            ScheduleManagerFactory.GetInstance(typeof(ServerInfoUpdateManager)).DoCommand(msgType, requestInfo);
        }
    }
}
