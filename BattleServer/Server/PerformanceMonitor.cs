using AkaLogger;
using AkaThreading;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BattleServer
{
    public class PerformanceMonitor
    {
        public static PerformanceMonitor Instance { get; private set; }

        static PerformanceMonitor()
        {
#if DEBUG
            //    Instance = new PerformanceMonitor_Imp();
            Instance = new PerformanceMonitor();
#else
            Instance = new PerformanceMonitor();
#endif
        }

        public virtual void StartMonitor()
        {
            _timer = new Timer(MonitorCallback, null, 5 * 60 * 1000, 5 * 60 * 1000);

        }


        public virtual void Dispose()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
        }

        public virtual void AddMessage(MessageType msgType, int time) { }
        public virtual void AddMessage(string typeName, int time) { }


        private Timer _timer;



        private void MonitorCallback(object state)
        {
            var paramDatas = new Dictionary<string, string>();

            ThreadPool.GetAvailableThreads(out var workThreads, out var completionPortThreads);
            ThreadPool.GetMaxThreads(out var maxworkThreads, out var maxCompletionPortsThreads);

            var process = System.Diagnostics.Process.GetCurrentProcess();
            paramDatas["Rooms"] = RoomManager.RoomCount.ToString();
            paramDatas["Sessions"] = MainServer.Instance.SessionCount.ToString();
            paramDatas["Threads"] = process.Threads.Count.ToString();
            paramDatas["WorkingSet"] = (process.WorkingSet64 / 1024).ToString();
            paramDatas["PrivateMem"] = (process.PrivateMemorySize64 / 1024).ToString();
            paramDatas["HandleCount"] = process.HandleCount.ToString();
            paramDatas["Cores"] = Environment.ProcessorCount.ToString();
            paramDatas["Tasks"] = (maxworkThreads - workThreads).ToString();
            paramDatas["TotalMem"] = (GC.GetTotalMemory(false) / 1024).ToString();

            var gcMaxGen = GC.MaxGeneration;
            for (int i = 0; i < gcMaxGen; i++)
                paramDatas["GC_" + i.ToString()] = GC.CollectionCount(i).ToString();

            paramDatas["ToRedisLocks"] = SemaphoreManager.Count(SemaphoreType.BattleServer2GameRedisServerBalancer).ToString();
            paramDatas["ToGameLocks"] = SemaphoreManager.Count(SemaphoreType.BattleServer2GameServerBalancer).ToString();

            Logger.Instance().Analytics("Monitor", 
                ("Monitor,"+ string.Join(",", paramDatas.Select(keyPair => keyPair.Key + "," + keyPair.Value))).Split(',')
                );
        }



        private class NotUsed_PerformanceMonitor_Imp : PerformanceMonitor
        {

            System.IO.TextWriter _writer;
            System.Collections.Concurrent.ConcurrentQueue<string> _messages = new System.Collections.Concurrent.ConcurrentQueue<string>();

            private Thread _workerThread;
            private ManualResetEventSlim _workEvent;
            private ManualResetEventSlim _disposedEvent;

            private int _disposed;

            public override void StartMonitor()
            {
                Dispose();


                _disposed = 0;
                if (Directory.Exists("./plogs") == false)
                    Directory.CreateDirectory("./plogs");

                var fileName = $"./plogs/message_{DateTime.Now.ToString("MMdd.hhmm")}.log";
                var isExist = System.IO.File.Exists(fileName);
                var messageFile = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite); //System.IO.File.OpenWrite(fileName);
                _writer = new System.IO.StreamWriter(messageFile);
                messageFile.Seek(0, System.IO.SeekOrigin.End);

                if (isExist == false)
                {
                    var header = string.Join("|", "Message", "Time", "Rooms", "Sessions", "PoolCount", "MaxThread", "IOCP");

                    _writer.WriteLine(header);
                }
                _workerThread = new Thread(new ThreadStart(InternalSave));
                _workerThread.Priority = ThreadPriority.Lowest;

                _workEvent = new ManualResetEventSlim(false);
                _disposedEvent = new ManualResetEventSlim(false);
                _workerThread.Start();
            }



            public override void Dispose()
            {
                if (0 == Interlocked.Exchange(ref _disposed, 1))
                {
                    _workEvent?.Set();
                    _disposedEvent?.Wait();

                    _disposedEvent?.Dispose();
                    _workEvent?.Dispose();
                }
            }


            public override void AddMessage(MessageType msgType, int time)
            {
                AddMessage(msgType.ToString(), (int)time);
            }

            public override void AddMessage(string typeName, int time)
            {
                if (_disposed == 1)
                    return;

                ThreadPool.GetAvailableThreads(out var workThreads, out var completionPortThreads);
                ThreadPool.GetMaxThreads(out var maxworkThreads, out var maxCompletionPortsThreads);

                var message = string.Join("|", typeName, time.ToString(), RoomManager.RoomCount.ToString(), MainServer.Instance.SessionCount.ToString(), workThreads.ToString(), maxworkThreads.ToString(), completionPortThreads.ToString());

                _messages.Enqueue(message);
                _workEvent.Set();
            }

            private async void InternalSave()
            {
                while (_messages.IsEmpty == false || _disposed == 0)
                {
                    if (_messages.IsEmpty == true)
                    {
                        await _writer.FlushAsync();
                        _workEvent.Reset();
                    }

                    _workEvent.Wait();
                    if (_messages.TryDequeue(out var message))
                    {
                        await _writer.WriteLineAsync(message);
                    }
                }

                _writer.Close();
                _disposedEvent.Set();

            }

        }
    }

}