using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Charger;
using Common.Entities.Season;
using System.Collections.Generic;

namespace Common.Entities.InfusionBox
{
    public class LeagueInfusionBoxOpen : InfusionBoxOpen
    {
        public LeagueInfusionBoxOpen(uint userId, DBContext db, DBContext accountDb, 
            ServerSeasonInfo seasonInfo, List<uint> purchasedSeasons)
            : base(userId, (byte)InfusionBoxType.LeagueBox,
                  ChargerFactory.CreateCharger(userId, InfusionBoxType.LeagueBox, db, accountDb, seasonInfo, purchasedSeasons)
                  , db, accountDb)
        {
        }
    }
}
