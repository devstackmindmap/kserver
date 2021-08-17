using AkaData;
using AkaSerializer;
using CommonProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;
using Network;
using AkaConfig;
using AkaRedisLogic;

namespace MatchingServer
{
    public class MatchLeagueBattle : MatchCommonBattle
    {
        private int _originTierMatchingId;
        private int _currentTierMatchingId;
        private DataRankTierMatching _tierMatchingInfo;
        private int _teamRankPoint;
        private int _userRankPoint;
        private int _minMatchingId;
        private int _groupCode;
        private DateTime _aiMatchingDateTime;

        public MatchLeagueBattle(ProtoTryMatching protoTryMatching, int groupCode, int matchingLine) : base(protoTryMatching, matchingLine)
        {
            RedisDBIndex = 0;
            BattleType = AkaEnum.Battle.BattleType.LeagueBattle;
            AiBattleType = AkaEnum.Battle.BattleType.LeagueBattleAi;
            _minMatchingId = (int)Data.GetConstant(DataConstantType.MIN_MATCHING_ID).Value;
            _groupCode = groupCode;
        }

        public override async Task InitMatchingPoint()
        {
            WebServerRequestor webServer = new WebServerRequestor(AkaThreading.SemaphoreType.MatchServer2GameServerRequestBalancer);
            var rawProtoTeamRankPoint = await webServer.RequestAsync(MessageType.GetRankPoint, AkaSerializer<ProtoRankPoint>.Serialize(new ProtoRankPoint
            {
                UserId = UserId,
                DeckNum = DeckNum,
                DeckModeType = ModeType.PVP,
                RankType = RankType.AllUnitRankPoint
            }), $"http://{Config.MatchingServerConfig.GameServer.ip}:{Config.MatchingServerConfig.GameServer.port}/");
            var protoOnTeamRankPoint = AkaSerializer<ProtoOnRankPoint>.Deserialize(rawProtoTeamRankPoint);
            _teamRankPoint = protoOnTeamRankPoint.TeamRankPoint;
            _userRankPoint = protoOnTeamRankPoint.UserRankPoint;
            Wins = protoOnTeamRankPoint.Wins;

            if (GetMyTeamRankPoint() < 0)
                return;

            _originTierMatchingId = Data.GetRankTierMatchingId(_teamRankPoint);
            _currentTierMatchingId = _originTierMatchingId;
            _tierMatchingInfo = Data.GetRankTierMatching(_originTierMatchingId);

            MatchingKey = KeyMaker.GetMatchingGroupScoreList(_matchingLine, _currentTierMatchingId.ToString(), _groupCode);
        }

        public override async Task AddMatchingScore(string roomId, IDatabase redis)
        {
            await WaitingRoomManager.AddTeamRankScoreAsync(roomId, redis, MemberKey, MatchingKey, GetMyTeamRankPoint());
        }

        public override async Task<SortedSetEntry?> GetTargetAsync(IDatabase redis)
        {
            var myRank = await WaitingRoomManager.GetMyRank(redis, MatchingKey, MemberKey);
            if (!myRank.HasValue)
                return null;

            var searchingRange = GetSearchRange(myRank.Value);
            return await WaitingRoomManager.GetSearchingTargetAsync(redis, MatchingKey, searchingRange, MemberKey);
        }

        public override int GetMyUserRankPoint()
        {
            return _userRankPoint;
        }

        public override int GetAiUserRankPoint()
        {
            Data.GetUserRankLevelIdFromPoint(_userRankPoint, out var nextPoint, out var minPoint);
            var aiRankPoint = minPoint + AkaRandom.Random.Next(nextPoint - minPoint);

            if (minPoint == 0)
            {
                var firstRankUnitPoint = Data.GetUnitRankPoint(1).WinPoint;
                if (firstRankUnitPoint <= 0)
                    firstRankUnitPoint = 1;
                aiRankPoint -= (aiRankPoint % firstRankUnitPoint);
            }
            return aiRankPoint;
        }

        public override int GetMyTeamRankPoint()
        {
            return _teamRankPoint;
        }

        public override int GetAiTeamRankPoint()
        {
            var noneLosePoint = Data.GetUnitRankPointForZeroPoint() * 3;
            var firstRankUnitPoint = Data.GetUnitRankPoint(1).WinPoint;
            if (firstRankUnitPoint <= 0)
                firstRankUnitPoint = 1;

            var startMatchingPoint = _originTierMatchingId - 1 <= 0 ? 0 : Data.GetRankTierMatching(_originTierMatchingId - 1 ).TeamRankPointForMatching;
            
            var range = _tierMatchingInfo.TeamRankPointForMatching;
            
            var aiRankPoint = startMatchingPoint;
            if (range > startMatchingPoint)
                aiRankPoint = startMatchingPoint + AkaRandom.Random.Next(range - startMatchingPoint);
            if (noneLosePoint > aiRankPoint)
                aiRankPoint -= (aiRankPoint % firstRankUnitPoint);
            return aiRankPoint;
        }

        public override void SetMatchingWaitingSecond()
        {
            if (Data.GetRankTierMatching(_originTierMatchingId).Priority == MatchingPriorityType.Ai)
                _aiMatchingDateTime = DateTime.UtcNow;
            else
                _aiMatchingDateTime = DateTime.UtcNow.AddMilliseconds(_tierMatchingInfo.AiMatchingWaitingMillisecond);
        }

        public override bool EnableTryMatching()
        {
            return  _aiMatchingDateTime > DateTime.UtcNow;
        }

        public override bool EnableExpansionSearch()
        {
            return _currentTierMatchingId != _minMatchingId ;
        }

        public override async Task UpdateSearchExpansion(string roomId, IDatabase redis)
        {
            _currentTierMatchingId = Math.Max(_currentTierMatchingId - 1, _minMatchingId);
            MatchingKey = KeyMaker.GetMatchingGroupScoreList(_matchingLine, _currentTierMatchingId.ToString(), _groupCode);

            await AddMatchingScore(roomId, redis);
        }

        public override SearchingRange GetSearchRange(long rankPoint)
        {
            return new SearchingRange(rankPoint)
            {
                BeforeMin = rankPoint - 5,
                BeforeMax = rankPoint - 1,
                AfterMin = rankPoint + 1,
                AfterMax = rankPoint + 5
            };
        }

        public override bool GetAiMatchRoundInfo(out uint stageRoundId)
        {
            stageRoundId = AkaRandom.Random.ChooseElementRandomlyInCount(_tierMatchingInfo.StageRoundIdList);
            return true;
        }

        public override string GetRoomType()
        {
            return BattleType.ToString();
        }
    }
}
