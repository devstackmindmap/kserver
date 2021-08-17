using System;

namespace AkaLogger.Common
{
    public sealed class LogRollback
    {

        public void Log(uint userId, string transactionRollbackLastSql, string transactionRollbackStack)
        {
            Logger.Instance().Analytics("Rollback", "Rollback",
                "UserId", userId.ToString(),
                "LastSql", transactionRollbackLastSql,
                "CallStack" , transactionRollbackStack);
        }

        public void Log(uint userId, string transactionRollbackLastSql, string transactionRollbackStack, Exception transactionRollbackException)
        {
            Logger.Instance().Analytics("Exception", "Rollback",
                "UserId", userId.ToString(),
                "LastSql", transactionRollbackLastSql,
                "CallStack", transactionRollbackStack,
                "Exception", transactionRollbackException.Message,
                "ExStack", transactionRollbackException.StackTrace);
        }
    }
}
