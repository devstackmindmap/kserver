using AkaConfig;
using AkaEnum;
using AkaSerializer;
using AkaThreading;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class BattleRedisJob
    {
        public static async Task AddBattlePlayingInfoAsync(IDatabase redis, string roomId, string battleServerIp, string battleServerPort, params string[] members )
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);

            var value = AkaSerializer<RedisValueBattlePlayingInfo>.Serialize(new RedisValueBattlePlayingInfo
            {
                BattleServerIp = battleServerIp,
                RoomId = roomId,
                BattleStartDate = DateTime.UtcNow.Ticks,
                BattleServerPort = battleServerPort
            });

            var hashEntries = members.Select(member => new HashEntry(member, value)).ToArray();

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.HashSetAsync(RedisKeyType.HBattlePlayingInfo.ToString(), hashEntries);
            }            
        }

    }
}
