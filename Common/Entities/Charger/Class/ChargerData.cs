using System;

namespace Common.Entities.Charger
{
    public class ChargerData
    {
        public int MaxCharger;
        public int ChargerAcquisitionUnitMinute;
        public int ChargerQuantityEachGet;
        public int ChargerQuantityEachGetPremiumForBeforeSeason;
        public int CurrentChargerQuantity;
        public DateTime RecentUpdateDateTimeOfCharger;
        public DateTime CurrentSeasonPassStartDateTime;
        public bool IsPurchasedBeforeSeasonPass;
        public bool IsPurchasedCurrentSeasonPass;
    }
}
