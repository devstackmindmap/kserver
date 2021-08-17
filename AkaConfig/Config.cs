using AkaEnum;
using AkaThreading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace AkaConfig
{
    public class Config
    {
        public static string RunMode;
        public static Server Server;
        private static object _reloadGameServerConfigLock = new object();

        public static CommonServerConfig CommonServerConfig;
        public static GameServerConfig GameServerConfig;
        public static MatchingServerConfig MatchingServerConfig;
        public static BattleServerConfig BattleServerConfig;
        public static PubSubServerConfig PubSubServerConfig;
        public static TriggerServerConfig TriggerServerConfig;

        public static string ConfigRootPath;

        public static void CommonServerInitConfig(Server server, string runmode)
        {
            if (CommonServerConfig != null)
                return;

            Server = server;
            RunMode = runmode;
            CommonServerConfig = GetConfigFromJson<CommonServerConfig>(runmode);
        }

        public static void GameServerInitConfig(Server server, IConfiguration configuration)
        {
            GameServerInitConfig(server, configuration.GetValue<string>("ENVIRONMENT"));
        }

        public static void GameServerInitConfig(Server server, string runmode)
        {
            CommonServerInitConfig(server, runmode);
            GameServerConfig = GetConfigFromJson<GameServerConfig>(runmode);

            SemaphoreManager.Add(
                SemaphoreType.GameServer2GameRedisServerBalancer,
                Config.GameServerConfig.GameRedisSetting.ServerSetting.IOThrottling.InitialCount,
                Config.GameServerConfig.GameRedisSetting.ServerSetting.IOThrottling.MaxCount);
        }

        public static void GameServerReloadConfig()
        {
            lock(_reloadGameServerConfigLock)
            {
                GameServerConfig = GetConfigFromJson<GameServerConfig>(RunMode);
            }
        }

        public static void MatchingServerInitConfig(Server server, string runmode)
        {
            CommonServerInitConfig(server, runmode);
            MatchingServerConfig = GetConfigFromJson<MatchingServerConfig>(runmode);

            SemaphoreManager.Add(
                SemaphoreType.MatchServer2GameServerRequestBalancer, 
                Config.MatchingServerConfig.GameServer.IOThrottling.InitialCount,
                Config.MatchingServerConfig.GameServer.IOThrottling.MaxCount);

            SemaphoreManager.Add(
                SemaphoreType.MatchServer2MatchingRedisServerBalancer,
                Config.MatchingServerConfig.MatchingRedisSetting.ServerSetting.IOThrottling.InitialCount,
                Config.MatchingServerConfig.MatchingRedisSetting.ServerSetting.IOThrottling.MaxCount);
        }

        public static void BattleServerInitConfig(Server server, string runmode)
        {
            CommonServerInitConfig(server, runmode);
            BattleServerConfig = GetConfigFromJson<BattleServerConfig>(runmode);

            SemaphoreManager.Add(
                SemaphoreType.BattleServer2GameServerBalancer,
                Config.BattleServerConfig.GameServer.IOThrottling.InitialCount,
                Config.BattleServerConfig.GameServer.IOThrottling.MaxCount);

            SemaphoreManager.Add(
                SemaphoreType.BattleServer2GameRedisServerBalancer, 
                Config.BattleServerConfig.GameRedisSetting.ServerSetting.IOThrottling.InitialCount,
                Config.BattleServerConfig.GameRedisSetting.ServerSetting.IOThrottling.MaxCount);
        }

        public static void PubSubServerInitConfig(Server server, string runmode)
        {
            CommonServerInitConfig(server, runmode);
            PubSubServerConfig = GetConfigFromJson<PubSubServerConfig>(runmode);

            SemaphoreManager.Add(
                SemaphoreType.PubsubServer2GameRedisServerBalancer,
                Config.PubSubServerConfig.PubSubRedisSetting.ServerSetting.IOThrottling.InitialCount,
                Config.PubSubServerConfig.PubSubRedisSetting.ServerSetting.IOThrottling.MaxCount);

            SemaphoreManager.Add(
                SemaphoreType.PubsubServer2ClientSocketBalancer,
                Config.PubSubServerConfig.SendSocketIOThrottling.InitialCount,
                Config.PubSubServerConfig.SendSocketIOThrottling.MaxCount);

            
        }

        public static void TriggerServerInitConfig(Server server, string runmode)
        {
            CommonServerInitConfig(server, runmode);
            GameServerConfig = GetConfigFromJson<GameServerConfig>(runmode);
            TriggerServerConfig = GetConfigFromJson<TriggerServerConfig>(runmode);
        }

        private static T GetConfigFromJson<T>(string runmode)
        {
            var jsonData = GetJsonFile(runmode, typeof(T).Name);
            return jsonData.ToObject<T>();
        }

        private static JObject GetJsonFile(string runmode, string configFileName)
        {
            try
            {
                return GetJsonFileBase(runmode, configFileName);
            }
            catch (Exception e)
            {
                return GetJsonFileNotBase(runmode, configFileName);
            }
        }

        private static JObject GetJsonFileBase(string runmode, string configFileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                while (true)
                {
                    var configPath = $@"{basePath}/Config/Server/{runmode}/{configFileName}.json";
                    if (System.IO.File.Exists(configPath))
                    {
                        ConfigRootPath = basePath + "/Config/";
                        return JObject.Parse(File.ReadAllText(String.Format(configPath)));
                    }
                    basePath = System.IO.Directory.GetParent(basePath).FullName;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private static JObject GetJsonFileNotBase(string runmode, string configFileName)
        {
            string configPath = $"../../../Config/Server/{runmode}/{configFileName}.json";
            try
            {
                return JObject.Parse(File.ReadAllText(String.Format(configPath)));
            }
            catch (DirectoryNotFoundException e2)
            {
                if (runmode == "Dylan2")
                {
                    configPath = @"D:/Projects/KnightRun/kserver/Config/Server/Dylan2/{0}.json";
                    return JObject.Parse(File.ReadAllText(String.Format(configPath, configFileName)));
                }
                else if (runmode == "Machance")
                {
                    configPath = @"D:/Git/kserver/Config/Server/Machance/{0}.json";
                    var jsonData = JObject.Parse(File.ReadAllText((String.Format(configPath, configFileName))));
                }
                else if (runmode == "Bongpalman")
                {
                    configPath = @"D:/git/Kserver/Config/Server/Bongpalman/{0}.json";
                    return JObject.Parse(File.ReadAllText(String.Format(configPath, configFileName)));
                }
                else
                {
                    //for Testcase Project
                    configPath = @"../../../../Config/Server/{0}/{1}.json";
                    return JObject.Parse(File.ReadAllText(String.Format(configPath, runmode, configFileName)));
                }
            }

            throw new Exception("Open Fail, Json Config File");
        }

        public static void InitConfigInAbsolutePath(string absolutePath)
        {
            var gameServerPath = $"{absolutePath}\\GameServerConfig.json";
            var commonServerPath = $"{absolutePath}\\CommonServerConfig.json";
            var battleServerPath = $"{absolutePath}\\BattleServerConfig.json";

            var jsonData = JObject.Parse(File.ReadAllText(gameServerPath));
            GameServerConfig = jsonData.ToObject<GameServerConfig>();

            jsonData = JObject.Parse(File.ReadAllText(commonServerPath));
            CommonServerConfig = jsonData.ToObject<CommonServerConfig>();

            jsonData = JObject.Parse(File.ReadAllText(battleServerPath));
            BattleServerConfig = jsonData.ToObject<BattleServerConfig>();
        }

        public static void AllServerInitConfig(string runmode, Server server = Server.GameServer)
        {
            GameServerInitConfig(server, runmode);
            MatchingServerInitConfig(server, runmode);
            BattleServerInitConfig(server, runmode);
        }

        public static int GetShardNum(uint userId)
        {
            var shardNum = (int)((userId - 1) / GameServerConfig.GameDBSetting.UserDBShardRange);
            return userId == 0 ? 0 : shardNum;
        }
    }
}
