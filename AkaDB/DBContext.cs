using AkaConfig;
using AkaLogger;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AkaDB.MySql
{
    public class DBContext : IDisposable
    {
        private uint _userId;
        MySqlConnection connection;
        MySqlTransaction transaction;
        public MySqlCommand Command { get; private set; }   
        public DbDataReader Cursor { get; private set; }
        bool isTransactionCompleted = false;


        string _transactionRollbackStack = "";
        Exception _transactionRollbackException;
        string _transactionRollbackLastSql = "";

        public DBContext(string dbVariety)
        {
            Connect(0, dbVariety);
        }

        public DBContext(uint userId = 0, string dbVariety = "UserDBSetting")
        {
            _userId = userId;
            Connect(userId, dbVariety);
        }

        private void Connect(uint userId, string dbVariety = "UserDBSetting")
        {
            try
            {
                var shardNum = Config.GetShardNum(userId);
                connection = new MySqlConnection(
                    DBEnv.SettingBuilderMap[dbVariety]
                    [shardNum].ConnectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public async Task<DbDataReader> ExecuteReaderAsync(string text, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);
            _transactionRollbackLastSql = Command.CommandText;

            Cursor = await Command.ExecuteReaderAsync();

            return Cursor;
        }

        public async Task<int> ExecuteNonQueryAsync(string text, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);
            _transactionRollbackLastSql = Command.CommandText;
            return await Command.ExecuteNonQueryAsync();
        }

        public long LastInsertedId()
        {
            return Command.LastInsertedId;
        }

        public async Task<int> ExecuteNonQueryAsync(string text, MySqlParameter[] sqlParam, params object[] args)
        {
            CloseCommand();

            Command = new MySqlCommand(String.Format(text, args), connection, transaction);

            if ((sqlParam?.Length ?? 0) != 0)
                Command.Parameters.AddRange(sqlParam);
            _transactionRollbackLastSql = Command.CommandText;
            return await Command.ExecuteNonQueryAsync();
        }

        public async Task<bool> BeginTransactionCallback(Func<Task<bool>> onTransactionBegin)
        {
            if (transaction != null)
            {
                //To do : Log
                //throw new UserWebException("already transaction is declared, it is not duplicated", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
            }

            if (Command != null && Cursor == null)
            {
                //To do : Log
                //throw new UserWebException("before call of BeginTransactionCallback, insert or update statement is executed, it is not invalid usage", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
            }//Update, Insert statement

            CloseCommand();

            transaction = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {
                isTransactionCompleted = await onTransactionBegin();
            }
            catch(Exception ex)
            {
                AkaLogger.Log.Debug.Exception("InnerTransactionError:" + _transactionRollbackLastSql, ex);
                _transactionRollbackException = ex;
                _transactionRollbackStack = Environment.StackTrace;
                throw ex;
            }

            if (isTransactionCompleted == false)
            {
                _transactionRollbackStack = Environment.StackTrace;
            }
            return isTransactionCompleted;
        }

        public async Task<DbDataReader> CallStoredProcedureAsync(string name, DBParamInfo paramInfo, bool isTransactionEnable = false)
        {
            bool isInnerTransaction = false;

            try
            {
                if (isTransactionEnable)
                {
                    if (transaction != null)
                    {
                        //To do : Log
                        //throw new UserWebException("already transaction is declared, it is not duplicated", WebErrorType.LogicError, WebSubErrorType.InvalidUsage);
                    }

                    transaction = await connection.BeginTransactionAsync(IsolationLevel.RepeatableRead);
                    isInnerTransaction = true;
                }

                CloseCommand();

                Command = new MySqlCommand(name, connection);
                Command.CommandType = CommandType.StoredProcedure;
                Command.Transaction = transaction;

                _transactionRollbackLastSql = name;

                if (paramInfo.InputArgs != null)
                {
                    foreach (var arg in paramInfo.InputArgs)
                    {
                        Command.Parameters.AddWithValue(arg.Key, arg.Value);
                        Command.Parameters[arg.Key].Direction = ParameterDirection.Input;
                    }
                }

                if (paramInfo.OutputArgs != null)
                {
                    foreach (var arg in paramInfo.OutputArgs)
                    {
                        Command.Parameters.Add(arg.Key, arg.ValueType);
                        Command.Parameters[arg.Key].Direction = ParameterDirection.Output;
                    }
                }

                Cursor = await Command.ExecuteReaderAsync();

                paramInfo.Sender = this;
                //paramInfo.ResultType = ResultType.Success;

                if (isInnerTransaction)
                {
                    isTransactionCompleted = true;
                }

                return Cursor;

            }
            catch (MySqlException e)
            {
                // To do : Log
                //var sqlstate = e.Message.Split('|')[0];

                //if (sqlstate == "45000")
                //{
                //    if (!System.Enum.IsDefined(typeof(ResultType), e.Number))
                //    {
                //        throw new UserWebException("error number in stored procedure is not equal to ResultType", WebErrorType.StoredProcedureError, WebSubErrorType.InvalidUsage, e, e.Number);
                //    }

                //    paramInfo.ResultType = (ResultType)e.Number;
                //    return null;
                //}//유저 정의 에러 ResultType에 설정되어짐. throw되지 않음.
                //else if (sqlstate == "46000")
                //{
                //    if (!System.Enum.IsDefined(typeof(WebSubErrorType), e.Number))
                //    {
                //        throw new UserWebException("error number is stored procedure is not equal to WebSubErrorType", WebErrorType.StoredProcedureError, WebSubErrorType.InvalidUsage, e, e.Number);
                //    }

                //    throw new UserWebException(e.Message, WebErrorType.StoredProcedureError, (WebSubErrorType)e.Number, e);
                //}//유저 정의에러 WebSubErrorType으로 throw 되어짐

                _transactionRollbackException = e;
                _transactionRollbackStack = Environment.StackTrace;
                throw e;
            }
        }

        public void Commit()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Dispose()
        {
            CloseCommand();

            if (transaction != null)
            {
                if (isTransactionCompleted)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    if (_transactionRollbackException == null)
                        Log.Common.Rollback.Log(_userId, _transactionRollbackLastSql, _transactionRollbackStack);
                    else
                        Log.Common.Rollback.Log(_userId, _transactionRollbackLastSql, _transactionRollbackStack, _transactionRollbackException);
                }

                transaction.Dispose();
                transaction = null;
            }

            connection.Dispose();
        }

        void CloseCommand()
        {
            if (Cursor != null)
            {
                if (!Cursor.IsClosed)
                {
                    Cursor.Close();
                    Cursor = null;
                }
            }

            Command?.Dispose();
            Command = null;
        }
    }
}
