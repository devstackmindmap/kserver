using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using System;
using System.Collections.Generic;

namespace Common.Entities.Charger
{
    public static class ChargerFactory
    {
        public static ICharger CreateCharger(uint userId, InfusionBoxType type, DBContext userDb, DBContext accountDb,
            ServerSeasonInfo seasonInfo, List<uint> purchasedSeasons)
        {
            switch (type)
            {
                case InfusionBoxType.LeagueBox:
                    return new LeagueBoxEnergy(userId, userDb, accountDb, seasonInfo, purchasedSeasons);
                default:
                    throw new Exception("CreateInfusionBox, Non existent type");
            }
        }
    }
}
