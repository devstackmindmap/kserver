using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaRedisLogic;
using Common.Entities.User;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace FixRewardedRankReward
{
    class SeasonFromTo
    {
        public string SeasonFrom;
        public string SeasonTo;
    }

    //N = RewardedRankSeason
    //S = N
    //S 시즌 로그인 정보가 있는지 확인
    //로그인 정보가 있다면 보상 지급 후 S = S+ 1

    //보상 지급 S 의 레디스에서 점수가져와서
    //점수에 많는 등급 확인
    //등급에 맞는 productId 에 대한 메일 씀.

    //S = 9까지 진행

    class FixRewardedRankReward
    {
        private DBContext _accountDb;
        private StringBuilder _query = new StringBuilder();
        private IDatabase _redis;
        private string _server;
        private uint _targetUserId;

        private Dictionary<uint, uint> products = new Dictionary<uint, uint>();
        private Dictionary<uint, SeasonFromTo> _seasonFromTo = new Dictionary<uint, SeasonFromTo>();

        public FixRewardedRankReward(DBContext accountDb, string server, uint targetUserId)
        {
            _accountDb = accountDb;
            _server = server;
            _targetUserId = targetUserId;
            _redis = AkaRedis.AkaRedis.GetDatabase();
        }

        public async Task Run()
        {
            // 0.
            SetData();

            // 1. 리스트 테이블 Select temp_fix_rewarded_rank_reward
            
            var fixList = await GetTargets();
            foreach (var user in fixList)
            {
                for (int season = user.Value; season < 10; season++)
                {
                    await UserJob(user.Key, (uint)season);
                }
            }
        }
        
        private async Task<Dictionary<uint,int>> GetTargets()
        {
            var fixList = new Dictionary<uint, int>();
            _query.Clear();
            if (_targetUserId != 0)
            {
                _query.Append("SELECT userId, rewardedRankSeason FROM temp_fix_rewarded_rank_reward WHERE userId=")
                    .Append(_targetUserId).Append(";");
            }
            else
            {
                _query.Append("SELECT userId, rewardedRankSeason FROM temp_fix_rewarded_rank_reward;");
            }
            

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    fixList.Add((uint)cursor["userId"], (int)cursor["rewardedRankSeason"]);
                }
            }
            return fixList;
        }


        private async Task UserJob(uint userId, uint season)
        {
            // 시즌 로그인 정보가 있는지 확인, 없으면 return
            // 보상 정보 가져오기
            // ㄴ 해당 시즌 점수 가져와서 보상 Level에 맞는 ProductId 가져오기
            try
            {
                if (_server == "Live" && false == await IsLogin(userId, season+1))
                {
                    using (var userDb = new DBContext(userId))
                    {
                        var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, userId,
                       UserAdditionalInfoType.RewardedRankSeason);
                        await userInfoChanger.Change(new RequestValue { StringValue = (season + 1).ToString() });
                        await SetTempTableRewardedSeason(userId, season + 1);
                    }
                    Console.WriteLine("[NoLogin] UserID : " + userId + ", Season : " + season);
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Kpi DB Error] UserID : " + userId + ", Season : " + season);
                return;
            }

            using (var userDb = new DBContext(userId))
            {
                var score = await GameBattleRankRedisJob.GetScoreRankKnightLeagueUserAsync(_redis, userId, season);
                if (false == score.HasValue)
                {
                    var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, userId,
                        UserAdditionalInfoType.RewardedRankSeason);
                    await userInfoChanger.Change(new RequestValue { StringValue = (season + 1).ToString() });
                    await SetTempTableRewardedSeason(userId, season + 1);
                    Console.WriteLine("[No Score] UserID : " + userId + ", Season : " + season);
                    return;
                }

                var seasonRankLevelId = GetUserRankLevelIdFromPoint((int)score.Value);
                var productId = products[seasonRankLevelId];

                await userDb.BeginTransactionCallback(async () =>
                {
                    await WriteMail(userDb, userId, productId, season);

                    var userInfoChanger = UserAdditionalInfoFactory.CreateUserInfoChanger(null, userDb, userId,
                        UserAdditionalInfoType.RewardedRankSeason);
                    await userInfoChanger.Change(new RequestValue { StringValue = (season + 1).ToString() });
                    await SetTempTableRewardedSeason(userId, season + 1);

                    Console.WriteLine("[Send Mail] UserID : " + userId + ", Season : " + season);
                    return true;
                });
            }
            // 해당 보상 메일쓰기
        }

        private async Task<bool> IsLogin(uint userId, uint season)
        {
            _query.Clear();
            _query.Append("SELECT * FROM login_log_season WHERE userId = ").Append(userId)
                .Append(" AND `date` >= '").Append(_seasonFromTo[season].SeasonFrom)
                .Append("' AND `date` < '").Append(_seasonFromTo[season].SeasonTo)
                .Append("';");

            using (var logDb = new TempDBContext(_server))
            {
                logDb.Connect();

                using (var cursor = await logDb.ExecuteReaderAsync(_query.ToString()))
                {
                    return cursor.Read();
                }
            }
        }

        private async Task WriteMail(DBContext userDb, uint userId, uint productId, uint season)
        {
            _query.Clear();
            _query.Append("INSERT INTO user_mail_private (userId, isDeleted, startDateTime, endDateTime, mailTitle, mailText, isRead, mailIcon, productId)" +
                " VALUES (").Append(userId).Append(",0,'2020-09-01 02:00:00', '2021-09-01 02:00:00', '나이트리그 시즌").Append(season).Append(" 보상','" +
                " 지급받지 못한 나이트리그 시즌 보상을 소급 적용해 드립니다. (시즌").Append(season).Append(")', 0, 'thumb_mail_icon_002',").Append(productId)
                .Append(");");

            await userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private uint GetUserRankLevelIdFromPoint(int userRankPoint)
        {
            var rows = Data.GetPrimitiveValues<uint, DataUserRank>(DataType.data_user_rank);

            uint userRankLevelId = 0;
            foreach (var data in rows)
            {
                userRankLevelId = data.UserRankLevelId;
                if (userRankPoint < data.NeedRankPointForNextLevelUp)
                    break;
            }
            return userRankLevelId;
        }

        private void SetData()
        {
            products.Add(1, 94011);
            products.Add(2, 94021);
            products.Add(3, 94031);
            products.Add(4, 94041);
            products.Add(5, 94051);
            products.Add(6, 94061);
            products.Add(7, 94071);
            products.Add(8, 94081);
            products.Add(9, 94091);
            products.Add(10, 94101);
            products.Add(11, 94111);
            products.Add(12, 94121);
            products.Add(13, 94131);
            products.Add(14, 94141);
            products.Add(15, 94151);
            products.Add(16, 94161);
            products.Add(17, 94171);
            products.Add(18, 94181);
            products.Add(19, 94191);
            products.Add(20, 94201);
            products.Add(21, 94211);
            products.Add(22, 94221);
            products.Add(23, 94231);
            products.Add(24, 94241);
            products.Add(25, 94251);
            products.Add(26, 94261);
            products.Add(27, 94271);
            products.Add(28, 94281);
            products.Add(29, 94291);
            products.Add(30, 94301);

            _seasonFromTo.Add(2, new SeasonFromTo { SeasonFrom = "2020-04-16 02:00:00", SeasonTo = "2020-05-01 02:00:00" });
            _seasonFromTo.Add(3, new SeasonFromTo { SeasonFrom = "2020-05-01 02:00:00", SeasonTo = "2020-05-16 02:00:00" });
            _seasonFromTo.Add(4, new SeasonFromTo { SeasonFrom = "2020-05-16 02:00:00", SeasonTo = "2020-05-31 02:00:00" });
            _seasonFromTo.Add(5, new SeasonFromTo { SeasonFrom = "2020-05-31 02:00:00", SeasonTo = "2020-06-15 02:00:00" });
            _seasonFromTo.Add(6, new SeasonFromTo { SeasonFrom = "2020-06-15 02:00:00", SeasonTo = "2020-06-30 02:00:00" });
            _seasonFromTo.Add(7, new SeasonFromTo { SeasonFrom = "2020-06-30 02:00:00", SeasonTo = "2020-07-15 02:00:00" });
            _seasonFromTo.Add(8, new SeasonFromTo { SeasonFrom = "2020-07-15 02:00:00", SeasonTo = "2020-07-30 02:00:00" });
            _seasonFromTo.Add(9, new SeasonFromTo { SeasonFrom = "2020-07-30 02:00:00", SeasonTo = "2020-08-14 02:00:00" });
            _seasonFromTo.Add(10, new SeasonFromTo { SeasonFrom = "2020-08-14 02:00:00", SeasonTo = "2020-08-29 02:00:00" });
        }

        private async Task SetTempTableRewardedSeason(uint userId, uint season)
        {
            _query.Clear();
            _query.Append("UPDATE temp_fix_rewarded_rank_reward SET rewardedRankSeason = ").Append(season)
                .Append(" WHERE userId = ").Append(userId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }

    public class TempDBContext : IDisposable
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

        string _server;

        public TempDBContext(string server)
        {
            _server = server;
        }
        public void Connect()
        {
            // Dylan2
            //"user": "root",
            //"password": "alaighdi54$%",
            //"database": "kpi",
            //"host": "172.30.1.222",
            //"port": 3306,
            //"charset": "utf8"

            // Live
            //"user": "akaAdmin",
            //"password": "dkzkdjemals$#",
            //"database": "kpi",
            //"host": "knightrun.cluster-ceofckii5eec.ap-northeast-2.rds.amazonaws.com",
            //"port": 3306,
            //"charset": "utf8"

            var stringBuilder = new MySqlConnectionStringBuilder();
            if (_server == "Dylan2")
            {
                stringBuilder.Server = "172.30.1.222";
                stringBuilder.Database = "kpi";
                stringBuilder.UserID = "root";
                stringBuilder.Password = "alaighdi54$%";
                stringBuilder.CharacterSet = "utf8";
                stringBuilder.Port = 3306;
            }
            else if (_server == "Live")
            {
                stringBuilder.Server = "knightrun.cluster-ceofckii5eec.ap-northeast-2.rds.amazonaws.com";
                stringBuilder.Database = "kpi";
                stringBuilder.UserID = "akaAdmin";
                stringBuilder.Password = "dkzkdjemals$#";
                stringBuilder.CharacterSet = "utf8";
                stringBuilder.Port = 3306;
            }
            
            stringBuilder.Pooling = true;
            stringBuilder.MinimumPoolSize = 10;
            stringBuilder.MaximumPoolSize = 100;
            stringBuilder.SslMode = MySqlSslMode.None;
            

            try
            {
                connection = new MySqlConnection(stringBuilder.ConnectionString);
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
            catch (Exception ex)
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

        public async Task<DbDataReader> CallStoredProcedureAsync(string name, TempDBParamInfo paramInfo, bool isTransactionEnable = false)
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

        public class TempDBParamInfo
        {
            public InputArg[] InputArgs { get; set; }
            public OutputArg[] OutputArgs { get; set; }
            public TempDBContext Sender { get; set; }
            bool isNextResult = true;

            public void SetInputParam(params InputArg[] args)
            {
                InputArgs = args;
            }

            public void SetOutputParam(params OutputArg[] args)
            {
                OutputArgs = args;
            }


            public T GetOutValue<T>(string key)
            {
                if (isNextResult)
                {
                    while (Sender.Cursor.NextResult()) { }

                    isNextResult = false;
                }

                var value = Sender.Command.Parameters[key].Value;
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }


}


//SELECT a.userId, nickname, a.loginDateTime, b.rewardedRankSeason, a.joinDateTime FROM accounts a
//LEFT OUTER JOIN knightrun.user_info b ON b.userid = a.userId
//WHERE (a.loginDateTime >= '2020-08-29 02:00:00' AND b.rewardedRankSeason< 11)
//OR(a.loginDateTime >= '2020-08-14 02:00:00' AND a.loginDateTime < '2020-08-29 02:00:00'  AND b.rewardedRankSeason < 10)
//OR(a.loginDateTime >= '2020-07-30 02:00:00' AND a.loginDateTime < '2020-08-14 02:00:00'  AND b.rewardedRankSeason < 9)
//OR(a.loginDateTime >= '2020-07-15 02:00:00' AND a.loginDateTime < '2020-07-30 02:00:00'  AND b.rewardedRankSeason < 8)
//OR(a.loginDateTime >= '2020-06-30 02:00:00' AND a.loginDateTime < '2020-07-15 02:00:00'  AND b.rewardedRankSeason < 7)
//OR(a.loginDateTime >= '2020-06-15 02:00:00' AND a.loginDateTime < '2020-06-30 02:00:00'  AND b.rewardedRankSeason < 6)
//OR(a.loginDateTime >= '2020-05-31 02:00:00' AND a.loginDateTime < '2020-06-15 02:00:00'  AND b.rewardedRankSeason < 5)
//OR(a.loginDateTime >= '2020-05-16 02:00:00' AND a.loginDateTime < '2020-05-31 02:00:00'  AND b.rewardedRankSeason < 4)
//OR(a.loginDateTime >= '2020-05-01 02:00:00' AND a.loginDateTime < '2020-05-16 02:00:00'  AND b.rewardedRankSeason < 3)
//OR(a.loginDateTime >= '2020-04-16 02:00:00' AND a.loginDateTime < '2020-05-01 02:00:00'  AND b.rewardedRankSeason < 2)
//ORDER BY a.loginDateTime ASC;


