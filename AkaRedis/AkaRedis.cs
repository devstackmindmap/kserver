using AkaConfig;
using AkaEnum;
using AkaThreading;
using AkaUtility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace AkaRedis
{
    public class AkaRedis : IDisposable
    {
        private static int defaultDatabase = 0;
        public static ConnectionMultiplexer Connector;

        public static Dictionary<Server, ConnectionMultiplexer> Connectors = new Dictionary<Server, ConnectionMultiplexer>(ServerComparer.Comparer);

        public static void AddServer(Server server, ServerSetting serverSetting, string password)
        {
            var configurationOptions = new ConfigurationOptions
            {
                DefaultDatabase = defaultDatabase
            };

            if (password?.Length > 0)
            {
                configurationOptions.Password = password;
            }

            configurationOptions.EndPoints.Add(serverSetting.ip, serverSetting.port);
            try
            {
                Connectors.Add(server, ConnectionMultiplexer.Connect(configurationOptions));
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        public static void AllSetUp()
        {
            if (Config.RunMode == "Live")
                throw new Exception("This Job is execute for not live");

            AddServer(Server.GameServer, 
                Config.GameServerConfig.GameRedisSetting.ServerSetting, 
                Config.GameServerConfig.GameRedisSetting.Password);

            AddServer(Server.MatchingServer,
                Config.MatchingServerConfig.MatchingRedisSetting.ServerSetting,
                Config.MatchingServerConfig.MatchingRedisSetting.Password);

            AddServer(Server.BattleServer,
                Config.BattleServerConfig.GameRedisSetting.ServerSetting,
                Config.BattleServerConfig.GameRedisSetting.Password);
        }

        public static IDatabase GetDatabase()
        {
            return GetDatabase(Config.Server);
        }

        public static IDatabase GetDatabase(Server server)
        {
            return Connectors[server].GetDatabase(defaultDatabase);
        }

        public static ISubscriber GetSubscriber()
        {
            return Connectors[Config.Server].GetSubscriber();
        }

        public static IServer GetServer()
        {
            var endPoints = Connectors[Config.Server].GetEndPoints();
            if (endPoints.Length > 0)
                return Connectors[Config.Server].GetServer(endPoints[0]);

            return null;
        }

        public void Dispose()
        {
            foreach (var connector in Connectors)
            {
                connector.Value.Dispose();
            }

            Connector.Dispose();
        }

        public static bool ConnectCheck(Server server)
        {
            if (AkaRedis.Connectors[server] == null)
                return false;

            return true;
        }

        public static SemaphoreType GetBattlePlayingInfoSemaphoreType()
        {
            if (Config.Server == Server.GameServer)
                return SemaphoreType.GameServer2GameRedisServerBalancer;
            else if (Config.Server == Server.MatchingRedisServer)
                return SemaphoreType.MatchServer2MatchingRedisServerBalancer;
            else
                return SemaphoreType.BattleServer2GameRedisServerBalancer;
        }
    }
}
