using AkaConfig;
using AkaThreading;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class MatchingServerInfoJob
    {

        public static async Task SetServerStateInfoAsync(IDatabase redis, int areaIndex, int matchingLine, int value)
        {
            await redis.SortedSetAddAsync(RedisKeyType.ZMatchingServerState.ToString() + areaIndex.ToString(), new SortedSetEntry[] { new SortedSetEntry(matchingLine, value) });
        }

        public static async Task<SortedSetEntry[]> GetServerStateInfo(IDatabase redis, int areaIndex)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.GameServer2GameRedisServerBalancer))
            {
                return await redis.SortedSetRangeByRankWithScoresAsync(RedisKeyType.ZMatchingServerState.ToString() + areaIndex.ToString(), 0, 1);
            }
        }

        public static void RemoveServerStateInfo(IDatabase redis, int areaIndex, int matchingLine)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = SemaphoreManager.Lock(SemaphoreType.GameServer2GameRedisServerBalancer))
            {
                redis.SortedSetRemove(RedisKeyType.ZMatchingServerState.ToString() + areaIndex.ToString(), matchingLine);
            }
        }
    }
}
