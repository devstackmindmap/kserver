using AkaData;
using AkaEnum.Battle;
using AkaLogger;
using AkaTimer;
using CommonProtocol;
using System;
using System.Collections.Generic;
using AkaUtility;

namespace BattleLogic
{
    public abstract class Battle : IDisposable
    {
        private readonly int _battleNum;
        private readonly Timer _elapsedTickTimer;

        protected Timer _battleTimer;
        protected Timer _boosterStartTimer;
        protected BattleHelper _battleHelper;
        protected Dictionary<PlayerType, Player> _players;
        protected Action<PlayerType, bool> _battleEnd;
        protected Action<BattleSendData> _onSessionSend;
        protected DataFillManager _dataFillManager;
        protected IBattleEndCondition _battleEndCondition;

        protected IBattleEndCondition BattleEndCondition => _battleEndCondition;
        public BattleHelper BattleHelper => _battleHelper;
        public BattleEnviroment Enviroment { get; }
        public Timer BattleTimer => _battleTimer;
        public Timer BoosterStartTimer => _boosterStartTimer;
        public Dictionary<PlayerType, Player> Players { get { return _players; } }
        public bool IsBattleEnd { get; private set; }
        public BattleStatus Status { get; }
        public string RoomId { get; private set; }

        private bool _isExtentionBattleTime = false;



        public Battle(int battleNum, BattleEnviroment enviroment)
        {
            _battleNum = battleNum;
            _elapsedTickTimer = new Timer(1000, true, () => _battleHelper.BattlePatternBehavior.DoTimePatternSchedule());
            _elapsedTickTimer.Name = "Battle Room Tick Counter Timer";
            Enviroment = enviroment;
            Status = new BattleStatus();
        }

        public void BattleInitialize(Dictionary<PlayerType, Player> players, Action<PlayerType, bool> battleEnd, Action<BattleSendData> sessionSend)
        {
            _players = players;
            _battleTimer = new Timer(Enviroment.BattleTimeMillisec, false, TryBattleEnd);
            _battleTimer.Name = "battle";
            _battleEnd = battleEnd;
            _onSessionSend = sessionSend;
            _boosterStartTimer = new Timer(Enviroment.BattleBoosterTimeMillisec, false, BoosterStart);
        }

        public void SetRoomId(string identifier)
        {
            RoomId = identifier;
        }

        public void SetDataManager(DataFillManager dataManager)
        {
            _dataFillManager = dataManager;
            _dataFillManager.SetBattle(this);
        }

        public void SetBattleEndCondition(IBattleEndCondition battleEndCondition)
        {
            _battleEndCondition = battleEndCondition;
        }

        public void SetBattleHelper(BattleHelper battleHelper)
        {
            _battleHelper = battleHelper;
            _battleHelper.SetBattle(this);
        }

        public void BattleStart(Object o = null)
        {
            var addSeconds = Data.GetContentsConstant(Enviroment.BattleType).BattleStartEffectTime;
            Status.StartDateTime = DateTime.UtcNow.AddSeconds(addSeconds);
            S2CManager.SendBattleStart(this, PlayerType.All);

            DoSkillBeforeStart();
            Utility.CallDelayAfter((int)(addSeconds * 1000), delegate
            {
                _battleHelper.BattleStart();
            });
        }

        public bool BattleEndCheck(Unit deathUnit)
        {
            if (BattleEndCondition.IsBattleEnd(_players, _isExtentionBattleTime, deathUnit))
            {
                IsBattleEnd = true;
                return true;
            }
            return false;
        }

        private void DoSkillBeforeStart()
        {
            foreach (var player in _players)
            {
                foreach (var unit in player.Value.Units)
                {
                    unit.Value.UnitPassive.PassiveConditionCheck(AkaEnum.PassiveConditionType.GameStart);
                }
            }
        }

        public Timer GetElapsedTimer()
        {
            return _elapsedTickTimer;
        }

        public Dictionary<PlayerType, Player> GetPlayers()
        {
            return _players;
        }

        public void Dispose()
        {
            _battleTimer.Dispose();
            _boosterStartTimer.Dispose();
            _elapsedTickTimer.Dispose();
            foreach (var player in _players.Values)
            {
                player.Dispose();
            }
            _battleHelper.BattleProgress.Dispose();
        }

        public BattleController GetBattleController()
        {
            return _battleHelper.BattleController;
        }

        public void AddBulletTime(int bulletTime)
        {
            Status.AccumulatedBulletTime += bulletTime;
            foreach (var player in _players)
            {
                player.Value.AddBulletTime(bulletTime);
            }
        }

        public int GetBattleNum()
        {
            return _battleNum;
        }

