using AkaConfig;
using AkaData;
using AkaThreading;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AkaRedisLogic
{
    public class GameBattleRankRedisJob
    {
        public static async Task<bool> ChangePointKnightLeagueAsync(IDatabase redis, 
            uint userId, double changePoint, double nextSeasonChangePoint,
            Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)> unitsChangePoints,
            uint serverCurrentSeason, uint clanId, string userCountryCode, string clanCountryCode)
        {
            if (changePoint == 0)
                return false;

            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetIncrementAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason), userId, changePoint);
                await redis.SortedSetIncrementAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason + 1), userId, nextSeasonChangePoint);

                if (false == string.IsNullOrEmpty(userCountryCode))
                {
                    await redis.SortedSetIncrementAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason, userCountryCode), userId, changePoint);
                    await redis.SortedSetIncrementAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason + 1, userCountryCode), userId, nextSeasonChangePoint);
                }

                if (clanId != 0)
                {
                    await redis.SortedSetIncrementAsync(KeyMaker.GlobalClanRankBoard(serverCurrentSeason), clanId, changePoint);
                    if (false == string.IsNullOrEmpty(userCountryCode))
                    {
                        await redis.SortedSetIncrementAsync(KeyMaker.GlobalClanRankBoard(serverCurrentSeason, clanCountryCode), clanId, changePoint);
                    }
                }

                foreach (var unitChangePoints in unitsChangePoints)
                {
                    await redis.SortedSetIncrementAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason, unitChangePoints.Key)
                        , userId, unitChangePoints.Value.UnitsChangePoint);
                    await redis.SortedSetIncrementAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason + 1, unitChangePoints.Key)
                        , userId, unitChangePoints.Value.UnitsNextSeasonChangePoint);

                    if (false == string.IsNullOrEmpty(userCountryCode))
                    {
                        await redis.SortedSetIncrementAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason, unitChangePoints.Key, userCountryCode)
                        , userId, unitChangePoints.Value.UnitsChangePoint);
                        await redis.SortedSetIncrementAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason + 1, unitChangePoints.Key, userCountryCode)
                            , userId, unitChangePoints.Value.UnitsNextSeasonChangePoint);
                    }
                }
            }
            return true;
        }

        public static async Task<long?> GetRankKnightLeagueAsync(IDatabase redis, uint userId, uint serverCurrentSeason)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRankAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason), userId, Order.Descending);
            }
        }

        public static async Task<SortedSetEntry[]> GetTopRanksKnightLeagueUserAsync(IDatabase redis, uint serverCurrentSeason, string countryCode)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRangeByRankWithScoresAsync(KeyMaker.GlobalUserRankBoard(serverCurrentSeason, countryCode), 0, 
                    (long)(Data.GetConstant(AkaEnum.DataConstantType.RANK_BOARD_LIST_MAX_COUNT).Value - 1), Order.Descending);
            }
        }

        public static async Task<SortedSetEntry[]> GetTopRanksKnightLeagueClanAsync(IDatabase redis, uint serverCurrentSeason, string countryCode)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRangeByRankWithScoresAsync(KeyMaker.GlobalClanRankBoard(serverCurrentSeason, countryCode), 0,
                    (long)(Data.GetConstant(AkaEnum.DataConstantType.RANK_BOARD_LIST_MAX_COUNT).Value - 1), Order.Descending);
            }
        }

        public static async Task<SortedSetEntry[]> GetTopRanksKnightLeagueUnitAsync(IDatabase redis, uint unitId, uint serverCurrentSeason, string countryCode)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRangeByRankWithScoresAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason, unitId, countryCode), 0,
                    (long)(Data.GetConstant(AkaEnum.DataConstantType.RANK_BOARD_LIST_MAX_COUNT).Value - 1), Order.Descending);
            }
        }

        public static async Task<long?> GetRankKnightLeagueUnitAsync(IDatabase redis, uint userId, uint unitId, uint serverCurrentSeason)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRankAsync(KeyMaker.GlobalUnitRankBoard(serverCurrentSeason, unitId), userId, Order.Descending);
            }
        }

        public static async Task<bool> SetRankKnightLeagueUnitAsync(IDatabase redis, uint userId, uint unitId, int rankPoint, uint season)
        {
            if (rankPoint == 0)
                return false;

            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetAddAsync(KeyMaker.GlobalUnitRankBoard(season, unitId), userId, rankPoint);
            }
        }

        public static async Task<bool> SetRankKnightLeagueUserAsync(IDatabase redis, uint userId, int rankPoint, uint season)
        {
            if (rankPoint == 0)
                return false;

            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetAddAsync(KeyMaker.GlobalUserRankBoard(season), userId, rankPoint);
            }
        }

        public static async Task<bool> SetRankKnightLeagueClanAsync(IDatabase redis, uint clanId, int rankPoint, uint season)
        {
            if (rankPoint == 0)
                return false;

            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetAddAsync(KeyMaker.GlobalClanRankBoard(season), clanId, rankPoint);
            }
            return true;
        }

        public static async Task<bool> IncrementRankKnightLeagueClanAsync(IDatabase redis, uint clanId, int rankPoint, uint season)
        {
            if (rankPoint == 0)
                return false;

            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetIncrementAsync(KeyMaker.GlobalClanRankBoard(season), clanId, rankPoint);
            }
            return true;
        }

        public static async Task<bool> RemoveRankKnightLeagueClanAsync(IDatabase redis, uint clanId, uint season)
        {
            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetRemoveAsync(KeyMaker.GlobalClanRankBoard(season), clanId);
            }
            return true;
        }

        public static async Task<bool> SetRankKnightLeagueUnitCountryAsync(IDatabase redis, uint userId, uint unitId, int rankPoint, uint season, string countryCode)
        {
            if (rankPoint == 0)
                return false;

            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetAddAsync(KeyMaker.GlobalUnitRankBoard(season, unitId, countryCode), userId, rankPoint);
            }
        }

        public static async Task<bool> SetRankKnightLeagueUserCountryAsync(IDatabase redis, uint userId, int rankPoint, uint season, string countryCode)
        {
            if (rankPoint == 0)
                return false;

            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetAddAsync(KeyMaker.GlobalUserRankBoard(season, countryCode), userId, rankPoint);
            }
        }

        public static async Task<bool> SetRankKnightLeagueClanCountryAsync(IDatabase redis, uint clanId, int rankPoint, uint season, string countryCode)
        {
            if (rankPoint == 0)
                return false;

            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetAddAsync(KeyMaker.GlobalClanRankBoard(season, countryCode), clanId, rankPoint);
            }
            return true;
        }

        public static async Task<bool> IncrementRankKnightLeagueClanCountryAsync(IDatabase redis, uint clanId, int rankPoint, uint season, string countryCode)
        {
            if (rankPoint == 0)
                return false;

            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                await redis.SortedSetIncrementAsync(KeyMaker.GlobalClanRankBoard(season, countryCode), clanId, rankPoint);
            }
            return true;
        }

        public static async Task<bool> RemoveRankKnightLeagueUnitCountryAsync(IDatabase redis, uint userId, uint unitId, uint season, string countryCode)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRemoveAsync(KeyMaker.GlobalUnitRankBoard(season, unitId, countryCode), userId);
            }
        }

        public static async Task<bool> RemoveRankKnightLeagueUserCountryAsync(IDatabase redis, uint userId, uint season, string countryCode)
        {
            AkaRedis.AkaRedis.ConnectCheck(Config.Server);
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRemoveAsync(KeyMaker.GlobalUserRankBoard(season, countryCode), userId);
            }
        }

        public static async Task<bool> RemoveRankKnightLeagueClanCountryAsync(IDatabase redis, uint clanId, uint season, string countryCode)
        {
            if (false == AkaRedis.AkaRedis.ConnectCheck(Config.Server))
                return false;

            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetRemoveAsync(KeyMaker.GlobalClanRankBoard(season, countryCode), clanId);
            }
        }

        public static async Task<double?> GetScoreRankKnightLeagueUserAsync(IDatabase redis, uint userId, uint season)
        {
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetScoreAsync(KeyMaker.GlobalUserRankBoard(season), userId);
            }
        }

        public static async Task<double?> GetScoreRankKnightLeagueUnitAsync(IDatabase redis, uint userId, uint unitId, uint season)
        {
            using (var balancer = await SemaphoreManager.LockAsync(AkaRedis.AkaRedis.GetBattlePlayingInfoSemaphoreType()))
            {
                return await redis.SortedSetScoreAsync(KeyMaker.GlobalUnitRankBoard(season, unitId, ""), userId);
            }
        }
    }
}
