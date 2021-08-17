using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Season;
using System.Collections.Generic;

namespace Common.Entities.Charger
{
    public class LeagueBoxEnergy : ChargerIO
    {
        public LeagueBoxEnergy(uint userId, DBContext userDb, DBContext accountDb, 
            ServerSeasonInfo seasonInfo, List<uint> purchasedSeasons) 
            : base(userId, new ChargerIOData
        {
            TableName = TableName.INFUSION_BOX,
            ColumnName = ColumnName.USER_ENERGY,
            BonusColumnName = ColumnName.USER_BONUS_ENERGY,
            RecentUpdateDateTimeColumnName = ColumnName.USER_ENERGY_RECENT_UPDATE_DATETIME,
            TypeValue = (byte)InfusionBoxType.LeagueBox
        }, userDb, accountDb, seasonInfo, purchasedSeasons)
        {

        }
    }
}
