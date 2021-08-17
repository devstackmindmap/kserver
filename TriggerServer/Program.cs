using AkaConfig;
using SuperSocket.SocketBase;
using System;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Config;
using AkaEnum;
using AkaData;
using TriggerServer.Managers;
using System.Collections.Generic;
using AkaDB;
using AkaLogger;
using AkaThreading;

namespace TriggerServer
{
    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);
        static string runMode;

        static void Main(string[] args)
        {
            DomainExceptionHandler.Initialize();
            runMode = args[0];

            var port = 0;
            var dataVersion = 0;
            var jobList = new List<string>();   //servercheck,square
            for (int i = 1; i < args.Length; i++)
            {
                var arg = args[i].Trim().ToLower();
                if (arg == "-p" && args.Length > i+1 && int.TryParse(args[i+1],out var argPort))
                {
                    port = argPort;
                    i++;
                }
                else if (arg == "-d" && args.Length > i + 1 && int.TryParse(args[i + 1], out var argDataVersion))
                {
                    dataVersion = argDataVersion;
                    i++;
                }
                else if (arg == "-f")
                {
                    i++;
                    for ( ; i < args.Length; i++)
                    {
                        var job = args[i].Trim().ToLower();
                        if (job[0] != '-')
                            jobList.Add(job);
                        else
                        {
                            i--;
                            break;
                        }
                    }
                }
            }

            AntiDuplicator.AppRunning(port);
            
            var buildVersion = Versioning.Version;
            Config.TriggerServerInitConfig(Server.TriggerServer, runMode);
            if (port == 0)
                port = Config.TriggerServerConfig.TriggerServerPort;


            Console.WriteLine($"Trigger Server {buildVersion} p:{port} d:{dataVersion}  Start( exit: enter press 'q')");

            DBEnv.AllSetUp();

            Console.CancelKeyPress += OnConsoleKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            Initialize(dataVersion).Wait();

            var ServerConfig = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = port,
                ListenBacklog = Config.TriggerServerConfig.ServerConfig.ListenBacklog,
                KeepAliveTime = Config.TriggerServerConfig.ServerConfig.KeepAliveTime,
                MaxConnectionNumber = Config.TriggerServerConfig.ServerConfig.MaxConnectionNumber,
                ReceiveBufferSize = Config.TriggerServerConfig.ServerConfig.ReceiveBufferSize,
                MaxRequestLength = Config.TriggerServerConfig.ServerConfig.MaxRequestLength,
                SendBufferSize = Config.TriggerServerConfig.ServerConfig.SendBufferSize,
                SendingQueueSize = Config.TriggerServerConfig.ServerConfig.SendingQueueSize,
                LogAllSocketException = Config.TriggerServerConfig.ServerConfig.LogAllSocketException,
                LogBasicSessionActivity = Config.TriggerServerConfig.ServerConfig.LogBasicSessionActivity,
                LogCommand = Config.TriggerServerConfig.ServerConfig.LogCommand,
                ClearIdleSession = Config.TriggerServerConfig.ServerConfig.ClearIdleSession,
                ClearIdleSessionInterval = Config.TriggerServerConfig.ServerConfig.ClearIdleSessionInterval,
                IdleSessionTimeOut = Config.TriggerServerConfig.ServerConfig.IdleSessionTimeOut,
                SyncSend = Config.TriggerServerConfig.ServerConfig.SyncSend,
                Mode = SocketMode.Tcp,
            };

            var maxCompletionPortThreads
                = Config.TriggerServerConfig.RootConfig.MultiCount == 0 ?
                Config.TriggerServerConfig.RootConfig.MaxCompletionPortThreads :
                Environment.ProcessorCount * Config.TriggerServerConfig.RootConfig.MultiCount;

            var maxWorkingThreads
                = Config.TriggerServerConfig.RootConfig.MultiCount == 0 ?
                Config.TriggerServerConfig.RootConfig.MaxWorkingThreads :
                Environment.ProcessorCount * Config.TriggerServerConfig.RootConfig.MultiCount;

            var RootConfig = new RootConfig
            {
                MaxCompletionPortThreads = maxCompletionPortThreads,
                MaxWorkingThreads = maxWorkingThreads,
                DisablePerformanceDataCollector = Config.TriggerServerConfig.RootConfig.DisablePerformanceDataCollector,
                MinWorkingThreads = Config.TriggerServerConfig.RootConfig.MinWorkingThreads
            };

            using (var scheduleManagerFactory = new ScheduleManagerFactory())
            {
                scheduleManagerFactory.Initialize(jobList).Wait();

                if (!MainServer.Instance.Setup(rootConfig: RootConfig, config: ServerConfig))
                {
                    Console.WriteLine("Setup Error");
                }
                MainServer.Instance.Start();
                //          Console.WriteLine("Setup Error" + pipe.CanRead.ToString());

                scheduleManagerFactory.Start();

                terminatingEvent.WaitOne();

            }
            AkaLogger.Logger.Instance().Dispose();
            MainServer.Instance.Dispose();
            DomainExceptionHandler.Unintialize();
        }

        static async Task Initialize(int dataVersion)
        {
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
