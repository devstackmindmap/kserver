using AkaConfig;
using AkaThreading;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class GameBattleRedisJob
    {
        public static async Task<RedisValueBattlePlayingInfo> GetBattlePlayingInfoAsync(IDatabase redis, string member)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            RedisValue result;

            //ServerState.RunMode
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                result = await redis.HashGetAsync(RedisKeyType.HBattlePlayingInfo.ToString(), member);
                if (result.HasValue)
                {
                    var battlePlayingInfo = AkaSerializer.AkaSerializer<RedisValueBattlePlayingInfo>.Deserialize(result);

                    if (new DateTime(battlePlayingInfo.BattleStartDate).AddMinutes(10) <= DateTime.UtcNow)
                    {
                        await DeleteBattlePlayingInfoAsync(redis, member);
                        return null;
                    }
                    return battlePlayingInfo;
                }
            }
            return null;

        }

        public static async Task DeleteBattlePlayingInfoAsync(IDatabase redis, params string[] members)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.HashDeleteAsync(RedisKeyType.HBattlePlayingInfo.ToString(),  members.Where(member=>member.Length > 0).Select(member=> (RedisValue)member).ToArray());
            }
        }
    }
}
