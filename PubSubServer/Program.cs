using AkaConfig;
using SuperSocket.SocketBase;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Config;
using AkaEnum;
using AkaThreading;

namespace PubSubServer
{

    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);
        static string runMode;

        static async Task Main(string[] args)
        {
            AntiDuplicator.AppRunning();

            runMode = args[0];
            var buildVersion = Versioning.Version;
            Console.WriteLine($"PubSub Server {buildVersion} Start( exit: enter press 'q')");

            Config.PubSubServerInitConfig(Server.PubSubServer, runMode);

            Console.CancelKeyPress += OnConsoleKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            await Initialize();

            var ServerConfig = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = Config.PubSubServerConfig.PubSubServerPort,
                ListenBacklog = Config.PubSubServerConfig.ServerConfig.ListenBacklog,
                KeepAliveTime = Config.PubSubServerConfig.ServerConfig.KeepAliveTime,
                MaxConnectionNumber = Config.PubSubServerConfig.ServerConfig.MaxConnectionNumber,
                ReceiveBufferSize = Config.PubSubServerConfig.ServerConfig.ReceiveBufferSize,
                MaxRequestLength = Config.PubSubServerConfig.ServerConfig.MaxRequestLength,
                SendBufferSize = Config.PubSubServerConfig.ServerConfig.SendBufferSize,
                SendingQueueSize = Config.PubSubServerConfig.ServerConfig.SendingQueueSize,
                LogAllSocketException = Config.PubSubServerConfig.ServerConfig.LogAllSocketException,
                LogBasicSessionActivity = Config.PubSubServerConfig.ServerConfig.LogBasicSessionActivity,
                LogCommand = Config.PubSubServerConfig.ServerConfig.LogCommand,
                ClearIdleSession = Config.PubSubServerConfig.ServerConfig.ClearIdleSession,
                ClearIdleSessionInterval = Config.PubSubServerConfig.ServerConfig.ClearIdleSessionInterval,
                IdleSessionTimeOut = Config.PubSubServerConfig.ServerConfig.IdleSessionTimeOut,
                SyncSend = Config.PubSubServerConfig.ServerConfig.SyncSend,
                Mode = SocketMode.Tcp,
            };

            var maxCompletionPortThreads
                = Config.PubSubServerConfig.RootConfig.MultiCount == 0 ?
                Config.PubSubServerConfig.RootConfig.MaxCompletionPortThreads :
                Environment.ProcessorCount * Config.PubSubServerConfig.RootConfig.MultiCount;

            var maxWorkingThreads
                = Config.PubSubServerConfig.RootConfig.MultiCount == 0 ?
                Config.PubSubServerConfig.RootConfig.MaxWorkingThreads :
                Environment.ProcessorCount * Config.PubSubServerConfig.RootConfig.MultiCount;

            var RootConfig = new RootConfig
            {
                MaxCompletionPortThreads = maxCompletionPortThreads,
                MaxWorkingThreads = maxWorkingThreads,
                DisablePerformanceDataCollector = Config.PubSubServerConfig.RootConfig.DisablePerformanceDataCollector,
                MinWorkingThreads = Config.PubSubServerConfig.RootConfig.MinWorkingThreads
            };

            if (!MainServer.Instance.Setup(rootConfig: RootConfig, config: ServerConfig))
            {
                Console.WriteLine("Setup Error");
            }
            MainServer.Instance.Start();

            terminatingEvent.WaitOne();

            MainServer.Instance.Dispose();
                            
        }

        static async Task Initialize()
        {
            AkaRedis.AkaRedis.AddServer(Config.Server, Config.PubSubServerConfig.PubSubRedisSetting.ServerSetting,
                Config.PubSubServerConfig.PubSubRedisSetting.Password);
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