        public void TryBattleEnd()
        {
            //AkaLogger.Logger.Instance().Info($"TryBattleEnd:{_players[PlayerType.Player1].Units.Count}  {_players[PlayerType.Player2].Units.Count}  ");
            if (IsBattleExtention())
            {
                S2CManager.SendBattleExtentionTime(this, PlayerType.All);
                _battleHelper.BattleProgress.EnqueueExtension(new BattleActionExtension(this));
                return;
            }

            BattleEnd();
        }

        public void BoosterStart()
        {
            StartBooster();
        }

        private bool IsBattleExtention()
        {
            return Enviroment.BattleExtensionTime != 0.0
                && _isExtentionBattleTime == false
                && BattleEndCondition.CorrectBattleExtension(_players);
        }

        public void StartBattleExtentionTime(DateTime startExtensionDatetime)
        {
            var correctionMilisecond = (DateTime.UtcNow - startExtensionDatetime).TotalMilliseconds;
            _isExtentionBattleTime = true;
            _battleTimer.SetAttribute(1000 * Enviroment.BattleExtensionTime - correctionMilisecond, false);
            _battleTimer.Start();

            string unitCounts = $"{Players[PlayerType.Player1].Units.Count.ToString()}-{Players[PlayerType.Player2].Units.Count}";
            Log.Battle.StartExtension.Log(RoomId, (byte)Enviroment.BattleType, unitCounts, Players[PlayerType.Player1].PlayerIdentifier.UserId, Players[PlayerType.Player2].PlayerIdentifier.UserId);


            _players[PlayerType.Player1].ActionLog.SetStatus(ActionStatusType.EnterExtensionTime, 1);
            _players[PlayerType.Player2].ActionLog.SetStatus(ActionStatusType.EnterExtensionTime, 1);
        }

        private void StartBooster()
        {
            _players[PlayerType.Player1].StartBooster();
            _players[PlayerType.Player2].StartBooster();
            S2CManager.SendBoosterTime(this, PlayerType.All);

            Log.Battle.StartBooster.Log(RoomId, (byte)Enviroment.BattleType, Players[PlayerType.Player1].PlayerIdentifier.UserId, Players[PlayerType.Player2].PlayerIdentifier.UserId);
        }

        private void BattleEnd()
        {
            IsBattleEnd = true;

            var _battleNum = GetBattleNum();

            Logger.Instance().Debug("[BNum:" + _battleNum + "]" + "BattleEnd");

            _battleHelper.BattleProgress.EnqueueEnd(new BattleActionEnd(EnqueuedBattleEnd, BattleEndCondition.FinalWinner, false));
        }

        private void EnqueuedBattleEnd(PlayerType winPlayer, bool isRetreat)
        {
            foreach (var player in _players)
            {
                foreach (var unit in player.Value.Units)
                {
                    unit.Value.Dispose();
                }
            }
            _battleEnd(winPlayer, isRetreat);
        }

        public void Send(BattleSendData data)
        {
            _onSessionSend?.Invoke(data);
        }

        public void FillBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            _dataFillManager.FillBattleStartInfo(protoBattleStarts);

            BattleHelper.BattleRecorder.SetBattleStartInfo(protoBattleStarts);
        }

        public void FillCurrentBattleStatus(PlayerType playerType, BattleType battleType, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            _dataFillManager.FillCurrentBattleStatus(playerType, battleType, protoCurrentBattleStatus);
        }

        public void FillUpdateCardInfo(PlayerType playerType, ProtoUnitDeath protoUnitDeath)
        {
            _dataFillManager.FillUpdateCardInfo(playerType, protoUnitDeath);
        }

        public void EnqueueSkillReservation(Action<CardUseActionData> enqueueSkillAction, ProtoCardUse protoCardUse,
            CardUseActionData cardUseActionData)
        {
            _players[protoCardUse.Performer.PlayerType].EnqueueSkillReservation(enqueueSkillAction, cardUseActionData);
        }

        public void Retreat(ProtoRetreat protoRetreat, uint userId)
        {
            if (_players[protoRetreat.PlayerType].IsCanRetreat(userId))
            {
                _battleHelper.BattleProgress.EnqueueEnd(new BattleActionEnd(_battleEnd, _players[protoRetreat.PlayerType].Enemy.PlayerIdentifier.Player, true));
            }
        }

        public void UnitDeathPassiveConditionCheck()
        {
            foreach (var player in _players)
            {
                player.Value.UnitDeathPassiveConditionCheck();
            }
        }

        public ProtoBattleRecord GetBattleRecord(PlayerType winPlayer)
        {
            return BattleHelper.BattleRecorder.ToBattleRecord(winPlayer);
        }
    }
}
