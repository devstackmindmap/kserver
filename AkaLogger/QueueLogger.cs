

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AkaLogger
{
    public sealed class QueueLogger : IDisposable
    {
        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        private Thread _workerThread ;

        private ManualResetEventSlim _workEvent;
        private ManualResetEventSlim _disposedEvent;

        private int _disposed;

        private string _loggerPath;
        private DateTime _today;

        TextWriter _writer;

        public void Start()
        {
            Dispose();


            _loggerPath = AppDomain.CurrentDomain.BaseDirectory + "/ErrLogs";
            _disposed = 0;

            _today = DateTime.Now;
            CreateLog(_today);

            _workerThread = new Thread(new ThreadStart(InternalProc));
            _workerThread.Priority = ThreadPriority.Lowest;

            _workEvent = new ManualResetEventSlim(false);
            _disposedEvent = new ManualResetEventSlim(false);
            _workerThread.Start();


        }

        public void Dispose()
        {
            if (0 == Interlocked.Exchange(ref _disposed,1))
            {
                _workEvent?.Set();
                _disposedEvent?.Wait();

                _disposedEvent?.Dispose();
                _workEvent?.Dispose();
            }
        }

        public void Log(string message, Exception ex)
        {
            if (_disposed == 1)
                return;
           
            _messages.Enqueue($"[{DateTime.Now}]{message} - {ex.Message}\n{ex.ToString()}\n");
            _workEvent.Set();
        }

        private void CreateLog(DateTime now)
        {
            if (Directory.Exists(_loggerPath) == false)
                Directory.CreateDirectory(_loggerPath);

            _today = now;
            var filePath = Path.Combine(_loggerPath, _today.ToString("yyyyMMdd") + ".log");
            _writer?.Close();

            var logFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _writer = new StreamWriter(logFile);
        }

        private async Task InternalTrySave(string message)
        {
            var now = DateTime.Now;
            if(now.Day != _today.Day)
            {
                CreateLog(now);
            }

            await _writer.WriteLineAsync(message);
        }


        private async void InternalProc()
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
                    await InternalTrySave(message);
                }
            }

            _writer.Close();
            _disposedEvent.Set();
        }

    }
}
