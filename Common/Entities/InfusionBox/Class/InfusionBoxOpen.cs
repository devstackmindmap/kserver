using AkaDB.MySql;
using Common.Entities.Challenge;
using Common.Entities.Charger;
using Common.Entities.Reward;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.InfusionBox
{
    public abstract  class InfusionBoxOpen : InfusionBox, IInfusionBoxOpen
    {
        private DBContext _accountDb;
        private ProtoInfusionBox _infusionBox;
        private Box.Box _box;
        private Correction.UserCorrection _correction;

        public InfusionBoxOpen(uint userId, byte type, ICharger energy, DBContext db, DBContext accountDb)
            : base(userId, type, energy, db)
        {
            _accountDb = accountDb;
        }

        public async Task<bool> IsEnoughEnergy()
        {
            _infusionBox = await GetInfusionBox();

            if (_infusionBox.BoxEnergy < AkaData.Data.GetReward(_infusionBox.Id).NeedEnergy)
                return false;

            return true;
        }

        public async Task<InfusionBoxOpenInfo> GetInfusionBoxOpenInfo()
        {
            if (_infusionBox == null)
                throw new System.Exception("You Need Call IsEnoughEnergy First");

            _box = Reward.Reward.GetBox(_userId, _infusionBox.Id, _db);
            var itemResults = await _box.GetReward();

            _correction = new Correction.UserCorrection(_userId.ToString(), _type.ToString(), _db);
            var nowCorrections = await _correction.GetCorrections();

            var probabilityGroup = new ProbabilityGroup(_userId, ConstValue.NORMAL_LEAGUE_PROBABILITY_GROUP_ID, _db);
            var newElementId = probabilityGroup.GetBoxWithCorrection(nowCorrections);

            var newCorrections = GetNewCorrections(newElementId, nowCorrections);

            return new InfusionBoxOpenInfo
            {
                NewInfusionBox = new ProtoNewInfusionBox
                {
                    Id = newElementId,
                    NewTotalBoxEnergy = _infusionBox.BoxEnergy - AkaData.Data.GetReward(_infusionBox.Id).NeedEnergy,
                    NewTotalUserEnergy = _infusionBox.UserEnergy,
                    NewTotalUserBonusEnergy = _infusionBox.UserBonusEnergy,
                    NewUserEnergyRecentUpdateDatetime = _infusionBox.UserEnergyRecentUpdateDatetime,
                    UseUserEnergy = 0,
                    UseUserBonusEnergy = 0
                },
                ItemResults = itemResults,
                Corrections = newCorrections
            };
        }

        private IDictionary<uint, int> GetNewCorrections(uint nowElementId, IDictionary<uint, int> nowCorrections)
        {
            var newCorrection = new Dictionary<uint, int>();

            if (nowCorrections.Count == 0)
                return GetUserCorrectionInit();
            else
                return GetUserCorrectionAttach(nowElementId, nowCorrections);
        }

        private Dictionary<uint, int> GetUserCorrectionInit()
        {
            var newCorrection = new Dictionary<uint, int>();
            var elements = AkaData.Data.GetProbabilityGroup(ConstValue.NORMAL_LEAGUE_PROBABILITY_GROUP_ID);
            foreach (var element in elements)
            {
                newCorrection.Add(element.ElementId, element.Correction);
            }
            return newCorrection;
        }

        private Dictionary<uint, int> GetUserCorrectionAttach(uint nowElementId, IDictionary<uint, int> nowCorrections)
        {
            var newCorrection = new Dictionary<uint, int>();
            foreach (var correction in nowCorrections)
            {
                if (!newCorrection.ContainsKey(correction.Key))
                    newCorrection.Add(correction.Key, correction.Value);


                if (correction.Key == nowElementId)
                {
                    newCorrection[correction.Key] = 0;
                }
                else
                {
                    var elementData
                        = AkaData.Data.GetProbabilityGroup(ConstValue.NORMAL_LEAGUE_PROBABILITY_GROUP_ID, correction.Key);
                    newCorrection[correction.Key] = newCorrection[correction.Key] + elementData.Correction;
                }
            }
            return newCorrection;
        }

        public async Task SetInfusionBoxOpenInfo(InfusionBoxOpenInfo infusionBoxOpenInfo)
        {
            if (_box == null)
                throw new System.Exception("You Need Call GetInfusionBoxOpenInfo First");

            if (_correction == null)
                throw new System.Exception("You Need Call GetInfusionBoxOpenInfo First");

            await _box.SetReward(infusionBoxOpenInfo.ItemResults, "InfusionBoxOpen");
            await _correction.SetCorrections(infusionBoxOpenInfo.Corrections);
            await SetInfusionBox(infusionBoxOpenInfo.NewInfusionBox);
        }
    }
}
