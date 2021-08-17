using AkaConfig;
using AkaData;
using AkaEnum;
using AkaEnum.Battle;
using AkaRedisLogic;
using AkaSerializer;
using AkaUtility;
using BattleLogic;
using BattleServer.BattleRecord;
using CommonProtocol;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer
{
    public abstract class Room
    {
        private static readonly string _runModeKey;
        public static int battleNum = 0;
        private readonly PlayerInfo _playerInfo = new PlayerInfo();

        private BattleController _controller;

        protected uint BackgroundImageId { get; private set; }
        protected Battle Battle { get; private set; }

        public PlayerInfo PlayerInfo => _playerInfo;
        public string RoomId { get; }

        public RoomStatus Status { get; set; }

        private BattleInfo _battleInfo;

        protected abstract Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId);
        protected abstract void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer);
        public abstract bool TryEnterGuestPlayer(BattleInfo playerInfo);

        public abstract Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(AkaEnum.ModeType modeType);

        public abstract string SessionIdByPlayer(PlayerType playerType);

        public abstract void ReEnterRoom(uint userId, string sessionId, ProtoCurrentBattleStatus protoCurrentBattleStatus);

        protected abstract void Send(BattleSendData data);

        protected abstract Task SendBattleResultAsync(PlayerType winPlayer);

        protected virtual AdditionalBattleInfo GetAdditionalBattleInfo() { return null; }

        public Room(string roomId, IBattleInfo battleInfo)
        {
            _battleInfo = battleInfo as BattleInfo;

            RoomId = roomId;
            Status = RoomStatus.Waiting;

            PlayerInfo.BattleType = _battleInfo.BattleType;
            PlayerInfo.DeckModeType = BattleEnviroment.GetDeckModeType(_battleInfo.BattleType);
            PlayerInfo.Player1DeckNum = _battleInfo.DeckNum;
            PlayerInfo.Player1UserId = _battleInfo.UserId;
        }

        public async Task<ResultType> BattleInitialize()
        {
            var battleCreator = new BattleCreator(PlayerInfo, GetDeckInfo);
            battleNum++;
            var battleAndResultType = await battleCreator.GetBattle(battleNum, PlayerInfo.BattleType, BattleEnd, Send, GetAdditionalBattleInfo());
            if (battleAndResultType.resultType != ResultType.Success)
                return battleAndResultType.resultType;

            Battle = battleAndResultType.battle;
            Battle.SetRoomId(RoomId);
            _controller = Battle.GetBattleController();

            return ResultType.Success;
        }

        public void BattleStart()
        {
            lock (this)
            {
                if (Status == RoomStatus.BattleRoomMatched)
                {
                    PlayerInfo.Player1Ready = true;
                    PlayerInfo.Player2Ready = true;
                    Status = RoomStatus.InBattle;

                    Battle.BattleStart();
                    AkaLogger.Log.Battle.Start.Log(RoomId, (byte)PlayerInfo.BattleType, PlayerInfo.Player1UserId, PlayerInfo.Player2UserId);
                }
            }
        }

        public void EmoticonUse(ProtoEmoticonUse protoEmoticonUse)
        {
            if (Battle.Enviroment.CanEmoticon)
            {
                S2CManager.SendEmoticonUse(Battle, protoEmoticonUse);

                Battle.Players[protoEmoticonUse.PlayerType].ActionLog.IncreaseStatus(ActionStatusType.EmoticonUse);

                AkaLogger.Log.User.Emoticon.Log(RoomId, protoEmoticonUse.IsPlayer, protoEmoticonUse.PlayerType.ToString(), protoEmoticonUse.EmoticonId, PlayerInfo.Player1UserId, PlayerInfo.Player2UserId);
            }
        }

        public void CardUse(ProtoCardUse protoCardUse)
        {
            Card card = null;
            try
            {
                card = GetCard(protoCardUse);
                if (card == null)
                {
                    S2CManager.SendCardUseResult(Battle, protoCardUse, AkaEnum.ElixirCountState.NotEnough);
                    return;
                }

            }
            catch (Exception e)
            {
                S2CManager.SendCardUseResult(Battle, protoCardUse, AkaEnum.ElixirCountState.NotEnough);
                AkaLogger.Logger.Instance().Error(e.ToString());
                return;
            }

            EnqueueCardUse(card, protoCardUse);
        }


        private Card GetCard(ProtoCardUse protoCardUse)
        {
            var card = Battle.GetBattleController().GetHandCard(protoCardUse.Performer.PlayerType, protoCardUse.HandIndex);
            if (card == null)
                return null;

            if (IsExistUnit(protoCardUse) == false ||
                Battle.Players[protoCardUse.Performer.PlayerType].Units[protoCardUse.Performer.UnitPositionIndex].UnitData.UnitIdentifier.UnitId
                != card.UnitId)
                return null;

            return card;
        }

        private bool IsExistUnit(ProtoCardUse protoCardUse)
        {
            return Battle.Players[protoCardUse.Performer.PlayerType].Units.ContainsKey(protoCardUse.Performer.UnitPositionIndex);
        }

        private void EnqueueCardUse(Card card, ProtoCardUse protoCardUse)
        {
            Battle.BattleHelper.BattleProgress.EnqueueCardUse(new BattleActionCardUse(Battle, card, protoCardUse));
        }

        public void FillBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            Battle.FillBattleStartInfo(protoBattleStarts);

            var dataBackgroundImageId = GetBackgroundImageId(Battle.Enviroment.Enviroment.BackgroundId);

            if (dataBackgroundImageId == 0)
            {
                var stageRound = AkaData.Data.GetStageRound(PlayerInfo.StageRoundId);
                var backgroundImageIdList = AkaData.Data.GetAllBackgroundImageId();
                dataBackgroundImageId = stageRound?.BackgroundImageId ?? 1;
                if (false == backgroundImageIdList.Any(backgroundImageId => backgroundImageId == dataBackgroundImageId))
                    dataBackgroundImageId = 1;
            }
            BackgroundImageId = dataBackgroundImageId;
            foreach (var beforeStart in protoBattleStarts.Values)
            {
                beforeStart.BackgroundImageId = dataBackgroundImageId;
            }

            SendLogBeforeBattleStart(protoBattleStarts);
        }

        private void SendLogBeforeBattleStart(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            var player1UserId = PlayerInfo.Player1UserId;
            var player2UserId = PlayerInfo.Player2UserId;

            var player1Info = protoBattleStarts[PlayerType.Player2].EnemyPlayer;
            var player2Info = protoBattleStarts[PlayerType.Player1].EnemyPlayer;
            var player1Units = player1Info.Units.Select(unit => unit.UnitId);
            var player2Units = player2Info.Units.Select(unit => unit.UnitId);
            var player1Levels = player1Info.Units.Select(unit => unit.Level);
            var player2Levels = player2Info.Units.Select(unit => unit.Level);

            AkaLogger.Log.Battle.BeforeStart.Log(RoomId, PlayerInfo.StageRoundId, (byte)PlayerInfo.BattleType, player1UserId, player2UserId,
                player1Info.CardStatIds, player1Units, player1Levels, player2Info.CardStatIds, player2Units, player2Levels
                );

        }

        private uint GetBackgroundImageId(uint backgroundId)
        {
            var dataBackground = AkaData.Data.GetBackgroundList(backgroundId);
            if (dataBackground?.Any() ?? false)
            {
                var selectedIndex = AkaRandom.Random.ChooseIndexRandomlyInSumOfProbability(dataBackground);
                return dataBackground[selectedIndex].BackgroundImageId;
            }
            return 0;
        }

        public string GetPlayer1SessionId()
        {
            return SessionIdByPlayer(PlayerType.Player1);
        }

        public string GetWebServerIp()
        {
            //already checked EnterRoom.IsValidArea
            return $"http://{Config.BattleServerConfig.GameServer.ip}:{Config.BattleServerConfig.GameServer.port}/";
        }

        public string GetPlayer1MemberKey()
        {
            return KeyMaker.GetMemberKey(PlayerInfo.Player1UserId);
        }

        public virtual void RemoveRoom()
        {
            Battle.Dispose();
            RoomManager.RemoveRoom(RoomId);
        }

        protected void TrySend(string sessionId, MessageType messageType, byte[] data)
        {
            var session = MainServer.Instance.GetSessionByID(sessionId);
            session?.Send(messageType, data);
        }


        public async Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(List<KeyValuePair<uint, byte>> userIdAndDeckNums, BattleType battleType, AkaEnum.ModeType modeType)
        {
            WebServerRequestor webServer = new WebServerRequestor();
            var resBytes = await webServer.RequestAsync(MessageType.GetDeckWithDeckNum, AkaSerializer<ProtoGetDeckWithDeckNum>.Serialize(new ProtoGetDeckWithDeckNum
            {
                UserIdAndDeckNums = userIdAndDeckNums,
                ModeType = modeType,
                BattleType = battleType
            }), GetWebServerIp());
            return AkaSerializer<ProtoOnGetDeckWithDeckNum>.Deserialize(resBytes);
        }

        protected List<ProtoBattleResultPlayerInfo> GetPlayerInfoList(PlayerType winPlayer)
        {
            List<ProtoBattleResultPlayerInfo> playerInfoList = new List<ProtoBattleResultPlayerInfo>
            {
                new ProtoBattleResultPlayerInfo
                {
                    UserId = PlayerInfo.Player1UserId,
                    DeckNum = PlayerInfo.Player1DeckNum,
                    BattleResultType = GetBattleResultType(winPlayer, PlayerType.Player1),
                    ActionStatusLog = GetActionLogs(PlayerType.Player1)
                }
            };

            if (PlayerInfo.Player2UserId != 0)
            {
                playerInfoList.Add(new ProtoBattleResultPlayerInfo
                {
                    UserId = PlayerInfo.Player2UserId,
                    DeckNum = PlayerInfo.Player2DeckNum,
                    BattleResultType = GetBattleResultType(winPlayer, PlayerType.Player2),
                    ActionStatusLog = GetActionLogs(PlayerType.Player2)
                });
            }

            return playerInfoList;
        }

        protected List<ProtoActionStatus> GetActionLogs(PlayerType playerType)
        {
            var actionStatusLogs = new List<ProtoActionStatus>();
            var cardActionLogs
                = Battle.GetBattleController().GetBattleCards()[playerType].AllCards
                .SelectMany(card => card.ActionLog.ToResult(card.CardId));

            foreach (var cardActionStatus in cardActionLogs)
            {
                var actionStatusLog = actionStatusLogs
                    .FirstOrDefault(actionStatus => actionStatus.ActionStatusType == cardActionStatus.ActionStatusType
                    && actionStatus.ClassId == cardActionStatus.ClassId);

                if (actionStatusLog == null)
                    actionStatusLogs.Add(cardActionStatus);
                else
                    actionStatusLog.Value += cardActionStatus.Value;
            }

            var playerActionLogs = Battle.Players[playerType].ActionLog.ToResult(0);
            var unitActionLogs = Battle.Players[playerType].Units
                .SelectMany(unitInfo => unitInfo.Value.ActionLog.ToResult(unitInfo.Value.UnitData.UnitIdentifier.UnitId));

            var deathUnitActionLogs = Battle.Players[playerType].DeathUnits
                .SelectMany(unitInfo => unitInfo.Value.ActionLog.ToResult(unitInfo.Value.UnitData.UnitIdentifier.UnitId));

            actionStatusLogs.AddRange(playerActionLogs);
            actionStatusLogs.AddRange(unitActionLogs);
            actionStatusLogs.AddRange(deathUnitActionLogs);

            return actionStatusLogs;
        }

        public async Task DeleteBattlePlayerInfoRedisKey()
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();

            var memberKeys = new string[] { KeyMaker.GetMemberKey(PlayerInfo.Player1UserId), PlayerInfo.Player2UserId != 0 ? KeyMaker.GetMemberKey(PlayerInfo.Player2UserId) : "" };
            await GameBattleRedisJob.DeleteBattlePlayingInfoAsync(redis, memberKeys);
        }

        private async void BattleEnd(PlayerType winPlayer, bool isRetreat)
        {
            if (Status == RoomStatus.EndBattle)
                return;

            AkaLogger.Logger.Instance().Debug($"Victory:{winPlayer.ToString()}");
            Battle.Status.EndDateTime = DateTime.UtcNow;
            Status = RoomStatus.EndBattle;

            var time = DateTime.Now;
            try
            {
                Send(new BattleSendData
                {
                    MessageType = MessageType.BattleEnd,
                    PlayerType = PlayerType.All,
                    Data = AkaSerializer<ProtoBattleEnd>.Serialize(new ProtoBattleEnd
                    {
                        MessageType = MessageType.BattleEnd,
                        Winner = winPlayer,
                        IsRetreat = isRetreat
                    })
                });

                await SendBattleResultAsync(winPlayer);
                PerformanceMonitor.Instance.AddMessage(MessageType.GetBattleResult, (int)(DateTime.Now - time).TotalMilliseconds);

                SendBattleRecord(winPlayer);
            }
            catch (Exception ex)
            {
                AkaLogger.Log.Debug.Exception("BattleEnd", ex);
                AkaLogger.Logger.Instance().Error($"[BattleEnd] {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                RemoveRoom();
                PerformanceMonitor.Instance.AddMessage("BattleResultFinal", (int)(DateTime.Now - time).TotalMilliseconds);

            }
        }

        private void SendBattleRecord(PlayerType winPlayer)
        {
            var protoBattleRecord = Battle.GetBattleRecord(winPlayer);
            if (protoBattleRecord != null)
            {
                protoBattleRecord.RecordKey = $"{protoBattleRecord.UserId.ToString()}_{protoBattleRecord.EnemyUserId.ToString()}_{protoBattleRecord.BattleStartTime.ToString()}_{_runModeKey}";

                BattleRecordStorage.Instance.Save(protoBattleRecord);
            }
        }

        protected AkaEnum.BattleResultType GetBattleResultType(PlayerType winPlayer, PlayerType who)
        {
            if (winPlayer == PlayerType.None)
                return AkaEnum.BattleResultType.Draw;
            else if (winPlayer == who)
                return AkaEnum.BattleResultType.Win;
            else
                return AkaEnum.BattleResultType.Lose;
        }

        public void Retreat(ProtoRetreat protoRetreat, uint userId)
        {
            AkaLogger.Log.Battle.Retreat.Log(userId, (byte)PlayerInfo.BattleType, RoomId);
            Battle.Retreat(protoRetreat, userId);
        }

        public void BattleReady(uint userId)
        {
            if (PlayerInfo.Player1UserId == userId)
                PlayerInfo.Player1Ready = true;
            else if (PlayerInfo.Player2UserId == userId)
                PlayerInfo.Player2Ready = true;

            if (PlayerInfo.Player1Ready && PlayerInfo.Player2Ready)
            {
                BattleStart();
            }
        }

        protected SortedList<uint, uint> GetAliveAndDeathUnits(Player player)
        {
            var units = new SortedList<uint, uint>();

            foreach (var unit in player.Units)
                units.Add(unit.Value.UnitData.UnitIdentifier.UnitId, unit.Value.UnitData.UnitIdentifier.Level);


            foreach (var unit in player.DeathUnits)
                units.Add(unit.Value.UnitData.UnitIdentifier.UnitId, unit.Value.UnitData.UnitIdentifier.Level);

            return units;
        }

        protected void SetPrevSendBattleResult(PlayerType winPlayer)
        {
            if (false == Battle.Players.TryGetValue(winPlayer, out var player))
                return;
            foreach (var unit in player.Units.Values)
                unit.ActionLog.SetStatus(ActionStatusType.UnitActionVictory, 1);
            foreach (var unit in player.DeathUnits.Values)
                unit.ActionLog.SetStatus(ActionStatusType.UnitActionVictory, 1);
        }
    }
}
