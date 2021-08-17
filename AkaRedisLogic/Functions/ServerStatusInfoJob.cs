using AkaConfig;
using AkaThreading;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class ServerStatusInfoJob
    {
        public static void SetServerStateInfo(IDatabase redis, RedisKeyType serverStatusKey, int areaIndex, string serverIp, int value)
        {
            redis.SortedSetAdd(serverStatusKey.ToString() + areaIndex.ToString(), new SortedSetEntry[] { new SortedSetEntry(serverIp, value) });
        }

        public static async Task<SortedSetEntry[]> GetServerStateInfo(IDatabase redis, RedisKeyType serverStatusKey, int areaIndex)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.GameServer2GameRedisServerBalancer))
            {
                return await redis.SortedSetRangeByRankWithScoresAsync(serverStatusKey.ToString() + areaIndex.ToString(), 0, 1);
            }
        }

        public static async Task< (int areaIndex, RedisValue[] values)> GetAllServerStateInfo(IDatabase redis, RedisKeyType serverStatusKey, int areaIndex)
        {
            var values = await redis.SortedSetRangeByRankAsync(serverStatusKey.ToString() + areaIndex.ToString());
            return (areaIndex: areaIndex, values: values);

        }

        public static void RemoveServerStateInfo(IDatabase redis, RedisKeyType serverStatusKey, int areaIndex, string serverIp)
        {
            redis.SortedSetRemove(serverStatusKey.ToString() + areaIndex.ToString(), serverIp);
        }

    }
}
