using CommonProtocol;
using System;

namespace BattleClientApp
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType battleMessageType)
        {
            switch (battleMessageType)
            {
                case MessageType.OnTryMatching:
                    return new TryMatching();
                default:
                    throw new Exception("Invalid BattleMessageType");
            }
        }
    }
}
