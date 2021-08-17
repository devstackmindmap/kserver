using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// _common 테이블  commonId 2 (ServerCommonTable.ClanRankBoardServerSeason) 의 commonValue로 이 Updator의 완료여부를 체크, 확인한다.
// 전체 clanId 의 min, max 값을 가져온다.
// _cycleFrom 값을 설정한다.
// Updator 실행 시 시작 clanId를 인자 값으로 받는데 이 인자 값이 _cycleFrom 값에 영향을 준다. 인자 값이 없다면 clanId min 값이 _cycleFrom 으로 쎄팅된다. 
// _cycleTo 값은 _cyclyCapacity로 쎄팅한다.

//***클랜작업*********************************************************************************************
// 쎄팅 된 _cycleFrom, _cycleTo 값으로 실제 클랜 정보를 가져와서 각 클랜 별 작업(ClansJob, ClanJob)을 수행한다.
// _cycleFrom, _cycleTo 값으로 클랜 정보들을 가져왔는데 정보가 하나도 없다면 Updator 완료 작업(SetCompleteBatch)을 수행한다. 
// 첫 번째 _cycleFrom, _cycleTo 사이의 클랜 작업이 끝나면 _cycleFrom, _cycleTo 각 값을 _cycleCapacity 만큼 더해 준다. 
// _cycleTo < minMaxClanId.maxClanId + _cycleCapacity  조건이 참이면 클랜작업을 반복한다. 
//********************************************************************************************************

// Updator 완료 작업(SetCompleteBatch)을 수행한다.

namespace ClanRankUpdator
{
    public class ClanRankUpdate
    {
        private DBContext _accountDb;
        private IDatabase _redis;
        private uint _cycleCapacity = 300;
        private uint _cycleFrom = 1;
        private uint _cycleTo = 0;
        private StringBuilder _query = new StringBuilder();
        private uint _serverCurrentSeason;
        private uint _startClanId;

        public ClanRankUpdate(DBContext accountDb, IDatabase redis)
        {
            _accountDb = accountDb;
            _redis = redis;
            _query.Clear();
        }

        public async Task Run()
        {
            if (await IsAlreadyDoneThisSeasonJobAndStartClanIdId())
                return;

            var minMaxClanId = await GetMinMaxClanId();
            InitCycleValue(minMaxClanId.minClanId);
            await RepeateClansJob(minMaxClanId.maxClanId);
            await SetCompleteBatch();
            
        }

        private async Task<bool> IsAlreadyDoneThisSeasonJobAndStartClanIdId()
        {
            ServerSeason serverSeason = new ServerSeason(_accountDb);
            var serverSeasonInfo = await serverSeason.GetKnightLeagueSeasonInfo();
            _serverCurrentSeason = serverSeasonInfo.CurrentSeason;

            var values = await serverSeason.GetCommonValueAll(ServerCommonTable.ClanRankBoardServerSeason);
            _startClanId = values.CommonValue2 + 1;

            if (values.CommonValue == _serverCurrentSeason)
                return true;
            return false;
        }

        private async Task<(uint minClanId, uint maxClanId)> GetMinMaxClanId()
        {
            _query.Clear();
            _query.Append("SELECT MIN(clanId) AS minClanId, MAX(clanId) AS maxClanId FROM clans;");
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return (0, 0);

                if (cursor.IsDBNull(0))
                    return (0, 0);

                return ((uint)cursor["minClanId"], (uint)cursor["maxClanId"]);
            }
        }

        private void InitCycleValue(uint minClanId)
        {
            if (_startClanId > 0)
                _cycleFrom = _startClanId;
            else
                _cycleFrom = minClanId;

            _cycleTo = _cycleCapacity;
        }

        private async Task RepeateClansJob(uint maxClanId)
        {
            do
            {
                var clanIds = await GetClanIds(_cycleFrom, _cycleTo);

                if (clanIds.Count == 0)
                {
                    await SetCompleteBatch();
                    return;
                }

                await ClansJob(clanIds);

                _cycleFrom += _cycleCapacity;
                _cycleTo += _cycleCapacity;

                System.Threading.Thread.Sleep(500);

            } while (_cycleTo < maxClanId + _cycleCapacity);
        }

