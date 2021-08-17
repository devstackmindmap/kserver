using AkaSerializer;
using CommonProtocol;
using StackExchange.Redis;
using System.Threading.Tasks;
using AkaEnum.Battle;
using AkaRedisLogic;
using AkaEnum;
using AkaData;
using System;
using Common.Entities.ServerStatus;

namespace MatchingServer
{
    public class TryMatching : BaseController
    {
        private MatchCommonBattle _matchInfo;
        private int _expandableNumber = (int)Data.GetConstant(DataConstantType.MATCHING_SEARCH_EXPANSION_INTERVAL_MAX).Value;
        private int _expandCount = 0;
        private int _areaIndex;
        private int _matchingLine;
        private IDatabase _redis;

        public TryMatching(int areaIndex, int matchingLine)
        {
            _areaIndex = areaIndex;
            _matchingLine = matchingLine;
        }

        public override async Task DoPipeline(NetworkSession session, BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoTryMatching;
            _redis = AkaRedis.AkaRedis.GetDatabase();
            if (await ServerStatus.IsServerDown(_redis)
                && false == await ServerStatus.IsDeveloperIp(_redis, session.RemoteEndPoint.Address.ToString()))
            {
                SendServerDown(session);
                return;
            }

            _matchInfo = MatchingFactory.CreateMatching(req, _matchingLine);
            await _matchInfo.InitMatchingPoint();

            if (_matchInfo == null || _matchInfo.GetMyTeamRankPoint() < 0)
            {
                SendInvalidMatching(session);
                AkaLogger.Log.Matching.Result.Log(req.UserId, (byte)req.BattleType, "Not supported BattleType");
                return;
            }
            
            string roomId = "";

            using (var lockEnter = await LockObject.LockMakeRoomAsync())
            {
                roomId = await MakeRoom(session.SessionID);
            }

            AkaLogger.Log.Matching.TryMatching.Log(req.UserId, roomId, (byte)_matchInfo.BattleType,_matchInfo.DeckNum, (byte)_matchInfo.DeckModeType,_matchInfo.BattleServerIp,
                                                    _matchInfo.MatchingKey, _matchInfo.MemberKey, _matchInfo.GetMyTeamRankPoint(), _matchInfo.GetMyUserRankPoint());
            _matchInfo.SetMatchingWaitingSecond();

            var expandcount = (int)Data.GetConstant(DataConstantType.MATCHING_SEARCH_EXPANSION_INTERVAL_MAX).Value;

            try
            {
                while (_matchInfo.EnableTryMatching())
                {
                    await Task.Delay(1);
                    using (var lockEnter = await LockObject.LockEnterRoomAsync())
                    {
                        var redisValue = await MatchingRedisJob.GetRoomInfoAsync(_redis, _matchInfo.MemberKey);
                        if (!redisValue.HasValue)
                            return;

                        await ExpansionSearchRange(roomId);

                        var pickedRoomInfo = await GetPickedRoomInfoAsync();
                        if (!pickedRoomInfo.HasValue)
                            continue;

                        var roomInfo = AkaSerializer<RedisValueRoomInfo>.Deserialize(pickedRoomInfo);
                        if (_matchInfo.GetRoomType() != roomInfo.RoomType)
                            continue;

                        if (!WaitingRooms.Instance.ContainsKey(roomId))
                            return;

                        if (!WaitingRoomManager.TryEnterRoom(roomInfo.RoomId))
                            continue;

                        var message = $"Matching[{_matchInfo.MemberKey}][{roomInfo.Member}]-{roomInfo.RoomId}, ip:{roomInfo.BattleServerIp}";
                        AkaLogger.Log.Matching.Result.Log(message, "MatchingSuccess");
                        AkaLogger.Logger.Instance().Info(message);
                        await MatchingSuccess(session, roomId, roomInfo);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                AkaLogger.Log.Debug.Exception("TryMatching-Fail" , e);
                await MatchingFail(session, roomId);
            }
            

            uint stageRoundId = 0;
            if (_matchInfo.GetAiMatchRoundInfo(out stageRoundId))
            {
                AkaLogger.Logger.Instance().Info("Ai Matching[{0}][{1}]", _matchInfo.MemberKey, stageRoundId.ToString());
                AkaLogger.Log.Matching.Result.Log(_matchInfo.MemberKey, "MatchingSuccessAi");
                await MatchingSuccess(session, roomId, stageRoundId);
            }
            else
            {
                //not called
                AkaLogger.Logger.Instance().Info("Match Fail[{0}]", _matchInfo.MemberKey);
                await MatchingFail(session, roomId);
            }
            return;
        }


        private void InitMatchingConfiguration(ProtoTryMatching matchingInfo)
        {
            switch (matchingInfo.BattleType)
            {
                case BattleType.LeagueBattle:
                    //case BattleType.LeagueBattle_Ai: //호출될 수 없음
                    _matchInfo = new MatchLeagueBattle(matchingInfo, matchingInfo.GroupCode, _matchingLine);
                    break;
                case BattleType.FriendlyBattle when matchingInfo is ProtoTryFvFMatching:
                    _matchInfo = new MatchFriendlyBattle(matchingInfo as ProtoTryFvFMatching, _matchingLine);
                    break;
            }

        }

        private async Task<RedisValue> GetPickedRoomInfoAsync()
        {
            var matchingTarget = await _matchInfo.GetTargetAsync(_redis);
            if (!IsValidMatchingTarget(matchingTarget, _matchInfo.MemberKey))
                return RedisValue.Null;

            return await MatchingRedisJob.GetRoomInfoAsync(_redis, matchingTarget.Value.Element);
        }

        private async Task ExpansionSearchRange(string roomId)
        {
            if (_expandableNumber <= _expandCount)
                return;

            if (_matchInfo.IsTimeToSearchExpansion() && _matchInfo.EnableExpansionSearch())
            {
                await _matchInfo.UpdateSearchExpansion(roomId, _redis);
                _expandCount++;
            }
        }

        private async Task<string> MakeRoom(string sessionId)
        {
            var memberKey = _matchInfo.MemberKey;
            var matchingKey = _matchInfo.MatchingKey;
            var battleServerIp = _matchInfo.BattleServerIp;
            var battleServerPort = _matchInfo.BattleServerPort;
            var myUserRankPoint = _matchInfo.GetMyUserRankPoint();
            var roomType = _matchInfo.GetRoomType();
            var wins = _matchInfo.Wins;

            var roomId = WaitingRoomManager.MakeRoom();

            await MatchingRedisJob.AddRoomInfoAsync(_redis, memberKey, roomId, roomType, battleServerIp, battleServerPort, sessionId, matchingKey, myUserRankPoint, wins);
            await MatchingRedisJob.AddMatchingSessionInfoAsync(_redis, sessionId, memberKey, roomId, matchingKey);

            await _matchInfo.AddMatchingScore(roomId, _redis);
            return roomId;
        }

        private bool IsValidMatchingTarget(SortedSetEntry? matchingTarget, string member)
        {
            if (!matchingTarget.HasValue)
                return false;

            if (member == matchingTarget.Value.Element)
                return false;

            return true;
        }


        private async Task MatchingSuccess(NetworkSession session, string roomId, RedisValueRoomInfo roomInfo)
        {
            var matchingKey = _matchInfo.MatchingKey;
            var memberKey = _matchInfo.MemberKey;

            var enemyScorePoint = await MatchingRedisJob.GetTeamRankScoreAsync(_redis, matchingKey, roomInfo.Member);

            await MatchingCommonJob.CleanRoomInfo(_redis, matchingKey, memberKey, roomId, session.SessionID);
            await MatchingCommonJob.CleanRoomInfo(_redis, matchingKey, roomInfo.Member, roomInfo.RoomId, roomInfo.SessionId);
            SendSuccess(session, roomInfo, enemyScorePoint);
        }

        private async Task MatchingSuccess(NetworkSession session, string roomId, uint stageRoundId)
        {
            await MatchingCommonJob.CleanRoomInfo(_redis, _matchInfo.MatchingKey, _matchInfo.MemberKey, roomId, session.SessionID);
            SendSuccessForAi(session, stageRoundId, _matchInfo.BattleServerIp, _matchInfo.BattleServerPort);
        }

        private async Task MatchingFail(NetworkSession session, string roomId)
        {
            await MatchingCommonJob.CleanRoomInfo(_redis, _matchInfo.MatchingKey, _matchInfo.MemberKey, roomId, session.SessionID);
            SendFail(session);
        }

        private void SendSuccessForAi(NetworkSession session, uint stageRoundId, string battleServerIp, string battleServerPort)
        {
            var enemyRankPoint = _matchInfo.GetAiTeamRankPoint();
            //var enemyUserRankPoint = _matchInfo.GetAiUserRankPoint();
            var enemyUserRankPoint = 0;
            if (enemyUserRankPoint <= enemyRankPoint)
                enemyUserRankPoint = enemyRankPoint;
            var data = AkaSerializer<ProtoMatchingSuccess>.Serialize(new ProtoMatchingSuccess
            {
                MessageType = MessageType.MatchingSuccess,
                StageRoundId = stageRoundId,
                BattleServerIp = battleServerIp,
                BattleServerPort = battleServerPort,
                EnemyRankPoint = enemyRankPoint,
                EnemyUserRankPoint = enemyUserRankPoint,
                BattleType = _matchInfo.AiBattleType,
                MyWins = _matchInfo.Wins,
            });
            session.Send(MessageType.MatchingSuccess, data);

            _matchInfo.SendSuccessLog( enemyRankPoint, enemyUserRankPoint, stageRoundId);

        }

        private void SendSuccess(NetworkSession session, RedisValueRoomInfo roomInfo, int enemyRankPoint)
        {
            uint.TryParse(roomInfo.Wins, out var myWins);
            var matchData = new ProtoMatchingSuccess
            {
                MessageType = MessageType.MatchingSuccess,
                RoomId = roomInfo.RoomId,
                BattleServerIp = roomInfo.BattleServerIp,
                BattleServerPort = roomInfo.BattleServerPort,
                EnemyRankPoint = _matchInfo.GetMyTeamRankPoint(),
                EnemyUserRankPoint = _matchInfo.GetMyUserRankPoint(),
                BattleType = _matchInfo.BattleType,
                MyWins = myWins,
                EnemyWins =_matchInfo.Wins
            };
            var data = AkaSerializer<ProtoMatchingSuccess>.Serialize(matchData);
            MainServer.Instance.GetSessionByID(roomInfo.SessionId).Send(MessageType.MatchingSuccess, data);

            int.TryParse(roomInfo.UserRankPoint, out var enemyUserRankPoint);
            matchData.EnemyRankPoint = enemyRankPoint;
            matchData.EnemyUserRankPoint = enemyUserRankPoint;
            matchData.MyWins = _matchInfo.Wins;
            matchData.EnemyWins = myWins;
            data = AkaSerializer<ProtoMatchingSuccess>.Serialize(matchData);
            session.Send(MessageType.MatchingSuccess, data);

            _matchInfo.SendSuccessLog(roomInfo.RoomId, roomInfo.BattleServerIp, enemyRankPoint, enemyUserRankPoint, roomInfo.Member, roomInfo.MatchingGroupKey);
        }

        private void SendFail(NetworkSession session)
        {
            session.Send(MessageType.MatchingFail, AkaSerializer<ProtoMatchingFail>.Serialize(new ProtoMatchingFail()
            {
                MessageType = MessageType.MatchingFail
            }));
        }

        private void SendServerDown(NetworkSession session)
        {
            session.Send(MessageType.MatchingBattleServerDown, AkaSerializer<ProtoMatchingBattleServerDown>.Serialize(new ProtoMatchingBattleServerDown()
            {
                MessageType = MessageType.MatchingBattleServerDown
            }));
        }

        private async Task SendTryReEnterRoom(NetworkSession session, uint userId)
        {
            var battlePlayingInfo = await GetBattlePlayingInfo(userId);
            session.Send(MessageType.MatchingTryReEnterRoom, AkaSerializer<ProtoMatchingTryReEnterRoom>.Serialize(new ProtoMatchingTryReEnterRoom()
            {
                MessageType = MessageType.MatchingTryReEnterRoom,
                BattlePlayingInfo = battlePlayingInfo
            }));
        }


        private void SendInvalidMatching(NetworkSession session)
        {
            session.Send(MessageType.MatchingFail, AkaSerializer<ProtoMatchingFail>.Serialize(new ProtoMatchingFail()
            {
                MessageType = MessageType.MatchingFail
            }));
        }


        private async Task<ProtoBattlePlayingInfo> GetBattlePlayingInfo(uint userId)
        {
            var member = KeyMaker.GetMemberKey(userId);
            var battlePlayingInfo = await GameBattleRedisJob.GetBattlePlayingInfoAsync(_redis, member);
            var protoBattlePlayingInfo = new ProtoBattlePlayingInfo();
            if (battlePlayingInfo != null)
            {
                var battleServerPort = int.TryParse(battlePlayingInfo.BattleServerPort, out var v) ? v : 0;
                protoBattlePlayingInfo.BattleServerIp = battlePlayingInfo.BattleServerIp;
                protoBattlePlayingInfo.BattleServerPort = battleServerPort;
                protoBattlePlayingInfo.RoomId = battlePlayingInfo.RoomId;
            }
            return protoBattlePlayingInfo;
        }
    }
}
