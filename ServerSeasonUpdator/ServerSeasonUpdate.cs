using AkaDB.MySql;
using System;
using System.Text;
using System.Threading.Tasks;
using AkaUtility;
using AkaEnum;

namespace ServerSeasonUpdator
{
    public class ServerSeasonUpdate
    {
        private DBContext _accountDb;

        public ServerSeasonUpdate(DBContext accountDb)
        {
            _accountDb = accountDb;
        }

        public async Task Run()
        {
            var seasonIntervalUpdate = new ServerSeasonIntervalUpdate(_accountDb);
            await seasonIntervalUpdate.Run(ServerCommonTable.KnightLeagueSeason, DataConstantType.LEAGUE_SEASON_REWARD_INTERVAL_MINUTE);

            var seasonIntervalUpdate2 = new ServerSeasonIntervalUpdate(_accountDb);
            await seasonIntervalUpdate2.Run(ServerCommonTable.ChallengeSeason, DataConstantType.CHALLENGE_SEASON_REWARD_INTERVAL_MINUTE);

            var seasonMonthlyUpdate = new ServerSeasonMonthlyUpdate(_accountDb);
            await seasonMonthlyUpdate.Run();
        }
    }
}
