using AkaConfig;
using System;
using System.Threading;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase;
using AkaData;
using System.Threading.Tasks;
using AkaEnum;
using AkaLogger;
using System.Diagnostics;
using AkaThreading;

namespace MatchingServer
{
    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);
        static string runMode;

        static async Task Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Wrong args length ");
                return;
            }

            var areaIndex = int.Parse(args[2]);
            var matchingLine = int.Parse(args[3]);

            AntiDuplicator.AppRunning(matchingLine);

            DomainExceptionHandler.Initialize();
            runMode = args[0];
            var buildVersion = Versioning.Version;
            Console.WriteLine($"Matching Server {buildVersion} Start( exit: enter press 'q')");
            Config.MatchingServerInitConfig(Server.MatchingServer, runMode);

            var dataVersion = Int32.Parse(args[1]);
            await Initialize(dataVersion);

            Console.CancelKeyPress += OnConsoleKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            var ServerConfig = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = Config.MatchingServerConfig.MatchingServerList[areaIndex][matchingLine].port,
                ListenBacklog = Config.MatchingServerConfig.ServerConfig.ListenBacklog,
                KeepAliveTime = Config.MatchingServerConfig.ServerConfig.KeepAliveTime,
                MaxConnectionNumber = Config.MatchingServerConfig.ServerConfig.MaxConnectionNumber,
                ReceiveBufferSize = Config.MatchingServerConfig.ServerConfig.ReceiveBufferSize,
                MaxRequestLength = Config.MatchingServerConfig.ServerConfig.MaxRequestLength,
                SendBufferSize = Config.MatchingServerConfig.ServerConfig.SendBufferSize,
                SendingQueueSize = Config.MatchingServerConfig.ServerConfig.SendingQueueSize,
                LogAllSocketException = Config.MatchingServerConfig.ServerConfig.LogAllSocketException,
                LogBasicSessionActivity = Config.MatchingServerConfig.ServerConfig.LogBasicSessionActivity,
                LogCommand = Config.MatchingServerConfig.ServerConfig.LogCommand,
                ClearIdleSession = Config.MatchingServerConfig.ServerConfig.ClearIdleSession,
                ClearIdleSessionInterval = Config.MatchingServerConfig.ServerConfig.ClearIdleSessionInterval,
                IdleSessionTimeOut = Config.MatchingServerConfig.ServerConfig.IdleSessionTimeOut,
                SyncSend = Config.MatchingServerConfig.ServerConfig.SyncSend,
                Mode = SocketMode.Tcp,
            };

            var maxCompletionPortThreads
                = Config.MatchingServerConfig.RootConfig.MultiCount == 0 ?
                Config.MatchingServerConfig.RootConfig.MaxCompletionPortThreads :
                Environment.ProcessorCount * Config.MatchingServerConfig.RootConfig.MultiCount;

            var maxWorkingThreads
                = Config.MatchingServerConfig.RootConfig.MultiCount == 0 ?
                Config.MatchingServerConfig.RootConfig.MaxWorkingThreads :
                Environment.ProcessorCount * Config.MatchingServerConfig.RootConfig.MultiCount;

            var RootConfig = new RootConfig
            {
                MaxCompletionPortThreads = maxCompletionPortThreads,
                MaxWorkingThreads = maxWorkingThreads,
                DisablePerformanceDataCollector = Config.MatchingServerConfig.RootConfig.DisablePerformanceDataCollector,
                MinWorkingThreads = Config.MatchingServerConfig.RootConfig.MinWorkingThreads
            };

            MainServer.Instance.InitMatchingLine(areaIndex, matchingLine);
            if (!MainServer.Instance.Setup(rootConfig: RootConfig, config: ServerConfig))
            {
                Console.WriteLine("Setup Error");
            }
            
            MainServer.Instance.Start();

            terminatingEvent.WaitOne();
            AkaLogger.Logger.Instance().Dispose();
            MainServer.Instance.Dispose();
            DomainExceptionHandler.Unintialize();
        }

        static async Task Initialize(int dataVersion)
        {
            AkaRedis.AkaRedis.AddServer(Config.Server, Config.MatchingServerConfig.MatchingRedisSetting.ServerSetting,
                Config.MatchingServerConfig.MatchingRedisSetting.Password);
            var loader = new FileLoader(FileType.Table, runMode, dataVersion);
            var fileList = await loader.GetFileLists();
            DataSetter dataSetter = new DataSetter();
            dataSetter.DataSet(fileList);
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            terminatingEvent.Set();
            MainServer.Instance.EndingMainProcess = true;
            MainServer.Instance.RemoveMatchingLine();
        }

        static void OnConsoleKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                e.Cancel = true;
                terminatingEvent.Set();
                MainServer.Instance.EndingMainProcess = true;
                MainServer.Instance.RemoveMatchingLine();
            }
        }
    }
}
