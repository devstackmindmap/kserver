using AkaEnum.Battle;
using BattleLogic;
using CommonProtocol;
using System.Collections.Generic;
using AkaUtility;
using AkaSerializer;
using System.Linq;
using System.Threading.Tasks;
using AkaLogger;

namespace BattleServer
{
    public abstract class PvPRoom : Room
    {
        private BattleInfo _battleInfo;

        private readonly Dictionary<PlayerType, string> _sessionIds = new Dictionary<PlayerType, string>(PlayerTypeComparer.Comparer)
        {
            {PlayerType.Player1, string.Empty},
            {PlayerType.Player2, string.Empty}
        };

        public PvPRoom(string roomId, IBattleInfo battleInfo)
            : base(roomId, battleInfo)
        {
            _battleInfo = battleInfo as BattleInfo;
            _sessionIds[PlayerType.Player1] = _battleInfo.SessionId;
        }

        public override bool TryEnterGuestPlayer(BattleInfo playerInfo)
        {
            if (Status == RoomStatus.Waiting)
            {
                PlayerInfo.Player2DeckNum = playerInfo.DeckNum;
                PlayerInfo.Player2UserId = playerInfo.UserId;
                Status = RoomStatus.BattleRoomMatched;
                _sessionIds[PlayerType.Player2] = playerInfo.SessionId;
                return true;
            }
            return false;
        }

        protected override async Task SendBattleResultAsync(PlayerType winPlayer)
        {
            SetPrevSendBattleResult(winPlayer);
            var resBytes = await GetBattleResultFromGameServerAsync(winPlayer, 0);
            SendBattleResultToClient(resBytes, null, winPlayer);
        }

        public override async Task<ProtoOnGetDeckWithDeckNum> GetDeckInfo(AkaEnum.ModeType modeType)
        {
            var battleType = PlayerInfo.BattleType;
                      
            List<KeyValuePair<uint, byte>> userIdAndDeckNums = new List<KeyValuePair<uint, byte>>()
            {
                new KeyValuePair<uint, byte>(PlayerInfo.Player1UserId, PlayerInfo.Player1DeckNum),
                new KeyValuePair<uint, byte>(PlayerInfo.Player2UserId, PlayerInfo.Player2DeckNum),
            };
            return await GetDeckInfo(userIdAndDeckNums, battleType, modeType);
        }

        public override string SessionIdByPlayer(PlayerType playerType)
        {
            return _sessionIds[playerType];
        }
        
        protected override void Send(BattleSendData data)
        {
            if (data.PlayerType == PlayerType.All)
            {
                foreach (var sessionId in _sessionIds)
                {
                    TrySend(sessionId.Value, data.MessageType, data.Data);
                }
            }
            else
            {
                TrySend(_sessionIds[data.PlayerType], data.MessageType, data.Data);
            }
        }

        public override void ReEnterRoom(uint userId, string sessionId, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            var playerType = SessionIdChangeByReEnterRoom(userId, sessionId);
            Battle.FillCurrentBattleStatus(playerType, PlayerInfo.BattleType, protoCurrentBattleStatus);
            protoCurrentBattleStatus.BackgroundImageId = BackgroundImageId;
        }

        private PlayerType SessionIdChangeByReEnterRoom(uint userId, string sessionId)
        {
            if (PlayerInfo.Player1UserId == userId)
            {
                _sessionIds[PlayerType.Player1] = sessionId;
                return PlayerType.Player1;
            }
            else
            {
                _sessionIds[PlayerType.Player2] = sessionId;
                return PlayerType.Player2;
            }
        }
    }
}
