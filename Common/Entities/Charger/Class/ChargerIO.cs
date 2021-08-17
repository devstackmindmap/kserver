using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Season;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Charger
{
    public abstract  class ChargerIO : ICharger
    {
        private uint _userId;
        private DBContext _accountDb;
        private DBContext _userDb;

        private ChargerData _chargerData;
        private ChargerIOData _chargerIOData;

        private ServerSeasonInfo _seasonInfo;
        private List<uint> _purchasedSeasons;

        public ChargerIO(uint userId, ChargerIOData chargerIOData, DBContext userDb, DBContext accountDb,
            ServerSeasonInfo seasonInfo, List<uint> purchasedSeasons)
        {
            _userId = userId;
            _chargerIOData = chargerIOData;
            _userDb = userDb;
            _accountDb = accountDb;
            _seasonInfo = seasonInfo;
            _purchasedSeasons = purchasedSeasons;
        }

        public async Task Update()
        {
            using (var cursor = await Select())
            {
                if (false == cursor.Read())
                    throw new Exception();

                if (_purchasedSeasons.Contains(_seasonInfo.CurrentSeason))
                    _chargerData = GetCharegerDataForPurchasedSeasonPass(cursor);
                else
                    _chargerData = GetCharegerDataForPuchasedBeforeSeasonPassOrNormal(cursor);
            }

            var charger = new Charger(_chargerData);

            if (charger.Update())
                await UpdateChargerData();
        }

        private ChargerData GetCharegerDataForPuchasedBeforeSeasonPassOrNormal(DbDataReader cursor)
        {
            return  new ChargerData
            {
                MaxCharger = (int)AkaData.Data.GetConstant(DataConstantType.MAX_ENERGY).Value,
                ChargerQuantityEachGet = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_QUANTITY_EACH_GET).Value,
                ChargerQuantityEachGetPremiumForBeforeSeason = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_QUANTITY_EACH_GET_PREMIUM).Value,
                ChargerAcquisitionUnitMinute = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_ACQUISITION_UNIT_MINUTE).Value,
                CurrentChargerQuantity = (int)cursor[_chargerIOData.ColumnName],
                RecentUpdateDateTimeOfCharger
                    = cursor[_chargerIOData.RecentUpdateDateTimeColumnName] != null ? (DateTime)cursor[_chargerIOData.RecentUpdateDateTimeColumnName] : DateTime.UtcNow,
                CurrentSeasonPassStartDateTime = _seasonInfo.CurrentSeasonStartDateTime,
                IsPurchasedBeforeSeasonPass = _purchasedSeasons.Contains(_seasonInfo.CurrentSeason - 1)
            };
        }

        // 이 함수가 호출될때는 시즌패스 구매 전 Charget.Update가 반드시 수행되어야 한다. 
        private ChargerData GetCharegerDataForPurchasedSeasonPass(DbDataReader cursor)
        {
            return new ChargerData
            {
                MaxCharger = (int)AkaData.Data.GetConstant(DataConstantType.MAX_ENERGY_PREMIUM).Value,
                ChargerQuantityEachGet = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_QUANTITY_EACH_GET_PREMIUM).Value,
                ChargerQuantityEachGetPremiumForBeforeSeason = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_QUANTITY_EACH_GET_PREMIUM).Value,
                ChargerAcquisitionUnitMinute = (int)AkaData.Data.GetConstant(DataConstantType.ENERGY_ACQUISITION_UNIT_MINUTE).Value,
                CurrentChargerQuantity = (int)cursor[_chargerIOData.ColumnName],
                RecentUpdateDateTimeOfCharger
                    = cursor[_chargerIOData.RecentUpdateDateTimeColumnName] != null ? (DateTime)cursor[_chargerIOData.RecentUpdateDateTimeColumnName] : DateTime.UtcNow,
                CurrentSeasonPassStartDateTime = _seasonInfo.CurrentSeasonStartDateTime,
                IsPurchasedBeforeSeasonPass = false,
                IsPurchasedCurrentSeasonPass = true
            };
        }

        private async Task<DbDataReader> Select()
        {
            var strUserId = _userId.ToString();
            var strTypeValue = _chargerIOData.TypeValue.ToString();
            var cursor 
                = await _userDb.ExecuteReaderAsync(
                    $"SELECT {_chargerIOData.ColumnName}, {_chargerIOData.RecentUpdateDateTimeColumnName} " +
                    $"FROM {_chargerIOData.TableName} WHERE userId = {strUserId} AND type={strTypeValue};");

            return cursor;
        }

        private async Task UpdateChargerData()
        {
            await _userDb.ExecuteNonQueryAsync("UPDATE {0} SET {1} = {2}, {3} = '{4}' WHERE userId = {5} AND type = {6};",
                _chargerIOData.TableName,
                _chargerIOData.ColumnName,
                _chargerData.CurrentChargerQuantity,
                _chargerIOData.RecentUpdateDateTimeColumnName,
                _chargerData.RecentUpdateDateTimeOfCharger.ToTimeString(),
                _userId,
                _chargerIOData.TypeValue
                );
        }

        public async Task UpdateChargerDataNowDateTime()
        {
            await _userDb.ExecuteNonQueryAsync("UPDATE {0} SET {1} = '{2}' WHERE userId = {3} AND type = {4};",
                _chargerIOData.TableName,
                _chargerIOData.RecentUpdateDateTimeColumnName,
                DateTime.UtcNow.ToTimeString(),
                _userId,
                _chargerIOData.TypeValue
                );
        }

        public async Task UpdateChargerDataNowDateTime(DateTime newDateTime)
        {
            await _userDb.ExecuteNonQueryAsync("UPDATE {0} SET {1} = '{2}' WHERE userId = {3} AND type = {4};",
                _chargerIOData.TableName,
                _chargerIOData.RecentUpdateDateTimeColumnName,
                newDateTime.ToTimeString(),
                _userId,
                _chargerIOData.TypeValue
                );
        }
    }
}
