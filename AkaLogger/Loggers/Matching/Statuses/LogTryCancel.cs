using System;

namespace AkaLogger.Matching
{
    public sealed class LogTryCancel
    {
        public void Log( string member)
        {
            Logger.Instance().Analytics("TryCancel", "Matching",
                "MemberKey", member);            
        }

    }
}
