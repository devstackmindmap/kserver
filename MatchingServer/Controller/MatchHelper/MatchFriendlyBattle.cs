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
using System.Text;

namespace MatchingServer
{
    public class MatchFriendlyBattle : MatchCommonBattle
    {
        private DateTime _matchingWaitingSecond;
        private uint _friendUserId;

        public MatchFriendlyBattle(ProtoTryFvFMatching protoTryMatching, int matchingLine) : base(protoTryMatching, matchingLine)
        {
            RedisDBIndex = 0;
            BattleType = AkaEnum.Battle.BattleType.FriendlyBattle;
            AiBattleType = AkaEnum.Battle.BattleType.FriendlyBattle;
            _friendUserId = protoTryMatching.FriendUserId;
        }

        public override async Task InitMatchingPoint()
        {
            var matchingId = new StringBuilder();
            if (UserId > _friendUserId)
                matchingId.Append(UserId).Append(_friendUserId);
            else
                matchingId.Append(_friendUserId).Append(UserId);

            MatchingKey = KeyMaker.GetFvFMatchingGroupScoreList(matchingId.ToString());
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
            return 0;
        }

        public override int GetAiUserRankPoint()
        {
            return 0;
        }

        public override int GetMyTeamRankPoint()
        {
            return 0;
        }

        public override int GetAiTeamRankPoint()
        {
            return 0;
        }

        public override void SetMatchingWaitingSecond()
        {
            _matchingWaitingSecond = DateTime.UtcNow.AddMilliseconds((int)Data.GetConstant(DataConstantType.AI_MATCHING_WAITING_MILLISECOND).Value);
        }

        public override bool EnableTryMatching()
        {
            return _matchingWaitingSecond > DateTime.UtcNow;
        }

        public override bool EnableExpansionSearch()
        {
            return false;
        }

        public override async Task UpdateSearchExpansion(string roomId, IDatabase redis)
        {
        }

        public override SearchingRange GetSearchRange(long rankPoint)
        {
            return new SearchingRange(rankPoint)
            {
                BeforeMin = rankPoint-1,
                BeforeMax = rankPoint-1,
                AfterMin = rankPoint+1,
                AfterMax = rankPoint+1
            };
        }

        public override bool GetAiMatchRoundInfo(out uint stageRoundId)
        {
            stageRoundId = 0;
            return false;
        }

        public override string GetRoomType()
        {
            return MatchingKey;
        }
    }
}
