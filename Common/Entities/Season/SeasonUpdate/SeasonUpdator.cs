using AkaDB.MySql;
using System;
using System.Threading.Tasks;

namespace Common.Entities.Season
{
    public class SeasonUpdator
    {
        DBContext _accountDb;
        DBContext _userDb;
        uint _userId;
        ServerSeasonInfo _serverSeasonInfo;

        public SeasonUpdator(uint userId)
        {
            _userId = userId;
        }

        public SeasonUpdator(DBContext accountDb, DBContext userDb, uint userId)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
        }

        public async Task<uint> SeasonUpdateWithTransaction()
        {
            uint serverCurrentSeason = 0;
            using (_accountDb = new DBContext("AccountDBSetting"))
            {
                using (_userDb = new DBContext(_userId))
                {
                    await _accountDb.BeginTransactionCallback(async () =>
                    {
                        await _userDb.BeginTransactionCallback(async () =>
                        {
                            await SeasonUpdate();
                            serverCurrentSeason = await GetCurrentSeason();
                            return true;
                        });
                        return true;
                    });

                    _accountDb.Commit();
                    _userDb.Commit();
                }
            }

            return serverCurrentSeason;
        }

        public async Task<uint> SeasonUpdateByUserSeasonUpdator()
        {
            await SeasonUpdate(true);
            return await GetCurrentSeason();
        }

        public async Task SeasonUpdate(bool forBatch = false)
        {
            var redis = AkaRedis.AkaRedis.GetDatabase();
            _serverSeasonInfo = await (new ServerSeason(_accountDb)).GetKnightLeagueSeasonInfo();
            SeasonUpdatorAccount accountSeasonUpdator = new SeasonUpdatorAccount(_accountDb, redis, _userId, _serverSeasonInfo.CurrentSeason);
            SeasonUpdatorUnit unitSeasonUpdator 
                = new SeasonUpdatorUnit(_userDb, redis, _userId, _serverSeasonInfo.CurrentSeason, accountSeasonUpdator.GetCountryCode() );

            if (accountSeasonUpdator.IsCurrentSeason())
            {
                return;
            }
            else if (accountSeasonUpdator.IsSeasonChange())
            {
                var newRankPoints = await unitSeasonUpdator.SeasonUpdate();
                await accountSeasonUpdator.SeasonUpdate(newRankPoints);
            }
            else
            {
                AkaLogger.Log.Debug.Info($"User:{_userId} Season:{_serverSeasonInfo.CurrentSeason}  UserSeason{accountSeasonUpdator.CurrentSeason}\nCallStack:{Environment.StackTrace}", "SeasonUpdate");
                throw new Exception("Check the season system");
            }
        }

        public async Task<uint> GetCurrentSeason()
        {
            if (_serverSeasonInfo == null)
                await SeasonUpdate();

            return _serverSeasonInfo.CurrentSeason;
        }
    }
}
