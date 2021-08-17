using System;
using System.Collections.Generic;
using System.Text;

namespace AkaConfig
{
    public class CommonServerConfig
    {
        public string DownloadUrl;
    }

    public class GameServerConfig
    {
        public int GameServerPort;
        public int BattleServerPort;
        public int PubSubServerPort;
        public string RecordStoragePath;
        public ServerSetting MatchingServer;
        public GameDBSetting GameDBSetting;
        public RedisSetting GameRedisSetting;
        public ServerSetting WebPubServer;
        public ServerSetting TriggerServer;
        public IDictionary<int, IDictionary<int, ServerSettingIpPort>> MatchingServerList;
        public string InAppUrl;
    }

    public class MatchingServerConfig
    {
        public IDictionary<int, IDictionary<int, ServerSettingIpPort>> MatchingServerList;
        public int BaseDelaySessionCount;
        public ServerSetting GameServer;
        public RedisSetting MatchingRedisSetting;
        public SuperSocketServerConfig ServerConfig;
        public SuperSocketRootConfig RootConfig;
    }

    public class BattleServerConfig
    {
        public int BattleServerPort;
        public ServerSetting GameServer;
        public RedisSetting GameRedisSetting;
        public SuperSocketServerConfig ServerConfig;
        public SuperSocketRootConfig RootConfig;
        public RecordSetting RecordSetting;
    }

    public class PubSubServerConfig
    {
        public int PubSubServerPort;
        public RedisSetting PubSubRedisSetting;
        public IOThrottling SendSocketIOThrottling;
        public SuperSocketServerConfig ServerConfig;
        public SuperSocketRootConfig RootConfig;
    }

    public class TriggerServerConfig
    {
        public int TriggerServerPort;
        public int ServerInfoCheckInterval;
        public ServerSetting TriggerPubServer;
        public SuperSocketServerConfig ServerConfig;
        public SuperSocketRootConfig RootConfig;
        public ServerList BattleServerList;
        public ServerList PubSubServerList;
        public RedisSetting TriggerRedisSetting;
    }

    public class ServerSettingIpPort
    {
        public string ip;
        public int port;
    }

    public class ServerSetting
    {
        public string ip;
        public int port;
        public int tryReconnectTime; 
        public IOThrottling IOThrottling;
    }

    public class RedisSetting
    {
        public ServerSetting ServerSetting;
        public int DefaultDatabase;
        public string Password;
    }

    public class IOThrottling
    {
        public int InitialCount;
        public int MaxCount;
    }

    public class GameDBSetting
    {
        public int UserDBShardRange;
        public DBConfig AccountDBSetting;
        public IDictionary<int, DBConfig> UserDBSetting;
    }

    public class RecordSetting
    {
        public bool UseAWS;
        public string AWSProfile;
        public string AWSCredential;
        public string AWSRegion;
        public string AWSBucket;
        public string LocalStoragePath;
    }

    public class ServerList
    {
        public int port;
        public int tryConnectionWaitingTime;
        public IDictionary<int, List<string>> countryIps;
    }

    public class MatchingServerList
    {
        public IDictionary<int, MatchingLine> MatchingLines;
    }

    public class DBConfig
    {
        public string user;
        public string password;
        public string database;
        public string host;
        public int port;
        public string charset;
    }

    public class SuperSocketServerConfig
    {
        public int ListenBacklog;
        public int KeepAliveTime;
        public int MaxConnectionNumber;
        public int ReceiveBufferSize;
        public int MaxRequestLength;
        public int SendBufferSize;
        public int SendingQueueSize;
        public bool LogAllSocketException;
        public bool LogBasicSessionActivity;
        public bool LogCommand;
        public bool ClearIdleSession;
        public int ClearIdleSessionInterval;
        public int IdleSessionTimeOut;
        public bool SyncSend;
    }

    public class SuperSocketRootConfig
    {
        public int MaxCompletionPortThreads;
        public bool DisablePerformanceDataCollector;
        public int MinWorkingThreads;
        public int MaxWorkingThreads;
        public int MultiCount;
    }

    public class MatchingLine
    {
        public IDictionary<int, ServerSettingIpPort> serverSetting;
    }

}
