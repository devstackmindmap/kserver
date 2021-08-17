using AkaEnum.Battle;
using CommonProtocol;
using BattleLogic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BattleServer
{
    public abstract class PvERoom : Room
    {
        protected BattleInfo _battleInfo;
        private string _sessionId;

        public PvERoom(string roomId, IBattleInfo battleInfo)
            :base(roomId, battleInfo)
        {
            _battleInfo = battleInfo as BattleInfo;
            _sessionId = _battleInfo.SessionId;
            PlayerInfo.StageRoundId = _battleInfo.StageRoundId;
        }

        public override bool TryEnterGuestPlayer(BattleInfo playerInfo)
        {
            if (Status == RoomStatus.Waiting)
            {
                PlayerInfo.Player2DeckNum = 0;
                PlayerInfo.Player2UserId = 0;
                PlayerInfo.Player2Ready = true;
                Status = RoomStatus.BattleRoomMatched;
                return true;
            }
            return false;
        }

        public override async Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(AkaEnum.ModeType modeType)
        {
            var battleType = PlayerInfo.BattleType;
            return await GetDeckInfo(new List<KeyValuePair<uint, byte>>() { new KeyValuePair<uint, byte> (PlayerInfo.Player1UserId, PlayerInfo.Player1DeckNum) }
            , battleType, modeType);
        }

        public override string SessionIdByPlayer(PlayerType playerType)
        {
            return playerType == PlayerType.Player1 || playerType == PlayerType.All ? _sessionId : null;
        }

        protected override void Send(BattleSendData data)
        {
            TrySend(SessionIdByPlayer(data.PlayerType), data.MessageType, data.Data);
        }
        
        protected override async Task SendBattleResultAsync(PlayerType winPlayer)
        {
            SetPrevSendBattleResult(winPlayer);
            await PlayerEnd(winPlayer);
        }

        protected virtual async Task PlayerEnd(PlayerType winPlayer)
        {
            var session = SessionIdByPlayer(PlayerType.Player1);
            var resBytes = await GetBattleResultFromGameServerAsync(winPlayer, _battleInfo.StageLevelId);

            if (resBytes != null)
                SendBattleResultToClient(resBytes, session, winPlayer);
        }

        public override void ReEnterRoom(uint userId, string sessionId, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            var playerType = SessionIdChangeByReEnterRoom(userId, sessionId);
            Battle.FillCurrentBattleStatus(playerType, PlayerInfo.BattleType, protoCurrentBattleStatus);
            protoCurrentBattleStatus.BackgroundImageId = BackgroundImageId;
            protoCurrentBattleStatus.StageRoundId = PlayerInfo.StageRoundId;

        }

        private PlayerType SessionIdChangeByReEnterRoom(uint userId, string sessionId)
        {
            _sessionId = sessionId;
            return PlayerType.Player1;            
        }
    }
}
