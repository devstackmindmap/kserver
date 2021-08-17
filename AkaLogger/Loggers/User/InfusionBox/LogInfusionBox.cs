namespace AkaLogger.Users
{
    public sealed class LogInfusionBox
    {
        public void Log(uint userId, byte infusionBoxOpenType, string itemResultList, uint infusionBoxId, 
            int newTotalBoxEnergy, int newTotalUserBonusEnergy, int newTotalUserEnergy)
        {
            Logger.Instance().Analytics("InfusionBoxOpen", "InfusionBox",
                "UserId", userId.ToString(),
                "InfusionBoxOpenType", infusionBoxOpenType.ToString(),
                "ItemResultList", itemResultList,
                "BoxId", infusionBoxId.ToString(),
                "NewTotalBoxEnergy", newTotalBoxEnergy.ToString(),
                "NewTotalUserBonusEnergy", newTotalUserBonusEnergy.ToString(),
                "NewTotalUserEnergy", newTotalUserEnergy.ToString());
        }

        public void Log(uint userId, uint infusionBoxId, int useUserEnergy, int useUserBonusEnergy, int newTotalBoxEnergy, 
            int newTotalUserBonusEnergy, int newTotalUserEnergy, int addtionalEnergy, bool isEnergyBonusEvent)
        {
            Logger.Instance().Analytics("InfusionBoxBattleResult", "InfusionBox",
                "UserId", userId.ToString(),
                "BoxId", infusionBoxId.ToString(),
                "UseUserEnergy", useUserEnergy.ToString(),
                "UseUserBonusEnergy", useUserBonusEnergy.ToString(),
                "NewTotalBoxEnergy", newTotalBoxEnergy.ToString(),
                "NewTotalUserBonusEnergy", newTotalUserBonusEnergy.ToString(),
                "NewTotalUserEnergy", newTotalUserEnergy.ToString(),
                "AddtionalEnergy", addtionalEnergy,
                "IsEnergyBonusEvent", isEnergyBonusEvent ? 1 : 0);
        }
    }
}
