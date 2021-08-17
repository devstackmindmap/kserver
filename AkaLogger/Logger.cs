using NLog;
using System;
using System.Collections.Generic;


namespace AkaLogger
{
   
    public class Logger : IDisposable
    {
        private static Logger _instance;

        LogFactory _factory;
        NLog.Logger _logger;
        NLog.Logger _crashLogger;
        NLog.Logger logger;

        public static Logger Instance()
        {
            if(_instance == null)
            {
                _instance = new Logger();
            }

            return _instance;
        }

        public Logger()
        {
            _factory = new LogFactory();
             _logger = _factory.GetLogger("Log");
            _crashLogger = _factory.GetLogger("Crash");

            //_logger.Factory.Configuration.Variables.Add("runtime", "test");
            _factory.ReconfigExistingLoggers();
        }


        public void Log(LogEventInfo logEventInfo)
        {
            _logger.Log(logEventInfo);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
        
        public void Warn(LogEventInfo logEventInfo)
        {
            _logger.Warn(logEventInfo);
        }

        public void Warn(int message)
        {
            _logger.Warn(message);
        }

        public void Warn(double message)
        {
            _logger.Warn(message);
        }

        public void Warn(Exception e, string message)
        {
            _logger.Warn(e, message);
        }

        public void Warn(string message, params object[] args )
        {
            _logger.Warn(message, args);
        }

        public void Error(object obj)
        {
            _logger.Error(obj); ;
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(int message)
        {
            _logger.Error(message);
        }

        public void Error(double message)
        {
            _logger.Error(message);
        }

        public void Error(Exception e, string message)
        {
            _logger.Error(e, message);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Debug(object obj)
        {
            _logger.Debug(obj); ;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(int message)
        {
            _logger.Debug(message);
        }

        public void Debug(double message)
        {
            _logger.Debug(message);
        }

        public void Debug(Exception e, string message)
        {
            _logger.Debug(e, message);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Exception(string message, Exception ex)
        {
            _crashLogger.Error(ex, message);
        }

        public void Fatal(string message, Exception ex)
        {
            _crashLogger.Fatal(ex, message);
        }


        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(int message)
        {
            _logger.Info(message);
        }

        public void Info(double message)
        {
            _logger.Info(message);
        }

        public void Info(Exception e, string message)
        {
            _logger.Info(e, message);
        }

        public void Info(string message, params object[] args)
        {

            _logger.Info(message, args);
            
        }

        public void Info(DateTime dateTime)
        {
            _logger.Info(dateTime.ToString());
        }

        public void Analytics(string message, params object[] args)
        {
            logger = _factory.GetLogger(args[0].ToString());
            var logEventInfo = new LogEventInfo(LogLevel.Info, args[0].ToString(), message);
            for (int i = 1; i < args.Length; i = i + 2)
            {
                logEventInfo.Properties[args[i].ToString()] = args[i + 1]?.ToString() ?? "Null";
            }
            
            logger.Info(logEventInfo);
        }



        //public void User(string userID, string nickName, string UDID, string ADID, string userLevel, int freeGold, int paidGold, int freeGem, int paidGem, string socialID, string placeType )
        //{
        //    logger = _factory.GetLogger("User");
        //    var logEventInfo = new LogEventInfo(LogLevel.Info, "Login", "");

        //    logEventInfo.Properties["userID"] = userID;
        //    logEventInfo.Properties["nickName"] = userID;
        //    logEventInfo.Properties["UDID"] = UDID;
        //    logEventInfo.Properties["ADID"] = ADID;
        //    logEventInfo.Properties["userLevel"] = userLevel;
        //    logEventInfo.Properties["freeGold"] = freeGold;
        //    logEventInfo.Properties["paidGold"] = paidGold;
        //    logEventInfo.Properties["freeGem"] = freeGem;
        //    logEventInfo.Properties["paidGem"] = paidGem;
        //    logEventInfo.Properties["socialID"] = socialID;
        //    logEventInfo.Properties["placeType"] = placeType;

        //    logger.Info(logEventInfo);
        //}

            /* fasdfasd */
        public void User(string message, params object[] args)
        {
            logger = _factory.GetLogger(args[0].ToString());
            var logEventInfo = new LogEventInfo(LogLevel.Info, args[0].ToString(), message);

            switch (args[0].ToString())
            {
                case "Login":

                    if (args.Length != 12) break;

                    logEventInfo.Properties["userID"] = args[1].ToString();
                    logEventInfo.Properties["nickName"] = args[2].ToString();
                    logEventInfo.Properties["UDID"] = args[3].ToString();
                    logEventInfo.Properties["ADID"] = args[4].ToString();
                    logEventInfo.Properties["userLevel"] = args[5].ToString();
                    logEventInfo.Properties["freeGold"] = args[6].ToString();
                    logEventInfo.Properties["paidGold"] = args[7].ToString();
                    logEventInfo.Properties["freeGem"] = args[8].ToString();
                    logEventInfo.Properties["paidGem"] = args[9].ToString();
                    logEventInfo.Properties["socialID"] = args[10].ToString();
                    logEventInfo.Properties["placeType"] = args[11].ToString();

                    logger.Info(logEventInfo);
                    break;

                case "Logout":

                    if (args.Length != 11) break;

                    logEventInfo.Properties["userID"] = args[1].ToString();
                    logEventInfo.Properties["nickName"] = args[2].ToString();
                    logEventInfo.Properties["UDID"] = args[3].ToString();
                    logEventInfo.Properties["ADID"] = args[4].ToString();
                    logEventInfo.Properties["userLevel"] = args[5].ToString();
                    logEventInfo.Properties["freeGold"] = args[6].ToString();
                    logEventInfo.Properties["paidGold"] = args[7].ToString();
                    logEventInfo.Properties["freeGem"] = args[8].ToString();
                    logEventInfo.Properties["paidGem"] = args[9].ToString();
                    logEventInfo.Properties["socialID"] = args[10].ToString();

                    logger.Info(logEventInfo);
                    break;
                case "createID":

                    if (args.Length != 9) break;

                    logEventInfo.Properties["userID"] = args[1].ToString();
                    logEventInfo.Properties["nickName"] = args[2].ToString();
                    logEventInfo.Properties["UDID"] = args[3].ToString();
                    logEventInfo.Properties["ADID"] = args[4].ToString();
                    logEventInfo.Properties["location"] = args[5].ToString();
                    logEventInfo.Properties["serviceProvider"] = args[6].ToString();
                    logEventInfo.Properties["OS"] = args[7].ToString();
                    logEventInfo.Properties["Language"] = args[8].ToString();
                    
                    logger.Info(logEventInfo);
                    break;
                case "dropID":

                    if (args.Length != 9) break;

                    logEventInfo.Properties["userID"] = args[1].ToString();
                    logEventInfo.Properties["nickName"] = args[2].ToString();
                    logEventInfo.Properties["UDID"] = args[3].ToString();
                    logEventInfo.Properties["ADID"] = args[4].ToString();
                    logEventInfo.Properties["location"] = args[5].ToString();
                    logEventInfo.Properties["serviceProvider"] = args[6].ToString();
                    logEventInfo.Properties["OS"] = args[7].ToString();
                    logEventInfo.Properties["Language"] = args[8].ToString();

                    logger.Info(logEventInfo);
                    break;
                default:
                    logger.Info(message);
                    break;

            }
        }

        public void Battle(string message, params object[] args)
        {
            logger = _factory.GetLogger(args[0].ToString());
            var logEventInfo = new LogEventInfo(LogLevel.Info, args[0].ToString(), message);

            switch (args[0].ToString())
            {
                case "matchRequest":

                    if (args.Length != 6) break;

                    logEventInfo.Properties["AreaNum"] = args[1].ToString();
                    logEventInfo.Properties["GroupNum"] = args[2].ToString();
                    logEventInfo.Properties["userID"] = args[3].ToString();
                    logEventInfo.Properties["nickName"] = args[4].ToString();
                    logEventInfo.Properties["RankPoint"] = args[5].ToString();

                    logger.Info(logEventInfo);
                    break;

                case "MatchCancle":

                    if (args.Length != 4) break;

                    logEventInfo.Properties["AreaNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["userID"] = args[3].ToString();

                    logger.Info(logEventInfo);
                    break;
                case "AIMatch":

                    if (args.Length != 6) break;

                    logEventInfo.Properties["AreaNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["userID"] = args[3].ToString();
                    logEventInfo.Properties["RankPoint"] = args[4].ToString();
                    logEventInfo.Properties["AIpatternID"] = args[5].ToString();

                    logger.Info(logEventInfo);
                    break;
                case "PVPMatch":

                    if (args.Length != 10) break;

                    logEventInfo.Properties["AreaNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["sourceUserID"] = args[3].ToString();
                    logEventInfo.Properties["sourceNickName"] = args[4].ToString();
                    logEventInfo.Properties["sourceRankpoint"] = args[5].ToString();
                    logEventInfo.Properties["targetUserID"] = args[6].ToString();
                    logEventInfo.Properties["targetNickName"] = args[7].ToString();
                    logEventInfo.Properties["targetRankpoint"] = args[8].ToString();
                    logEventInfo.Properties["RoomID"] = args[9].ToString();

                    logger.Info(logEventInfo);
                    break;
                case "battleEntry":

                    if (args.Length != 14) break;

                    logEventInfo.Properties["BattleServerNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["userID"] = args[3].ToString();
                    logEventInfo.Properties["nickName"] = args[4].ToString();
                    logEventInfo.Properties["Rankpoint"] = args[5].ToString();
                    logEventInfo.Properties["GameType"] = args[6].ToString();
                    logEventInfo.Properties["GameStartTime"] = args[7].ToString();
                    logEventInfo.Properties["unit1"] = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("unitID", args[8].ToString()),
                        new KeyValuePair<string, object>("Nickname", args[9].ToString())
                    };
                    logEventInfo.Properties["unit2"] = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("unitID", args[10].ToString()),
                        new KeyValuePair<string, object>("Nickname", args[11].ToString())
                    };
                    logEventInfo.Properties["unit3"] = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("unitID", args[12].ToString()),
                        new KeyValuePair<string, object>("Nickname", args[13].ToString())
                    };

                    logger.Info(logEventInfo);
                    break;
                case "NomalAttack":

                    if (args.Length != 9) break;

                    logEventInfo.Properties["BattleServerNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["sourceUserID"] = args[3].ToString();
                    logEventInfo.Properties["sourceNickname"] = args[4].ToString();
                    logEventInfo.Properties["attackerUnitID"] = args[5].ToString();
                    logEventInfo.Properties["targetUserID"] = args[6].ToString();
                    logEventInfo.Properties["targetNickname"] = args[7].ToString();
                    

                    logger.Info(logEventInfo);
                    break;
                case "NomalAttackResult":

                    //Buff attackerbuff = new Buff
                    //{
                    //    buffSkillID = args[1].ToString(),
                    //    buffcondition = args[2].ToString(),
                    //};
                    if (args.Length != 12) break;

                    logEventInfo.Properties["BattleServerNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["sourceUserID"] = args[3].ToString();
                    logEventInfo.Properties["sourceNickname"] = args[4].ToString();
                    logEventInfo.Properties["attackerUnitID"] = args[5].ToString();
                    logEventInfo.Properties["targetUserID"] = args[6].ToString();
                    logEventInfo.Properties["targetNickname"] = args[7].ToString();
                    //logEventInfo.Properties["AttackBufflist"] = attackerbuff;
                    //logEventInfo.Properties["AttackDebufflist"] = attackerbuff;
                    //logEventInfo.Properties["TargetBufflist"] = attackerbuff;
                    //logEventInfo.Properties["TargetDebufflist"] = attackerbuff;
                    logEventInfo.Properties["AttackPoint"] = args[8].ToString();
                    logEventInfo.Properties["TargetBeforeHP"] = args[9].ToString();
                    logEventInfo.Properties["TargetAfterHP"] = args[10].ToString();
                    logEventInfo.Properties["TargetDown"] = args[11].ToString();

                    logger.Info(logEventInfo);
                    break;
                case "BattleResult":

                    if (args.Length != 12) break;

                    logEventInfo.Properties["BattleServerNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["UserID"] = args[3].ToString();
                    logEventInfo.Properties["Nickname"] = args[4].ToString();
                    logEventInfo.Properties["battleResult"] = args[5].ToString();
                    logEventInfo.Properties["beforeBoxEnergy"] = args[6].ToString();
                    logEventInfo.Properties["inputBoxEnergy"] = args[7].ToString();
                    logEventInfo.Properties["afterBoxEnergy"] = args[8].ToString();
                    logEventInfo.Properties["getGold"] = args[9].ToString();
                    logEventInfo.Properties["beforeGold"] = args[10].ToString();
                    logEventInfo.Properties["afterGold"] = args[11].ToString();
                    

                    logger.Info(logEventInfo);
                    break;
                case "ReEntry":

                    
                    if (args.Length != 12) break;

                    logEventInfo.Properties["BattleServerNum"] = args[1].ToString();
                    logEventInfo.Properties["RoomID"] = args[2].ToString();
                    logEventInfo.Properties["UserID"] = args[3].ToString();
                    logEventInfo.Properties["Nickname"] = args[4].ToString();
                    logEventInfo.Properties["gameType"] = args[5].ToString();
                    logEventInfo.Properties["beforeBoxEnergy"] = args[6].ToString();
                    logEventInfo.Properties["inputBoxEnergy"] = args[7].ToString();
                    logEventInfo.Properties["afterBoxEnergy"] = args[8].ToString();
                    logEventInfo.Properties["getGold"] = args[9].ToString();
                    logEventInfo.Properties["beforeGold"] = args[10].ToString();
                    logEventInfo.Properties["afterGold"] = args[11].ToString();
                    

                    logger.Info(logEventInfo);
                    break;
                default:
                    logger.Info(message);
                    break;

            }
        }


        public void Dispose()
        {
            _factory.Dispose();
        }
    }

}
