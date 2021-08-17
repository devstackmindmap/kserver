using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserSeasonUpdator
{
    public class UserSeasonUpdate
    {
        private DBContext _accountDb;
        private uint _cycleCapacity = 300;
        private uint _cycleFrom = 1;
        private uint _cycleTo = 0;
        private StringBuilder _query = new StringBuilder();
        private uint _serverCurrentSeason;
        private uint _startUserId;

        public async Task Run()
        {
            using (_accountDb = new DBContext("AccountDBSetting"))
            {
                if (await IsAlreadyDoneThisSeasonJobAndStartUserId())
                    return;

                var minMaxUserId = await GetMinMaxUserId();
                InitCycleValue(minMaxUserId.minUserId);
                await RepeateUsersJob(minMaxUserId.maxUserId);
                await SetCompleteBatch();
            }
        }

        private async Task<bool> IsAlreadyDoneThisSeasonJobAndStartUserId()
        {
            ServerSeason serverSeason = new ServerSeason(_accountDb);
            var serverSeasonInfo = await serverSeason.GetKnightLeagueSeasonInfo();
            _serverCurrentSeason = serverSeasonInfo.CurrentSeason;

            var values = await serverSeason.GetCommonValueAll(ServerCommonTable.ForNotUpdatedSeasonJobUser);
            _startUserId = values.CommonValue2 + 1;
            if (values.CommonValue == _serverCurrentSeason)
                return true;
            return false;
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
                    await SetCompleteBatch();
                    return;
                }

                await UsersJob(userInfos);

                _cycleFrom += _cycleCapacity;
                _cycleTo += _cycleCapacity;


            } while (_cycleTo < maxClanId + _cycleCapacity);
        }

        private async Task<List<UserInfo>> GetInfos(uint fromUserId, uint toUserId)
        {
            _query.Clear();
            _query.Append("SELECT userId, currentSeason FROM accounts WHERE userId >= ").Append(fromUserId)
                .Append(" AND userId <=").Append(toUserId).Append(";");

            List<UserInfo> userInfos = new List<UserInfo>();
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    userInfos.Add(new UserInfo
                    {
                        UserId = (uint)cursor["userId"],
                        CurrentSeason = (int)cursor["currentSeason"]
                    });
                }
            }

            return userInfos;
        }

        private async Task UsersJob(List<UserInfo> userInfos)
        {
            foreach (var userInfo in userInfos)
            {
                if (userInfo.CurrentSeason != _serverCurrentSeason)
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
                    {
                        var userSeasonUpdator = new SeasonUpdator(_accountDb, userDb, userId);
                        await userSeasonUpdator.SeasonUpdateByUserSeasonUpdator();
                        await SetUserId(userId);
                        return true;
                    });
                    return true;
                    
                });
                _accountDb.Commit();
                userDb.Commit();
            }
        }
    

        private async Task SetCompleteBatch()
        {
            _query.Clear();
            _query.Append("UPDATE _common SET commonValue2 = 0, commonValue = ").Append(_serverCurrentSeason)
                .Append(" WHERE commonId = ").Append((int)ServerCommonTable.ForNotUpdatedSeasonJobUser).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task SetUserId(uint userId)
        {
            _query.Clear();
            _query.Append("UPDATE _common SET commonValue2 = ").Append(userId)
                .Append(" WHERE commonId = ").Append((int)ServerCommonTable.ForNotUpdatedSeasonJobUser).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }
    }

    public class UserInfo
    {
        public uint UserId;
        public int CurrentSeason;
    }
}
