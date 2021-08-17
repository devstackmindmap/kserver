using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Challenge;
using Common.Entities.Charger;
using Common.Entities.Item;
using Common.Entities.Season;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public abstract  class InfusionBoxInfusion : InfusionBox, IInfusionBoxInfusion
    {
        private DBContext _accountDb;
        private ServerSeasonInfo _seasonInfo;
        private List<uint> _purchasedSeasons;

        public InfusionBoxInfusion(uint userId, byte type, ICharger energy, DBContext accountDb, DBContext db, 
            ServerSeasonInfo seasonInfo, List<uint> purchasedSeasons)
            : base(userId, type, energy, db)
        {
            _accountDb = accountDb;
            _seasonInfo = seasonInfo;
            _purchasedSeasons = purchasedSeasons;
        }

        public async Task<ProtoNewInfusionBox> GetNewInfusionBox(bool isWin, int addtionalInfusionEnergy, bool isDoubleEnergy)
        {
            await _energy.Update();
            var infusionBox = await GetInfusionBox();

            //if (!IsCanInfusion(infusionBox))
            if (infusionBox.UserEnergy + addtionalInfusionEnergy == 0)
            {
                return new ProtoNewInfusionBox
                {
                    Id = infusionBox.Id,
                    NewTotalBoxEnergy = infusionBox.BoxEnergy,
                    NewTotalUserBonusEnergy = infusionBox.UserBonusEnergy,
                    NewTotalUserEnergy = infusionBox.UserEnergy,
                    NewUserEnergyRecentUpdateDatetime = infusionBox.UserEnergyRecentUpdateDatetime,
                    UseUserEnergy = 0,
                    UseUserBonusEnergy = 0
                };
            }

            var (newInfusionBox, totalInfusionEnergy) = await GetNewInfusionBox(isWin, infusionBox, addtionalInfusionEnergy, isDoubleEnergy);
            await AddEventBoxEnergy(totalInfusionEnergy);
            return newInfusionBox;
        }

        private async Task AddEventBoxEnergy(int addInfusionEnergy)
        {
            if (await IsInChallengeEvent())
            {
                var coin = MaterialFactory.CreateTermMaterial(MaterialType.EventBoxEnergy, _userId, addInfusionEnergy, _accountDb, _db);
                await coin.Get("EnergyInfusion");
            }
        }

        public async Task SetNewInfusionBox(ProtoNewInfusionBox newInfusionBox)
        {
            await SetInfusionBox(newInfusionBox);
        }

        private async Task<(ProtoNewInfusionBox newInfusionBox, int totalInfusionEnergy)> 
            GetNewInfusionBox(bool isWin, ProtoInfusionBox infusionBox, int addtionalInfusionEnergy, bool isDoubleEnergy)
        {
            var useEnergy      = GetUseEnergy(isWin, infusionBox.UserEnergy);
            var addtionalEnergies = (useEnergy + addtionalInfusionEnergy) * (isDoubleEnergy ? 2 : 1);
            var useBonusEnergy = GetUseBonusEnergy(addtionalEnergies, infusionBox);
            DateTime newUserEnergyRecentUpdateDatetime = await GetNewUserEnergyRecentUpdateDatetime(infusionBox);
            var totalInfusionEnergy = addtionalEnergies + useBonusEnergy;//useEnergy + addtionalInfusionEnergy + useBonusEnergy;
            var newTotalBoxEnergy = infusionBox.BoxEnergy + totalInfusionEnergy;

            return (new ProtoNewInfusionBox
            {
                Id = infusionBox.Id,
                NewTotalBoxEnergy = (int)newTotalBoxEnergy,
                UseUserEnergy = useEnergy,
                UseUserBonusEnergy = useBonusEnergy,
                NewTotalUserEnergy = infusionBox.UserEnergy - useEnergy,
                NewTotalUserBonusEnergy = infusionBox.UserBonusEnergy - useBonusEnergy,
                NewUserEnergyRecentUpdateDatetime = newUserEnergyRecentUpdateDatetime.Ticks
            }, totalInfusionEnergy);
        }

        private int GetUseEnergy(bool isWin, int energy)
        {
            var useUserEnergy = isWin
               ? (int)AkaData.Data.GetConstant(DataConstantType.WIN_INFUSION_ENERGY).Value
               : (int)AkaData.Data.GetConstant(DataConstantType.LOSE_INFUSION_ENERGY).Value;

            return Math.Min(useUserEnergy, energy);
        }

        private int GetUseBonusEnergy(int energy, ProtoInfusionBox infusionBox)
        {
            return Math.Min(energy, infusionBox.UserBonusEnergy);
        }

        private async Task<DateTime> GetNewUserEnergyRecentUpdateDatetime(ProtoInfusionBox infusionBox)
        {
            DateTime newUserEnergyRecentUpdateDatetime;
            if (infusionBox.UserEnergy == GetMaxEnergy())
            {
                newUserEnergyRecentUpdateDatetime = DateTime.UtcNow;
                await _energy.UpdateChargerDataNowDateTime(newUserEnergyRecentUpdateDatetime);
            }
            else
            {
                newUserEnergyRecentUpdateDatetime = new DateTime(infusionBox.UserEnergyRecentUpdateDatetime);
            }

            return newUserEnergyRecentUpdateDatetime;
        }

        private int GetMaxEnergy()
        {
            if (_purchasedSeasons.Contains(_seasonInfo.CurrentSeason))
                return (int)AkaData.Data.GetConstant(DataConstantType.MAX_ENERGY_PREMIUM).Value;
            else
                return (int)AkaData.Data.GetConstant(DataConstantType.MAX_ENERGY).Value;
        }

        private async Task<bool> IsInChallengeEvent()
        {
            var challenge = ChallengeFactory.CreateEventChallengeManager(_accountDb, null, _userId, 0, 0);
            return await challenge.IsInEvent();
        }
    }
}
