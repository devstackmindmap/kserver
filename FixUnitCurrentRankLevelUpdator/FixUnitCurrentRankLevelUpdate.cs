using AkaData;
using AkaDB.MySql;
using AkaRedisLogic;
using Common.Entities.Battle;
using Common.Entities.Clan;
using Common.Entities.Season;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FixUnitCurrentRankLevelUpdator
{
    public class FixUnitCurrentRankLevelUpdate
    {
        private DBContext _accountDb;
        private StringBuilder _query = new StringBuilder();
        private uint _cycleCapacity = 300;
        private uint _cycleFrom = 1;
        private uint _cycleTo = 0;
        private uint _startUserId;
        private bool _isUpdate = false;
        private uint _serverSeason;

        public FixUnitCurrentRankLevelUpdate(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task Run(bool isUpdate = false)
        {
            ServerSeason seasonManager = new ServerSeason(_accountDb);
            var seasonInfo = await seasonManager.GetKnightLeagueSeasonInfo();
            _serverSeason = seasonInfo.CurrentSeason;

            _isUpdate = isUpdate;
            var minMaxUserId = await GetMinMaxUserId();
            InitCycleValue(minMaxUserId.minUserId);
            await RepeateUsersJob(minMaxUserId.maxUserId);
            await RepeatClansJob();
            Console.WriteLine("All Done");
        }

        private async Task RepeatClansJob()
        {
            var clanIds = await GetClanIds();
            foreach (var clanId in clanIds)
            {
                var clanRankPoint = await GetClanRankPoint(clanId);
                if (clanRankPoint.HasValue)
                {
                    await SetClanRankPoint(clanId, clanRankPoint.Value);
                }
            }
        }

        private async Task SetClanRankPoint(uint clanId, int rankPoint)
        {
            _query.Clear();
            _query.Append("UPDATE clans SET rankPoint = ").Append(rankPoint)
                .Append(" WHERE clanId=").Append(clanId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<int?> GetClanRankPoint(uint clanId)
        {
            _query.Clear();
            _query.Append("SELECT SUM(currentSeasonRankPoint) FROM knightrun.units WHERE userId in " +
                "(SELECT userId FROM accounts " +
                "WHERE clanName = (SELECT clanName FROM clans WHERE clanId = ").Append(clanId).Append(")); ");
            
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                {
                    return cursor.GetInt32(0);
                }
                return null;
            }
        }

        private async Task<List<uint>> GetClanIds()
        {
            _query.Clear();
            _query.Append("SELECT clanid, clanName FROM clans;");

            List<uint> clanIds = new List<uint>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    clanIds.Add((uint)cursor["clanId"]);
                }
            }
            return clanIds;
        }

        private async Task<(uint minUserId, uint maxUserId)> GetMinMaxUserId()
        {
            _query.Clear();
            _query.Append("SELECT MIN(userId) AS minUserId, MAX(userId) AS maxUserId FROM accounts;");
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return (0, 0);

                return ((uint)cursor["minUserId"], (uint)cursor["maxUserId"]);
            }
        }

        private void InitCycleValue(uint minClanId)
        {
            if (_startUserId > 0)
                _cycleFrom = _startUserId;
            else
                _cycleFrom = minClanId;

            _cycleTo = _cycleCapacity;
        }

        private async Task RepeateUsersJob(uint maxClanId)
        {
            do
            {
                var userInfos = await GetInfos(_cycleFrom, _cycleTo);

                if (userInfos.Count == 0)
                {
                    return;
                }

                await UsersJob(userInfos);

                Console.WriteLine("From:" + _cycleFrom + " To:" + _cycleTo + ", Done");
                _cycleFrom += _cycleCapacity;
                _cycleTo += _cycleCapacity;


            } while (_cycleTo < maxClanId + _cycleCapacity);
        }

        private async Task<List<UserInfo>> GetInfos(uint fromUserId, uint toUserId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM accounts WHERE userId >= ").Append(fromUserId)
                .Append(" AND userId <=").Append(toUserId).Append(";");

            List<UserInfo> userInfos = new List<UserInfo>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    userInfos.Add(new UserInfo
                    {
                        UserId = (uint)cursor["userId"]
                    });
                }
            }

            return userInfos;
        }

        private async Task UsersJob(List<UserInfo> userInfos)
        {
            foreach (var userInfo in userInfos)
            {
                await UserJob(userInfo.UserId);
            }
        }

        private async Task UserJob(uint userId)
        {
            using (var userDb = new DBContext(userId))
            {
                await _accountDb.BeginTransactionCallback(async () =>
                {
                    await userDb.BeginTransactionCallback(async () =>
                    {                        var clanId = await ClanManager.GetClanId(_accountDb, userId);
                        var clanCountryCode = await ClanManager.GetClanInfo(_accountDb, clanId);

                        var unitsChangePoints = await UnitJob(userDb, userId);
                        var accountChangePoints = await AccountJob(userDb, userId);
                        await RedisJob(userId, accountChangePoints.currentSeasonRankPoint, accountChangePoints.nextSeasonRankPoint,
                            unitsChangePoints, _serverSeason, clanId, accountChangePoints.CountryCode, clanCountryCode);
                        return true;
                    });
                    return true;

                });
                _accountDb.Commit();
                userDb.Commit();
            }
        }

        private async Task<(int currentSeasonRankPoint, int nextSeasonRankPoint, string CountryCode)>
            AccountJob(DBContext userDb, uint userId)
        {
            var fixAccountInfo = await GetAccount(userId);
            var currentSeasonRankPoint = await GetSumOfUnits(userDb, userId);
            var nextSeasonRankPoint = await GetSumOfUnitsNext(userDb, userId);
            var currentMaxUserRank = Data.GetUserRankLevelByPoint(currentSeasonRankPoint);

            var maxRankLevel = fixAccountInfo.MaxRankLevel;
            if (currentMaxUserRank > fixAccountInfo.MaxRankLevel)
            {
                maxRankLevel = currentMaxUserRank;
                AkaLogger.Logger.Instance().Info("[FixUserMaxLevel] UserId[" + userId + "] " +
                    "BeforeLevel[" + fixAccountInfo.MaxRankLevel + "] FixLevel[" + currentMaxUserRank + "]");
            }

            var maxRankLevelByMaxRankPoint = Data.GetUserRankLevelByPoint(fixAccountInfo.MaxRankPoint);
            if (maxRankLevel < maxRankLevelByMaxRankPoint)
            {
                maxRankLevel = maxRankLevelByMaxRankPoint;
                AkaLogger.Logger.Instance().Info("[FixUserMaxLevel] UserId[" + userId + "] " +
                    "BeforeLevel[" + fixAccountInfo.MaxRankLevel + "] FixLevel[" + maxRankLevelByMaxRankPoint + "]");
            }

            if (_isUpdate)
                await UpdateAccount(userId, currentSeasonRankPoint, nextSeasonRankPoint, maxRankLevel);

            return (currentSeasonRankPoint, nextSeasonRankPoint, fixAccountInfo.CountryCode);
        }

        private async Task UpdateAccount(uint userId, int currentSeasonRankPoint, int nextSeasonRankPoint, uint maxRankLevel)
        {
            _query.Clear();
            _query.Append("UPDATE accounts SET maxRankLevel=").Append(maxRankLevel)
                .Append(", currentSeasonRankPoint=").Append(currentSeasonRankPoint)
                .Append(", nextSeasonRankPoint=").Append(nextSeasonRankPoint)
                .Append(" WHERE userId=").Append(userId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<FixAccount> GetAccount(uint userId)
        {
            _query.Clear();
            _query.Append("SELECT maxRankLevel, maxRankPoint, countryCode FROM accounts WHERE userId=")
                .Append(userId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                var fixAccount = new FixAccount();
                if (cursor.Read())
                {
                    return new FixAccount
                    {
                        MaxRankLevel = (uint)cursor["maxRankLevel"],
                        MaxRankPoint = (int)cursor["maxRankPoint"],
                        CountryCode = (string)cursor["countryCode"]
                    };
                }
                return new FixAccount();
            }
        }

        private async Task<int> GetSumOfUnits(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT sum(currentSeasonRankPoint) FROM units WHERE userId=")
                .Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;

                return cursor.GetInt32(0);
            }
        }

        private async Task<int> GetSumOfUnitsNext(DBContext userDb, uint userId)
        {
            _query.Clear();
            _query.Append("SELECT sum(nextSeasonRankPoint) FROM units WHERE userId=")
                .Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;

                return cursor.GetInt32(0);
            }
        }

        private async Task<Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)>>
            UnitJob(DBContext userDb, uint userId)
        {
            Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)> unitsChangePoints
                = new Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)>();

            var fixUnits = await GetUnits(userDb, userId);

            foreach (var fixUnit in fixUnits)
            {
                var fixCurrentRankLevel = fixUnit.CurrentRankLevel;
                if (IsNeedFix(fixUnit))
                {
                    fixCurrentRankLevel = GetTrueLevel(fixUnit);
                    await FixUnitCurrentLevel(userDb, userId, fixUnit, fixCurrentRankLevel);
                }

                if (fixCurrentRankLevel > fixUnit.MaxRankLevel)
                    await FixUnitMaxLevel(userDb, userId, fixUnit, fixCurrentRankLevel);

                var nextSeasonRankPoint = RankSeason.GetNextSeasonRankPoint(fixUnit.CurrentSeasonRankPoint);
                if (nextSeasonRankPoint != fixUnit.NextSeasonRankPoint)
                {
                    await FixUnitNextRankPoint(userDb, userId, fixUnit, nextSeasonRankPoint);
                }

                unitsChangePoints.Add(fixUnit.UnitId, (fixUnit.CurrentSeasonRankPoint, nextSeasonRankPoint));
            }

            return unitsChangePoints;
        }

        private async Task<List<FixUnit>> GetUnits(DBContext userDb, uint userId)
        {
            _query.Clear();

            _query.Append("SELECT id, currentRankLevel, currentSeasonRankPoint, maxRankLevel, nextSeasonRankPoint" +
                " FROM units WHERE userId=").Append(userId).Append(";");

            using (var cursor = await userDb.ExecuteReaderAsync(_query.ToString()))
            {
                var fixUnits = new List<FixUnit>();
                while (cursor.Read())
                {
                    fixUnits.Add(new FixUnit
                    {
                        UnitId = (uint)cursor["id"],
                        CurrentRankLevel = (uint)cursor["currentRankLevel"],
                        CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                        MaxRankLevel = (uint)cursor["maxRankLevel"],
                        NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"]
                    });
                }
                return fixUnits;
            }
        }

        private bool IsNeedFix(FixUnit fixUnit)
        {
            if (fixUnit.CurrentRankLevel == 1)
            {
                if (Data.GetUnitRankPoint(fixUnit.CurrentRankLevel).NeedRankPointForNextLevelUp
                    <= fixUnit.CurrentSeasonRankPoint)
                {
                    return true;
                }
            }
            else
            {
                if (Data.GetUnitRankPoint(fixUnit.CurrentRankLevel - 1).NeedRankPointForNextLevelUp
                    > fixUnit.CurrentSeasonRankPoint ||
                    Data.GetUnitRankPoint(fixUnit.CurrentRankLevel).NeedRankPointForNextLevelUp
                    <= fixUnit.CurrentSeasonRankPoint)
                {
                    return true;
                }
            }
            return false;
        }

        private uint GetTrueLevel(FixUnit fixUnit)
        {
            int beforePoint = 0;
            int toPoint = 0;
            for (uint level = 1; level <= 30; level++)
            {
                if (level == 1)
                {
                    toPoint = Data.GetUnitRankPoint(level).NeedRankPointForNextLevelUp - 1;
                }
                else
                {
                    beforePoint = Data.GetUnitRankPoint(level - 1).NeedRankPointForNextLevelUp;
                    toPoint = Data.GetUnitRankPoint(level).NeedRankPointForNextLevelUp - 1;
                }

                if (beforePoint <= fixUnit.CurrentSeasonRankPoint &&
                    toPoint >= fixUnit.CurrentSeasonRankPoint)
                    return level;
            }
            return fixUnit.CurrentRankLevel;
        }

        private async Task FixUnitCurrentLevel(DBContext userDb, uint userId, FixUnit fixUnit, uint fixCurrentRankLevel)
        {
            _query.Clear();
            _query.Append("UPDATE units SET currentRankLevel=").Append(fixCurrentRankLevel)
                .Append(" WHERE userId=").Append(userId).Append(" AND id=")
                .Append(fixUnit.UnitId).Append(";");

            if (_isUpdate)
                await userDb.ExecuteNonQueryAsync(_query.ToString());

            AkaLogger.Logger.Instance().Info("[FixCurrentLevel] UserId[" + userId + "] UnitId[" + fixUnit.UnitId
                + "] BeforeLevel[" + fixUnit.CurrentRankLevel + "] FixLevel[" + fixCurrentRankLevel + "]");
        }

        private async Task FixUnitMaxLevel(DBContext userDb, uint userId, FixUnit fixUnit, uint fixMaxRankLevel)
        {
            _query.Clear();
            _query.Append("UPDATE units SET maxRankLevel=").Append(fixMaxRankLevel)
                .Append(" WHERE userId=").Append(userId).Append(" AND id=")
                .Append(fixUnit.UnitId).Append(";");

            if (_isUpdate)
                await userDb.ExecuteNonQueryAsync(_query.ToString());

            AkaLogger.Logger.Instance().Info("[FixMaxLevel] UserId[" + userId + "] UnitId[" + fixUnit.UnitId
                + "] BeforeLevel[" + fixUnit.MaxRankLevel + "] FixLevel[" + fixMaxRankLevel + "]");
        }

        private async Task FixUnitNextRankPoint(DBContext userDb, uint userId, FixUnit fixUnit, int fixNextRankPoint)
        {
            _query.Clear();
            _query.Append("UPDATE units SET nextSeasonRankPoint=").Append(fixNextRankPoint)
                .Append(" WHERE userId=").Append(userId).Append(" AND id=")
                .Append(fixUnit.UnitId).Append(";");

            if (_isUpdate)
                await userDb.ExecuteNonQueryAsync(_query.ToString());

            AkaLogger.Logger.Instance().Info("[FixUnitNextRankPoint] UserId[" + userId + "] UnitId[" + fixUnit.UnitId
                + "] BeforePoint[" + fixUnit.NextSeasonRankPoint + "] FixPoint[" + fixNextRankPoint + "]");
        }

        private async Task RedisJob(uint userId, double changePoint, double nextSeasonChangePoint,
            Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)> unitsChangePoints,
            uint serverCurrentSeason, uint clanId, string userCountryCode, string clanCountryCode)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();

            await GameBattleRankRedisJob.ChangePointKnightLeagueAsync(redis, userId, changePoint, nextSeasonChangePoint,
                unitsChangePoints, serverCurrentSeason, clanId, userCountryCode, clanCountryCode);
        }
    }

    public class UserInfo
    {
        public uint UserId;
    }

    public class FixAccount
    {
        public uint MaxRankLevel;
        public int MaxRankPoint;
        public string CountryCode;
    }

    public class FixUnit
    {
        public uint UnitId;
        public uint CurrentRankLevel;
        public int CurrentSeasonRankPoint;
        public uint MaxRankLevel;
        public int NextSeasonRankPoint;
    }
}
