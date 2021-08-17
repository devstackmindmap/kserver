using System;

namespace Common.Entities.Charger
{
    public class Charger
    {
        private ChargerData _chargerData;

        public Charger(ChargerData chargerData)
        {
            _chargerData = chargerData;
        }

        public bool Update()
        {
            if (IsNeedPremiumBoostCalculate())
            {
                var isChange = BeforeSeasonBoostUpdate();
                return NormalUpdate() ||isChange;
            }
            else
            {
                return NormalUpdate();
            } 
        }

        private bool IsNeedPremiumBoostCalculate()
        {
            return _chargerData.IsPurchasedBeforeSeasonPass &&
                _chargerData.RecentUpdateDateTimeOfCharger.AddMinutes(_chargerData.ChargerAcquisitionUnitMinute) 
                < _chargerData.CurrentSeasonPassStartDateTime;
        }

        public bool NormalUpdate()
        {
            if (IsAlreadyMaxEnergy())
            {
                return false;
            }
            else if (IsExceededMaxEnergy())
            {
                _chargerData.CurrentChargerQuantity = _chargerData.MaxCharger;
                return true;
            }

            var flowTime = GetFlowTime(DateTime.UtcNow);
            var multicount = GetMultiCountToUseObtainTheAcquiredEnergy(flowTime);

            if (multicount == 0)
                return false;

            SetNewChargerData(multicount, _chargerData.ChargerQuantityEachGet, _chargerData.MaxCharger);
            return true;
        }

        public bool BeforeSeasonBoostUpdate()
        {
            if (IsAlreadyMaxEnergy())
            {
                return false;
            }
            else if (IsExceededMaxEnergy())
            {
                _chargerData.CurrentChargerQuantity = _chargerData.MaxCharger;
                return true;
            }

            var flowTime = GetFlowTime(_chargerData.CurrentSeasonPassStartDateTime);
            var multicount = GetMultiCountToUseObtainTheAcquiredEnergy(flowTime);

            if (multicount == 0)
                return false;

            SetNewChargerData(multicount, _chargerData.ChargerQuantityEachGetPremiumForBeforeSeason, _chargerData.MaxCharger);
            return true;
        }

        private bool IsAlreadyMaxEnergy()
        {
            return _chargerData.CurrentChargerQuantity == _chargerData.MaxCharger;
        }
        private bool IsExceededMaxEnergy()
        {
            return _chargerData.CurrentChargerQuantity > _chargerData.MaxCharger;
        }

        private TimeSpan GetFlowTime(DateTime baseDateTime)
        {
            return baseDateTime - _chargerData.RecentUpdateDateTimeOfCharger;
        }

        private int GetMultiCountToUseObtainTheAcquiredEnergy(TimeSpan flowTime)
        {
            return (int)flowTime.TotalMinutes / _chargerData.ChargerAcquisitionUnitMinute;
        }

        private void SetNewChargerData(int multicount, int chargerQuantityEachGet, int maxCharger)
        {
            var quantityAvailable = GetQuantityAvailable(multicount, chargerQuantityEachGet);
            var newChargerQuantity = quantityAvailable + _chargerData.CurrentChargerQuantity;

            if (newChargerQuantity > maxCharger)
                UpdateNewChargerData(maxCharger);
            else
                UpdateNewChargerData(newChargerQuantity, quantityAvailable, chargerQuantityEachGet);
        }

        private int GetQuantityAvailable(int multiCount, int chargerQuantityEachGet)
        {
            return chargerQuantityEachGet * multiCount;
        }

        private void UpdateNewChargerData(int maxCharger)
        {
            _chargerData.CurrentChargerQuantity = maxCharger;
        }

        private void UpdateNewChargerData(int newChargerQuantity, int quantityAvailable, int chargerQuantityEachGet)
        {
            var multiCountToBeUsedInCalculatingNewestUpdateTime 
                = GetMultiCountToBeUsedInCalculatingNewestUpdatDateTime(quantityAvailable, chargerQuantityEachGet);
            var timeToAddToNewUpdateTime = multiCountToBeUsedInCalculatingNewestUpdateTime * _chargerData.ChargerAcquisitionUnitMinute;
            var newChargerRecentUpdateDateTime = _chargerData.RecentUpdateDateTimeOfCharger.AddMinutes(timeToAddToNewUpdateTime);

            _chargerData.CurrentChargerQuantity = newChargerQuantity;
            _chargerData.RecentUpdateDateTimeOfCharger = newChargerRecentUpdateDateTime;
        }

        private int GetMultiCountToBeUsedInCalculatingNewestUpdatDateTime(int quantityAvailable, int chargerQuantityEachGet)
        {
            return quantityAvailable / chargerQuantityEachGet;
        }
    }
}
