using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SuperSocket.ClientEngine;
using System.Threading;
using KnightUWP.Servicecs.Network;
using Windows.UI.Xaml;
using AkaData;
using Windows.Storage;

namespace KnightUWP.Dao
{
    public enum UserState
    {
        None,
        Matching,
        Matched,
        BattleEnterRoom,
        BattleBeforeStart,
        BattleStart,
        BattleWrongClosed,
    }

    public partial class UserInfo : System.ComponentModel.INotifyPropertyChanged
    {
        public UserInfo()
        {
            UnitsWithPvPDeck = new Dictionary<int, List<uint>>();
            CardsWithPvPDeck = new Dictionary<int, List<uint>>();
            Latencties = new ConcurrentQueue<int>();
            CurrentDeckNum = 0;
            CurrentBattleInfo = new BattleInfo();
        }

        public Accounts accounts { get; set; }
        public Users users { get; set; }



        public ProtoOnLogin LoginInfo { get; set; }

        public Dictionary<int, List<uint>> UnitsWithPvPDeck { get; set; }

        public Dictionary<int, List<uint>> CardsWithPvPDeck { get; set; }



        public string Gold => $"골드:{users.gold}";
        public string Gem => $"보석:{users.gem}";


        public ClientNetworkConnector PubsubConnecter { get; set; }

        public ClientNetworkConnector MatchingConnecter { get; set; }

        public ConcurrentQueue<DataEventArgs> MatchingDatas { get; set; }


        public ClientNetworkConnector BattleConnecter { get; set; }

        public ConcurrentQueue<DataEventArgs> BattleDatas { get; set; }

        public ProtoMatchingSuccess CurrentMatchingInfo { get; set; }

        public ProtoOnGetDeckWithDeckNum CurrentMyBattleDeck { get; set; }

        public int CurrentDeckNum { get; set; }


        public DateTime LastSyncTimeSended = DateTime.UtcNow;
        public bool ReceivedSyncTime = true;
        public ConcurrentQueue<int> Latencties { get; set; }
        public int LastLatency { get; set; }

        static readonly string[] logMetaHeader = new string[] { "Result", "UserId", "Nickname", "Level", "MaxRankLevel", "RankPoint","MaxLatency","AverageLatency","BattleTime"
                                                         , "Unit1","Unit1Name","Unit1Level", "Unit2","Unit2Name","Unit2Level", "Unit3","Unit3Name","Unit3Level"
                                                         , "EnemyUserId", "EnemyNickname", "EnemyRankPoint", "EnemyUnit1", "EnemyUnit1Name","EnemyUnit1Level","EnemyUnit2", "EnemyUnit2Name","EnemyUnit2Level","EnemyUnit3", "EnemyUnit3Name","EnemyUnit3Level" };
        static readonly string[] logHeader = new string[] { "Message", "Performer", "Time", "Latency","AverageLatency","LeftSide","Center","RightSide" };


        static StorageFile metaFile;

        public Task Processor;

        static UserInfo()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
           
            var fileName = $"{DateTime.Now.ToString("MMdd_hhmmss")}";
            
            var existTask = storageFolder.TryGetItemAsync($"{fileName}_meta.csv").AsTask();
            existTask.Wait();
            bool exist = existTask.Result != null;


            var task = storageFolder.CreateFileAsync($"{fileName}_meta.csv", CreationCollisionOption.OpenIfExists).AsTask();
            

            task.Wait();
            metaFile = task.Result;
            if (exist == false)
            {
                _ = FileIO.WriteTextAsync(metaFile, string.Join("|", logMetaHeader) + "\n");                
            }

            Task.Factory.StartNew(()=> MetaLogWriter());
        }

        static SpinLock _metaLogLock = new SpinLock();

        public async void WriteLog(string otherStatus = "")
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            bool gotLock = false;
            if (otherStatus.Length > 0)
            {

                _metaLogDatas.Enqueue($"{otherStatus}|{users.userId}|\n");
           //     _metaLogLock.Enter(ref gotLock);
           //     FileIO.AppendTextAsync(metaFile, $"{otherStatus}|{users.userId}|\n").AsTask().Wait();
           //     if (gotLock) _metaLogLock.Exit();

                return;
            }

            var logList = CurrentBattleInfo.History.Select(process =>
            {
                return string.Join("|", process.MessageType, process.Performer, process.EventTimeString, process.LastLatency, process.EverageLatency, process.LeftSideMessage, process.CenterMessage, process.RightSideMessage);
            });

            var resultState = State == UserState.BattleWrongClosed ? "Wrong" : "End";
            var fileName = $"{DateTime.Now.ToString("MMdd_hhmmss")}_{resultState}_{ users.userId}_{EnemyUserId}";

            var metaResult =
                string.Join("|", resultState, users.userId, accounts.nickName, users.level, accounts.maxRankLevel, accounts.currentSeasonRankPoint, MaxLatency, AverageLatency, CurrentBattleInfo.BattleTime
                               , CurrentBattleInfo.MyUnits[0].UnitId, CurrentBattleInfo.MyUnits[0].Name, CurrentBattleInfo.MyUnits[0].Level
                               , CurrentBattleInfo.MyUnits[1].UnitId, CurrentBattleInfo.MyUnits[1].Name, CurrentBattleInfo.MyUnits[1].Level
                               , CurrentBattleInfo.MyUnits[2].UnitId, CurrentBattleInfo.MyUnits[2].Name, CurrentBattleInfo.MyUnits[2].Level
                               , EnemyUserId, EnemyNickName, EnemyRankPoint
                               , CurrentBattleInfo.EnemyUnits[0].UnitId, CurrentBattleInfo.EnemyUnits[0].Name, CurrentBattleInfo.EnemyUnits[0].Level
                               , CurrentBattleInfo.EnemyUnits[1].UnitId, CurrentBattleInfo.EnemyUnits[1].Name, CurrentBattleInfo.EnemyUnits[1].Level
                               , CurrentBattleInfo.EnemyUnits[2].UnitId, CurrentBattleInfo.EnemyUnits[2].Name, CurrentBattleInfo.EnemyUnits[2].Level
                               ) + "\n";




            List<string> historyText = new List<string>();
            historyText.Add(string.Join("|", logHeader));
            historyText.AddRange(logList);


            StorageFile logfile = await storageFolder.CreateFileAsync($"{fileName}.csv", CreationCollisionOption.ReplaceExisting);
            _ = FileIO.WriteLinesAsync(logfile,  historyText);

            _metaLogDatas.Enqueue(metaResult);
            //gotLock = false;
            //_metaLogLock.Enter(ref gotLock);
            //FileIO.AppendTextAsync(metaFile, metaResult) .AsTask().Wait();
            //if (gotLock) _metaLogLock.Exit();


        }


        public static volatile bool KeepRun = true;
        static ConcurrentQueue<string> _metaLogDatas = new ConcurrentQueue<string>();

        private static async void MetaLogWriter()
        {
            while(KeepRun)
            {
                if (_metaLogDatas.TryPeek(out var logData))
                {
                    bool success = true;
                    try
                    {
                        await FileIO.AppendTextAsync(metaFile, logData).AsTask();
                    }
                    catch(Exception ex)
                    {
                        success = false;
                    }
                    if (success)
                        _metaLogDatas.TryDequeue(out logData);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
