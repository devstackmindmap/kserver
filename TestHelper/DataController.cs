using AkaData;
using AkaEnum;
using AkaUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestHelper
{
    public class DataController
    {
        public static uint MakeReward(ItemType itemType, uint classId, int count, int probability)
        {
            var id = MakeTempId();
            GetCreatedItems(id, itemType, classId, count, probability);
            var rewards = DataGetter.StoreDataMap[DataType.data_reward] as IDictionary<uint, DataReward>;
            var itemIds = new List<uint>();
            itemIds.Add(id);
            rewards.Add(id, new DataReward
            {
                ItemIdList = itemIds,
                NeedEnergy = 0,
                RewardId = id
            });
            return id;
        }

        private static uint MakeTempId()
        {
            var data = DataGetter.StoreDataMap[DataType.data_item] as IDictionary<uint, List<DataItem>>;

            Random random = new Random();
            return (uint)random.Next(10000000, 100000000);
        }

        private static void GetCreatedItems(uint id, ItemType itemType, uint classId, int count, int probability)
        {
            var dataMapItems = DataGetter.StoreDataMap[DataType.data_item] as IDictionary<uint, List<DataItem>>;

            var items = new List<DataItem>();
            items.Add(new DataItem
            {
                ItemId = id,
                ItemType = itemType,
                ClassId = classId,
                MinNumber = count,
                MaxNumber = count,
                Probability = probability
            });
            dataMapItems.Add(id, items);
        }

        public static void SetEnergyBoxDataConstant(
            int startEnergy, int startBonusEnergy,
            float energyQuantityEachGet, float energyAcquisitionUnitMinute, 
            float winInfusionEnergy, float loseInfusionEnergy, 
            float maxInfusionEnergy, float maxEnergy, int firstBoxNeedEnergy)
        {
            Data.GetMaterial(MaterialType.Energy).StartValue = startEnergy;
            Data.GetConstant(DataConstantType.ENERGY_QUANTITY_EACH_GET).Value = energyQuantityEachGet;
            Data.GetConstant(DataConstantType.ENERGY_ACQUISITION_UNIT_MINUTE).Value = energyAcquisitionUnitMinute;
            Data.GetConstant(DataConstantType.WIN_INFUSION_ENERGY).Value = winInfusionEnergy;
            Data.GetConstant(DataConstantType.LOSE_INFUSION_ENERGY).Value = loseInfusionEnergy;
            Data.GetConstant(DataConstantType.MAX_ENERGY).Value = maxEnergy;
            Data.GetMaterial(MaterialType.BonusEnergy).StartValue = startBonusEnergy;
            var rewardId = (uint)Data.GetConstant(DataConstantType.FIRST_LEAGUE_BOX).Value;
            Data.GetReward(rewardId).NeedEnergy = firstBoxNeedEnergy;
        }
    }
}
