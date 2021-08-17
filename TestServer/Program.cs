using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var ServerConfig = new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = 50507,
                ListenBacklog = 1024,
                KeepAliveTime = 30,
                MaxConnectionNumber = 3000,
                ReceiveBufferSize = 4096,
                MaxRequestLength = 1024,
                SendBufferSize = 4096,
                SendingQueueSize = 10,
                LogAllSocketException = true,
                LogBasicSessionActivity = true,
                LogCommand = true,
                ClearIdleSession = true,
                IdleSessionTimeOut = 600,
                SyncSend = false,
                Mode = SocketMode.Tcp
            };

            var RootConfig = new RootConfig
            {
                MaxCompletionPortThreads = 1000,
                MaxWorkingThreads = 1000,
                DisablePerformanceDataCollector = true,
                MinWorkingThreads = 100
            };

            if (!MainServer.Instance.Setup(rootConfig: RootConfig, config: ServerConfig))
            {
                Console.WriteLine("Setup Error");
            }
            MainServer.Instance.Start();

            terminatingEvent.WaitOne();

            MainServer.Instance.Dispose();
        }
    }
}
