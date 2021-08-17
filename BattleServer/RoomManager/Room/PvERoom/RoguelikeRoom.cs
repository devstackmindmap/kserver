using AkaEnum;
using AkaEnum.Battle;
using AkaLogger;
using AkaSerializer;
using BattleLogic;
using CommonProtocol;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleServer
{
    public class RoguelikeRoom : PvERoom
    {
        protected readonly Stage _stage;
        private BattleInfo _battleInfo;
        private string _myIp;

        public RoguelikeRoom(string roomId, IBattleInfo battleInfo)
            : base(roomId, battleInfo)
        {
            //_battleInfo = battleInfo as BattleInfoRoguelike;
            _battleInfo = battleInfo as BattleInfo;
            //PlayerInfo.Player1TreasureIdList = _battleInfo.TreasureIdList == null ? new List<uint>() : new List<uint>(_battleInfo.TreasureIdList);
            PlayerInfo.Player1TreasureIdList = new List<uint>();
            _stage = new Stage(_battleInfo);
            PlayerInfo.StageRoundId = _stage.CurrentStageRoundId();
            _myIp = _battleInfo.BattleServerIp;
        }

        protected override async Task<byte[]> GetBattleResultFromGameServerAsync(PlayerType winPlayer, uint stageLevelId)
        {
            _ = DeleteBattlePlayerInfoRedisKey();
            var playerInfoList = GetPlayerInfoList(winPlayer);

            WebServerRequestor webServer = new WebServerRequestor();
            return await webServer.RequestAsync(MessageType.GetBattleResult, AkaSerializer<ProtoBattleResultStage>.Serialize(new ProtoBattleResultStage
            {
                PlayerInfoList = playerInfoList,
                BattleType = PlayerInfo.BattleType,
                MessageType = MessageType.GetBattleResult,
                StageLevelId = _battleInfo.StageLevelId
            }), GetWebServerIp());
        }

        public override async Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(ModeType modeType)
        {
            return new ProtoOnGetDeckWithDeckNum
            {
                UserAndDecks = new Dictionary<uint, ProtoDeckWithDeckNum>() { { PlayerInfo.Player1UserId, _stage.GetCurrentRoundSaveDeck() } }
            };
        }

        public override bool TryEnterGuestPlayer(BattleInfo playerInfo)
        {
            if (base.TryEnterGuestPlayer(playerInfo))
            {
                //PlayerInfo.Player2TreasureIdList = _battleInfo.TreasureIdList == null ? new List<uint>() : new List<uint>(_battleInfo.TreasureIdList);
                PlayerInfo.Player2TreasureIdList = new List<uint>();
                return true;
            }
            return false;
        }

        protected override async Task PlayerEnd(PlayerType winPlayer)
        {
            var session = SessionIdByPlayer(PlayerType.Player1);
            var userId = PlayerInfo.Player1UserId;
            var battleResultType = GetBattleResultType(winPlayer, PlayerType.Player1);
            bool isRoundResult = false;

            var roundResult = new ProtoStageRoundResult
            {
                StageLevelId = _stage.StageLevelId,
                Round = _stage.CurrentRound,
            };
            var currentStageRoundId = _stage.CurrentStageRoundId();

            uint logPlayer1RankLevel = 0;

            if (battleResultType == BattleResultType.Win && _stage.RoundClearAndGoNextRound(roundResult))
            {
                if (ModeType.SaveDeck == BattleEnviroment.GetDeckModeType(PlayerInfo.BattleType))
                {
                    roundResult.CardStatIdList = _stage.SaveDeckCardStatIdList;
                    roundResult.TreasureIdList = PlayerInfo.Player1TreasureIdList;
                    await UpdateSaveDeckInfoAsync(userId, roundResult);
                }

                TrySend(session, MessageType.BattleChallengeRoundResult, AkaSerializer<ProtoStageRoundResult>.Serialize(roundResult));
                isRoundResult = true;
            }
            else
            {
                var battleResult = await GetBattleResultFromGameServerAsync(winPlayer, _stage.StageLevelId);


                //if (onBattleResult.BattleResult.TryGetValue(PlayerInfo.Player1UserId, out var playerBattleResult))
                //{
                    //var resBytes = AkaSerializer<ProtoOnBattleResult>.Serialize(playerBattleResult);
                    //Battle.BattleHelper.BattleRecorder.BattleResultRecord(new BattleSendData
                    //{
                    //    Data = onBattleResult,    //ProtoOnGetBattleResult
                    //    MessageType = MessageType.GetBattleResult,
                    //    PlayerType = PlayerType.Player1
                    //});
                    TrySend(session, MessageType.GetBattleResultVirtualLeague, battleResult);
                //}

                var player1UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player1]);
                var player2UnitsInfo = GetAliveAndDeathUnits(Battle.Players[PlayerType.Player2]);
                var player1UnitLevels = player1UnitsInfo.Values;
                var player1Cards = Battle.BattleHelper.BattleController.GetBattleCards()[PlayerType.Player1].AllCards.Select(card => card.CardId);

                Log.Battle.BattleEndResult.Log(RoomId, (byte)PlayerInfo.BattleType, winPlayer.ToString(),
                    currentStageRoundId,
                    roundResult.StageLevelId,
                    roundResult.Round,
                    isRoundResult,
                    PlayerInfo.Player1UserId, PlayerInfo.Player2UserId,
                    Battle.Players[PlayerType.Player1].PlayerIdentifier.Nickname, Battle.Players[PlayerType.Player2].PlayerIdentifier.Nickname,
                    Battle.Status.StartDateTime, Battle.Status.EndDateTime,
                    player1UnitsInfo.Keys,
                    player2UnitsInfo.Keys,
                    player1UnitLevels, player1Cards,
                    0);
            }
        }

        private async Task UpdateSaveDeckInfoAsync(uint userId, ProtoStageRoundResult clearInfo)
        {
            WebServerRequestor webServer = new WebServerRequestor();
            var result = await webServer.RequestAsync(MessageType.SetStageLevelRoomInfo, AkaSerializer<ProtoSetStageLevelRoomInfo>.Serialize(new ProtoSetStageLevelRoomInfo
            {
                MessageType = MessageType.SetStageLevelRoomInfo,
                UserId = userId,
                BattleType = PlayerInfo.BattleType,
                StageLevelId = clearInfo.StageLevelId,
                ClearRound = clearInfo.Round,
                CardStatIdList = clearInfo.CardStatIdList,
                ReplaceCardStatIdList = clearInfo.NextCardStatIdList,
                ProposalTreasureIdList = clearInfo.ProposalTreasureIdList,
                TreasureIdList = clearInfo.TreasureIdList
            }), GetWebServerIp());
        }

        public override void RemoveRoom()
        {
            var nextStageRoundId = _stage.CurrentStageRoundId();
            var nextStageRound = _stage.CurrentRound;

            var battleInfo = new BattleInfo
            {
                BattleType = PlayerInfo.BattleType,
                UserId = PlayerInfo.Player1UserId,
                DeckNum = PlayerInfo.Player1DeckNum,
                StageRoundId = nextStageRoundId,
                StageLevelId = _stage.StageLevelId,
                BattleServerIp = _myIp,
                SessionId = SessionIdByPlayer(PlayerType.Player1)
            };

            base.RemoveRoom();
            if (nextStageRound == 0 || nextStageRoundId == 0 || ModeType.SaveDeck == BattleEnviroment.GetDeckModeType(PlayerInfo.BattleType))
                return;

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await new Controller.Controllers.EnterPvERoom().DoNextRound(battleInfo);
                }
                catch (Exception ex)
                {

                    var errMessage = $"DoNextRound<{battleInfo.UserId},{battleInfo.DeckNum},{battleInfo.StageLevelId},{battleInfo.StageRoundId}> {ex.ToString()}";
                    Log.Debug.Exception(errMessage, ex);
                    Logger.Instance().Error(errMessage);
                }
            });

        }

        protected override void SendBattleResultToClient(byte[] battleResult, string session, PlayerType winPlayer)
        {
            throw new NotImplementedException();
        }
    }
}
