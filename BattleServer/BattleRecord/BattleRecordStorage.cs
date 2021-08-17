using AkaSerializer;
using CommonProtocol;
using Network;
using System;
using System.IO.Compression;
using System.IO;
using AkaConfig;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace BattleServer.BattleRecord
{
    public sealed class BattleRecordStorage : IDisposable
    {
        private ConcurrentQueue<ProtoBattleRecord> _battleRecordsQueue;

        private Thread _workerThread;

        private ManualResetEventSlim _workEvent;
        private ManualResetEventSlim _disposedEvent;
        private IWriter _recordWriter;

        private bool _disposed;
        
        public static BattleRecordStorage Instance { get; private set; }

        static BattleRecordStorage()
        {
            Instance = new BattleRecordStorage();
        }


        public void Start()
        {
            Dispose();
            InitRecordWriter();
            _disposed = false;
            _battleRecordsQueue = new ConcurrentQueue<ProtoBattleRecord>();

            _workerThread = new Thread(new ThreadStart(InternalSave));
            _workerThread.Priority = ThreadPriority.BelowNormal;
            _workEvent = new ManualResetEventSlim(false);
            _disposedEvent = new ManualResetEventSlim(false);

            _workerThread.Start();
        }

        private void InitRecordWriter()
        {
            //if (Config.BattleServerConfig.RecordSetting.UseAWS)
            //    _recordWriter = new AWSWriter();
            //else if (Config.BattleServerConfig.RecordSetting.LocalStoragePath.Length > 0)
            //    _recordWriter = new FileWriter();
            //else
            //    _recordWriter = new NullWriter();

        }

        public void Save(ProtoBattleRecord battleRecord)
        {
            if (_disposed)
                return;

            _battleRecordsQueue.Enqueue(battleRecord);
            _workEvent.Set();
        }


        public void Dispose()
        {
            _disposed = true;
            _workEvent?.Set();
            _disposedEvent?.Wait();
            
            _disposedEvent?.Dispose();
            _workEvent?.Dispose();
        }

        private async Task InternalTrySave(ProtoBattleRecord battleRecord)
        {
            var data = AkaSerializer<ProtoBattleRecord>.Serialize(battleRecord);
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, CompressionLevel.Optimal, true))
                {
                    await ds.WriteAsync(data, 0, data.Length);
                    ds.Close();
                }

                if (true == await _recordWriter.WriteAsync(ms, battleRecord.RecordKey))
                {
                    WebServerRequestor webServer = new WebServerRequestor();
                    battleRecord.Behaviors = null;
                    await webServer.RequestAsync(MessageType.SaveBattleRecordInfo, AkaSerializer<ProtoBattleRecord>.Serialize(battleRecord), GetWebServerIp());
                }

                ms.Close();
                return;
            }

        }

        private string GetWebServerIp()
        {
            return $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/";
        }

        private async void InternalSave()
        {
            while (_battleRecordsQueue.IsEmpty == false || _disposed == false)
            {
                if (_battleRecordsQueue.IsEmpty == true)
                    _workEvent.Reset();

                _workEvent.Wait();
                if (_battleRecordsQueue.TryPeek(out var battleRecord))
                {
                    try
                    {
                        await InternalTrySave(battleRecord);
                    }
                    catch(Exception ex)
                    {
                        AkaLogger.Log.Debug.Exception("BattleRecord:" + battleRecord.RecordKey , ex);
                        AkaLogger.Logger.Instance().Error(battleRecord.RecordKey + " : " + ex.ToString());
                    }
                    _battleRecordsQueue.TryDequeue(out battleRecord);
                }
            }
            _disposedEvent.Set();
        }

    }
}
