using AkaConfig;
using SuperSocket.SocketBase;
using System;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Config;
using AkaData;
using AkaEnum;
using AkaLogger;
using BattleServer.BattleRecord;
using System.Diagnostics;
using AkaThreading;

namespace BattleServer
{
    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);
        static string runMode;

        static async Task Main(string[] args)
        {
            AntiDuplicator.AppRunning();

            DomainExceptionHandler.Initialize();
            runMode = args[0];
            var buildVersion = Versioning.Version;
            Console.WriteLine($"Battle Server {buildVersion} Start( exit: enter press 'q')");

            Config.BattleServerInitConfig(Server.BattleServer, runMode);

            Console.CancelKeyPress += OnConsoleKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            var dataVersion = args.Length >= 2 ? Int32.Parse(args[1]) : 0;
            await Initialize(dataVersion);

            var ServerConfig = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = Config.BattleServerConfig.BattleServerPort,
                ListenBacklog = Config.BattleServerConfig.ServerConfig.ListenBacklog,
                KeepAliveTime = Config.BattleServerConfig.ServerConfig.KeepAliveTime,
                MaxConnectionNumber = Config.BattleServerConfig.ServerConfig.MaxConnectionNumber,
                ReceiveBufferSize = Config.BattleServerConfig.ServerConfig.ReceiveBufferSize,
                MaxRequestLength = Config.BattleServerConfig.ServerConfig.MaxRequestLength,
                SendBufferSize = Config.BattleServerConfig.ServerConfig.SendBufferSize,
                SendingQueueSize = Config.BattleServerConfig.ServerConfig.SendingQueueSize,
                LogAllSocketException = Config.BattleServerConfig.ServerConfig.LogAllSocketException,
                LogBasicSessionActivity = Config.BattleServerConfig.ServerConfig.LogBasicSessionActivity,
                LogCommand = Config.BattleServerConfig.ServerConfig.LogCommand,
                ClearIdleSession = Config.BattleServerConfig.ServerConfig.ClearIdleSession,
                ClearIdleSessionInterval = Config.BattleServerConfig.ServerConfig.ClearIdleSessionInterval,
                IdleSessionTimeOut = Config.BattleServerConfig.ServerConfig.IdleSessionTimeOut,
                SyncSend = Config.BattleServerConfig.ServerConfig.SyncSend,
                Mode = SocketMode.Tcp,
            };

            var maxCompletionPortThreads
                = Config.BattleServerConfig.RootConfig.MultiCount == 0 ?
                Config.BattleServerConfig.RootConfig.MaxCompletionPortThreads :
                Environment.ProcessorCount * Config.BattleServerConfig.RootConfig.MultiCount;

            var maxWorkingThreads
                = Config.BattleServerConfig.RootConfig.MultiCount == 0 ?
                Config.BattleServerConfig.RootConfig.MaxWorkingThreads :
                Environment.ProcessorCount * Config.BattleServerConfig.RootConfig.MultiCount;

            var RootConfig = new RootConfig
            {
          //      MaxCompletionPortThreads = maxCompletionPortThreads,
          //      MaxWorkingThreads = maxWorkingThreads,
                DisablePerformanceDataCollector = Config.BattleServerConfig.RootConfig.DisablePerformanceDataCollector,
          //      MinWorkingThreads = Config.BattleServerConfig.RootConfig.MinWorkingThreads
            };

            if (!MainServer.Instance.Setup(rootConfig: RootConfig, config: ServerConfig))
            {
                Console.WriteLine("Setup Error");
            }
            MainServer.Instance.Start();
            //BattleRecordStorage.Instance.Start();

            terminatingEvent.WaitOne();

            AkaLogger.Logger.Instance().Dispose();
            BattleRecordStorage.Instance.Dispose();
            MainServer.Instance.Dispose();

            DomainExceptionHandler.Unintialize();
        }

        static async Task Initialize(int dataVersion)
        {
            AkaRedis.AkaRedis.AddServer(Config.Server, Config.BattleServerConfig.GameRedisSetting.ServerSetting,
                Config.BattleServerConfig.GameRedisSetting.Password);
            var loader = new FileLoader(FileType.Table, runMode, dataVersion);
            var fileList = await loader.GetFileLists();
            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);
        }
        
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            terminatingEvent.Set();
        }

        static void OnConsoleKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                e.Cancel = true;
                terminatingEvent.Set();
            }
        }
    }
}
