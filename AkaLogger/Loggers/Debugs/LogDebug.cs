using System;

namespace AkaLogger.Debug
{
    public sealed class LogDebug
    {
        public void Exception(string message, Exception ex)
        {
            Logger.Instance().Analytics("Exception", "Debug",
                "Message", message,
                "Exception", ex.ToString());
        }

        public void Info(string message, string category)
        {
            Logger.Instance().Analytics("Info", "Debug",
                "Message", message,
                "Category", category);
        }

        public void InvalidPacket(string message, string server)
        {
            Logger.Instance().Analytics(server, "InvalidPacket", "Message", message);
        }

        public void InvalidProtocol(string message, string server)
        {
            Logger.Instance().Analytics(server, "InvalidProtocol", "Message", message);
        }

    }
}
