using AkaConfig;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace AkaDB
{
    public class DBEnv
    {
        public static SortedDictionary<string, SortedDictionary<int, MySqlConnectionStringBuilder>> SettingBuilderMap
            = new SortedDictionary<string, SortedDictionary<int, MySqlConnectionStringBuilder>>();

        public static void AllSetUp()
        {
            SetUp("AccountDBSetting", 0, Config.GameServerConfig.GameDBSetting.AccountDBSetting);

            foreach (var shard in Config.GameServerConfig.GameDBSetting.UserDBSetting)
            {
                SetUp("UserDBSetting", shard.Key, shard.Value);
            }
        }

        public static void SetUp(string dbVariety, int shardNum, DBConfig dbConfigMap)
        {
            if (SettingBuilderMap.ContainsKey(dbVariety) == false)
                SettingBuilderMap.Add(dbVariety, new SortedDictionary<int, MySqlConnectionStringBuilder>());

            if (SettingBuilderMap[dbVariety].ContainsKey(shardNum) == false)
                SettingBuilderMap[dbVariety].Add(shardNum, new MySqlConnectionStringBuilder());

            var stringBuilder = new MySqlConnectionStringBuilder();
            stringBuilder.Port = (uint)dbConfigMap.port;
            stringBuilder.Server = dbConfigMap.host;
            stringBuilder.Database = dbConfigMap.database;
            stringBuilder.UserID = dbConfigMap.user;
            stringBuilder.Password = dbConfigMap.password;
            stringBuilder.Pooling = true;
            stringBuilder.MinimumPoolSize = 10;
            stringBuilder.MaximumPoolSize = 100;
            stringBuilder.SslMode = MySqlSslMode.None;
            stringBuilder.CharacterSet = dbConfigMap.charset;

            SettingBuilderMap[dbVariety][shardNum] =  stringBuilder;
        }
    }
}
