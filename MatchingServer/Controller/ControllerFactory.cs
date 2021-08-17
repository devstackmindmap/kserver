using CommonProtocol;
using System;

namespace MatchingServer
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType messageType, int areaIndex, int matchingLine)
        {
            switch (messageType)
            {
                case MessageType.TryFvFMatching:
                case MessageType.TryMatching:
                    return new TryMatching(areaIndex, matchingLine);
                case MessageType.MatchingCancel:
                    return new MatchingCancel();
                default:
                    throw new Exception("Invalid MessageType");
            }
        }
    }
}
