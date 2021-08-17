using CommonProtocol;
using System;

namespace TriggerServer
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.ReloadServerList:
                    return new ReloadServerList();
                default:
                    throw new Exception("Invalid MessageType");
            }
        }
    }
}