        private async Task ClansJob(List<uint> clanIds)
        {
            foreach (var clanId in clanIds)
            {
                await ClanJob(clanId);
            }
        }

        private async Task ClanJob(uint clanId)
        {
            int newCurrentSeasonRankPoint = 0;
            await _accountDb.BeginTransactionCallback(async () =>
            {
                var userIds = await GetUserIds(clanId);
                newCurrentSeasonRankPoint = await GetSumClanMembersCurrentSeasonRankPoint(userIds);
                if (newCurrentSeasonRankPoint != 0)
                    await UpdateClanRankPoint(clanId, newCurrentSeasonRankPoint);

                return true;
            });

            var countryCode = await GetClanCountryCode(clanId);

            _accountDb.Commit();

            if (newCurrentSeasonRankPoint != 0)
            {
                await AkaRedisLogic.GameBattleRankRedisJob.SetRankKnightLeagueClanAsync(_redis, clanId, newCurrentSeasonRankPoint, _serverCurrentSeason);
                await AkaRedisLogic.GameBattleRankRedisJob.SetRankKnightLeagueClanCountryAsync(_redis, clanId, newCurrentSeasonRankPoint, _serverCurrentSeason, countryCode);
            }
                

            await SetClanId(clanId);
        }

        private async Task<List<uint>> GetClanIds(uint fromClanId, uint toClanId)
        {
            _query.Clear();
            _query.Append("SELECT clanId FROM clans WHERE clanId >= ").Append(fromClanId)
                .Append(" AND clanId <=").Append(toClanId).Append(";");

            List<uint> clansIds = new List<uint>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    clansIds.Add((uint)cursor["clanId"]);
                }
            }

            return clansIds;
        }

        private async Task<List<uint>> GetUserIds(uint clanId)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM clan_members WHERE clanId = ").Append(clanId).Append(" LOCK IN SHARE MODE;");

            List<uint> userIds = new List<uint>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    userIds.Add((uint)cursor["userId"]);
                }
            }

            return userIds;
        }

        private async Task<int> GetSumClanMembersCurrentSeasonRankPoint(List<uint> userIds)
        {
            _query.Clear();
            var strUserIds = userIds.Any() ? string.Join(",", userIds) : "0";

            _query.Append("SELECT SUM(newRankPoint) AS newRankPoint " +
                "FROM (SELECT nextSeasonRankPoint AS newRankPoint FROM accounts " +
                "WHERE userId IN(").Append(strUserIds).Append(") AND currentSeason = ").Append(_serverCurrentSeason - 1)
                .Append(" UNION SELECT currentSeasonRankPoint AS newRankPoint FROM accounts " +
                "WHERE userId IN(").Append(strUserIds).Append(") AND currentSeason = ").Append(_serverCurrentSeason).Append(") tTable FOR UPDATE;");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return cursor.GetInt32(0);
                return 0;
            }
        }

        private async Task UpdateClanRankPoint(uint clanId, int newCurrentSeasonRankPoint)
        {
            _query.Clear();
            _query.Append("UPDATE clans SET rankPoint = ").Append(newCurrentSeasonRankPoint)
                .Append(" WHERE clanId = ").Append(clanId).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());

        }

        private async Task<string> GetClanCountryCode(uint clanId)
        {
            _query.Clear();
            _query.Append("SELECT countryCode FROM clans WHERE clanId = ").Append(clanId).Append(";");

            
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    throw new System.Exception();

                return (string)cursor["countryCode"];
            }
        }

        private async Task SetClanId(uint clanId)
        {
            _query.Clear();
            _query.Append("UPDATE _common SET commonValue2 = ").Append(clanId)
                .Append(" WHERE commonId = ").Append((int)ServerCommonTable.ClanRankBoardServerSeason).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task SetCompleteBatch()
        {
            _query.Clear();
            _query.Append("UPDATE _common SET commonValue2 = 0, commonValue = ").Append(_serverCurrentSeason)
                .Append(" WHERE commonId = ").Append((int)ServerCommonTable.ClanRankBoardServerSeason).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }
}
