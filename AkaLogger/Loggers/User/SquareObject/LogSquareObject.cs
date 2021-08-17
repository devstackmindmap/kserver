using AkaEnum;
using System;

namespace AkaLogger.Users
{
    public sealed class LogSquareObject
    {
        public void StartLog(uint userId, DateTime startTime, DateTime invasionTime, uint monsterLevel, uint monsterId, uint activatedLevel, uint objectLevel, uint coreLevel, uint agencyLevel)
        {
            Logger.Instance().Analytics("Start", "Square",
                "UserId", userId.ToString(),
                "StartTime", startTime.ToLog(),
                "InvasionTime", invasionTime.ToLog(),
                "MonsterLevel", monsterLevel.ToString(),
                "MonsterId", monsterId.ToString(),
                "ActivatedLevel", activatedLevel.ToString(),
                "ObjectLevel", objectLevel.ToString(),
                "CoreLevel", coreLevel.ToString(),
                "AgencyLevel", agencyLevel.ToString()
                );
        }

        public void RewardLog(bool isDestroyed, uint userId, DateTime startTime, DateTime stopTime, DateTime invasionTime, uint monsterLevel, uint monsterId, uint boxLevel, uint rewardId, int power, int shield, bool destroyed, uint activatedLevel, uint objectLevel, uint coreLevel, uint agencyLevel)
        {
            Logger.Instance().Analytics("Reward", "Square",
                "UserId", userId.ToString(),
                "StartTime", startTime.ToLog(),
                "StopTime", stopTime.ToLog(),
                "InvasionTime", invasionTime.ToLog(),
                "MonsterLevel", monsterLevel.ToString(),
                "MonsterId", monsterId.ToString(),
                "Power", power.ToString(),
                "Shield", shield.ToString(),
                "Destoryed", destroyed.ToString(),
                "ActivatedLevel", activatedLevel.ToString(),
                "ObjectLevel", objectLevel.ToString(),
                "CoreLevel", coreLevel.ToString(),
                "AgencyLevel", agencyLevel.ToString(),
                "RewardId", rewardId.ToString(),
                "BoxLevel", boxLevel.ToString(),
                "IsDestroyed", isDestroyed.ToString()
                );
        }

        public void InvasionLog(uint userId, DateTime startTime, DateTime invasionTime, uint monsterLevel, uint monsterId, uint boxLevel, int power, int shield, bool destroyed, uint activatedLevel, uint objectLevel, uint coreLevel, uint agencyLevel)
        {
            Logger.Instance().Analytics("Invaded", "Square",
                "UserId", userId.ToString(),
                "StartTime", startTime.ToLog(),
                "InvasionTime", invasionTime.ToLog(),
                "MonsterLevel", monsterLevel.ToString(),
                "MonsterId", monsterId.ToString(),
                "Power", power.ToString(),
                "Shield", shield.ToString(),
                "Destoryed", destroyed.ToString(),
                "ActivatedLevel", activatedLevel.ToString(),
                "ObjectLevel", objectLevel.ToString(),
                "CoreLevel", coreLevel.ToString(),
                "AgencyLevel", agencyLevel.ToString(),
                "BoxLevel", boxLevel.ToString()
                );
        }

        public void UseEnergyLog(uint userId, int boxEnergy, int powerEnery, uint boxLevel, int power, uint coreLevel, uint agencyLevel, bool updated)
        {
            Logger.Instance().Analytics("Energy", "Square",
                "UserId", userId.ToString(),
                "BoxEnergy", boxEnergy.ToString(),
                "PowerEnergy", powerEnery.ToString(),
                "Power", power.ToString(),
                "BoxLevel", boxLevel.ToString(),
                "CoreLevel", coreLevel.ToString(),
                "AgencyLevel", agencyLevel.ToString(),
                "Updated",updated
                );
        }

        public void UpdateNextInvasionLog(uint userId, DateTime startTime, DateTime invasionTime, uint monsterLevel, uint monsterId, uint activatedLevel, uint objectLevel, uint coreLevel, uint agencyLevel)
        {
            Logger.Instance().Analytics("NextInvasion", "Square",
                "UserId", userId.ToString(),
                "StartTime", startTime.ToLog(),
                "InvasionTime", invasionTime.ToLog(),
                "MonsterLevel", monsterLevel.ToString(),
                "MonsterId", monsterId.ToString(),
                "ActivatedLevel", activatedLevel.ToString(),
                "ObjectLevel", objectLevel.ToString(),
                "CoreLevel", coreLevel.ToString(),
                "AgencyLevel", agencyLevel.ToString()
                );
        }

        public void Donate(uint sendUserId, uint receivedUserId, SquareObjectResponseType result)
        {
            Logger.Instance().Analytics("Donate", "Square",
                "SendUserId", sendUserId.ToString(),
                "ReceivedUserId", receivedUserId.ToString(),
                "Result", result.ToString()
                );

        }
    }
}
