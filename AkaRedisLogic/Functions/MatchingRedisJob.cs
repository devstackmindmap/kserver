using AkaEnum;
using AkaSerializer;
using AkaThreading;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class MatchingRedisJob
    {
        public static async Task AddRoomInfoAsync(IDatabase redis, string member, string roomId, string roomType, string battleServerAddress, 
                                                    string battleServerPort, string sessionId, string key, int userRankPoint, uint wins)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);

            var value = AkaSerializer<RedisValueRoomInfo>.Serialize(new RedisValueRoomInfo
            {
                BattleServerIp = battleServerAddress,
                BattleServerPort = battleServerPort,
                RoomId = roomId,
                SessionId = sessionId,
                Member = member,
                MatchingGroupKey = key,
                UserRankPoint = userRankPoint.ToString(),
                RoomType = roomType,
                Wins = wins.ToString(),
            });

            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                await redis.HashSetAsync(RedisKeyType.HRoomIdList.ToString(), new HashEntry[] { new HashEntry(member, value) });
            }
        }

        public static async Task<RedisValue> GetRoomInfoAsync(IDatabase redis, string member)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);
            RedisValue result;
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                result = await redis.HashGetAsync(RedisKeyType.HRoomIdList.ToString(), member);
            }
            return result;
        }

        public static async Task DeleteRoomInfoAsync(IDatabase redis, string member)
        {
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                await redis.HashDeleteAsync(RedisKeyType.HRoomIdList.ToString(), member);
            }
        }

        public static async Task AddTeamRankScoreAsync(IDatabase redis, string key, string member, int teamRankPoint)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);

            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                await redis.SortedSetAddAsync(key, new SortedSetEntry[] { new SortedSetEntry(member, teamRankPoint) });
            }
        }

        public static async Task<int> GetTeamRankScoreAsync(IDatabase redis, string key, string member)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);

            double? score = null;
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                score = await redis.SortedSetScoreAsync(key, member);
            }
            return (int)(score ?? 0);
        }

        public static async Task<long?> GetRankAsync(IDatabase redis, string key, string member)
        {
            long? rank = null;
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                rank = await redis.SortedSetRankAsync(key, member);
            }
            return (long)(rank ?? 0);
        }

        public static async Task SortedSetRemoveAsync(IDatabase redis, string key, string member)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);

            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                if (false == await redis.SortedSetRemoveAsync(key, member))
                {
                    AkaLogger.Logger.Instance().Info("RemoveFail{0}", member);
                }
            }
        }

        public static async Task<SortedSetEntry[]> GetRangeListAsync(IDatabase redis, string key, long start, long stop)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);
            SortedSetEntry[] result;
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                result = await redis.SortedSetRangeByRankWithScoresAsync(key, start, stop);
            }
            return result;
        }

        public static async Task AddMatchingSessionInfoAsync(IDatabase redis, string sessionId, string member, string roomId, string key)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);

            var value = AkaSerializer<RedisValueMatchingSessionInfo>.Serialize(new RedisValueMatchingSessionInfo
            {
                Member = member,
                RoomId = roomId,
                MatchingGroupKey = key
            });

            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                await redis.HashSetAsync(RedisKeyType.HMatchingSessionInfo.ToString(), new HashEntry[] { new HashEntry(sessionId, value) });
            }
        }

        public static async Task<RedisValue> GetMatchingSessionInfoAsync(IDatabase redis, string sessionId)
        {
            AkaRedis.AkaRedis.ConnectCheck(Server.MatchingServer);
            RedisValue result;
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                result = await redis.HashGetAsync(RedisKeyType.HMatchingSessionInfo.ToString(), sessionId);
            }
            return result;
        }

        public static async Task DeleteMatchingSessionInfoAsync(IDatabase redis, string sessionId)
        {
            using (var balancer = await SemaphoreManager.LockAsync(SemaphoreType.MatchServer2MatchingRedisServerBalancer))
            {
                await redis.HashDeleteAsync(RedisKeyType.HMatchingSessionInfo.ToString(), sessionId);
            }
        }


    }
}
