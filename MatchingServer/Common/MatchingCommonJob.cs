using AkaRedisLogic;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace MatchingServer
{
    public class MatchingCommonJob
    {
        public static async Task CleanRoomInfo(IDatabase redis, string key, string member, string roomId, string sessionId)
        {
            //GameRedisData.SortedSetRemove(redis, key, member);
            await WaitingRoomManager.ClearTeamRankScoreAsync(roomId, redis, member);
            await MatchingRedisJob.DeleteRoomInfoAsync(redis, member);
            await MatchingRedisJob.DeleteMatchingSessionInfoAsync(redis, sessionId);
            //await MatchingRedisJob.AddBattleStartState(redis, member);

            WaitingRoomManager.RemoveRoom(roomId);
        }
    }
}
