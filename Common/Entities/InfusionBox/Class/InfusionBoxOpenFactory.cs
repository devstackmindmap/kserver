using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using Common.Pass;
using System;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public static class InfusionBoxOpenFactory
    {
        public static async Task<IInfusionBoxOpen> CreateInfusionBoxOpen(uint userId, InfusionBoxType type, DBContext db, DBContext accountDb)
        {
            var serverSeason = new ServerSeason(accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            var purchasedSeasons
                = await(new SeasonPassManager(userId, seasonInfo.CurrentSeason, db))
                .GetBeforeAndCurrentPurchasedSeasonPassList();

            switch (type)
            {
                case InfusionBoxType.LeagueBox:
                    return new LeagueInfusionBoxOpen(userId, db, accountDb, seasonInfo, purchasedSeasons);
                default:
                    throw new Exception("CreateInfusionBox, Non existent type");
            }
        }
    }
}
