using AkaRedisLogic;
using AkaSerializer;
using System.Threading.Tasks;

namespace MatchingServer
{
    public static class MatchingSessionClosed
    {
        public static async Task CleanRoomAsync(string sessionId)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            var matchingSessionInfo = await MatchingRedisJob.GetMatchingSessionInfoAsync(redis, sessionId);

            if (matchingSessionInfo.HasValue)
            {
                var sessionInfo = AkaSerializer<RedisValueMatchingSessionInfo>.Deserialize(matchingSessionInfo);
                await MatchingCommonJob.CleanRoomInfo(redis, sessionInfo.MatchingGroupKey, sessionInfo.Member, sessionInfo.RoomId, sessionId);
            }
        }
    }
}
