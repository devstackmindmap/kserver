
using AkaEnum;

namespace AkaLogger.Item
{
    public sealed class LogItem
    {
        public void MaterialGet(string userId, MaterialType materialType, string count, int totalCount, string logCategory)
        {
            Logger.Instance().Analytics("MaterialGet", "Item",
                "UserId", userId,
                "MaterialType", ((int)materialType).ToString(),
                "Count", count,
                "TotalCount", totalCount.ToString(),
                "LogCategory", logCategory);
        }

        public void MaterialUse(string userId, MaterialType materialType, int count, int totalCount, string logCategory)
        {
            Logger.Instance().Analytics("MaterialUse", "Item",
                "UserId", userId,
                "MaterialType", ((int)materialType).ToString(),
                "Count", (-1 * count).ToString(),
                "TotalCount", totalCount.ToString(),
                "LogCategory", logCategory);
        }

        public void PieceGet(string userId, string tableName, string classId, string count, string logCategory)
        {
            Logger.Instance().Analytics("PieceGet", "Item",
                "UserId", userId,
                "TableName", tableName,
                "ClassId", classId,
                "Count", count,
                "LogCategory", logCategory);
        }

        public void PieceUse(uint userId, PieceType pieceType, uint classId, int count, string logCategory)
        {
            Logger.Instance().Analytics("PieceUse", "Item",
                "UserId", userId.ToString(),
                "PieceType", (int)pieceType,
                "ClassId", classId.ToString(),
                "Count", (-1 * count).ToString(),
                "LogCategory", logCategory);
        }

        public void SkinGet(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("SkinGet", "Item",
                "UserId", userId.ToString(),
                "ClassId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void EmoticonGet(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("EmoticonGet", "Item",
                "UserId", userId.ToString(),
                "ClassId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void ContentGet(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("ContentGet", "Item",
                "UserId", userId.ToString(),
                "ClassId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void StageUnlock(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("StageUnlock", "Item",
                "UserId", userId.ToString(),
                "StageLevelId", classId.ToString(),
                "LogCategory", logCategory);
        }
        public void QuestUnlock(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("QuestUnlock", "Item",
                "UserId", userId.ToString(),
                "QuestGroupId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void SeasonPassUnlock(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("SeasonPassUnlock", "Item",
                "UserId", userId.ToString(),
                "SeasonPassId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void UserProfile(uint userId, uint classId, string logCategory)
        {
            Logger.Instance().Analytics("UserProfile", "Item",
                "UserId", userId.ToString(),
                "ClassId", classId.ToString(),
                "LogCategory", logCategory);
        }

        public void ProductBuyDigital(uint userId, uint productId, uint itemValue, string logCategory)
        {
            Logger.Instance().Analytics("ProductBuyDigital", "Item",
                "UserId", userId.ToString(),
                "ProductId", productId.ToString(),
                "ItemValue", itemValue.ToString(),
                "LogCategory", logCategory);
        }

        public void ProductBuyReal(uint userId, uint productId, PlatformType platformType, string transactionId, string logCategory)
        {
            Logger.Instance().Analytics("ProductBuyReal", "Item",
                "UserId", userId.ToString(),
                "ProductId", productId.ToString(),
                "PlatformType", ((int)platformType).ToString(),
                "TransactionId", transactionId,
                "LogCategory", logCategory);
        }

        public void EnergyGet(string userId, string count, string logCategory)
        {
            Logger.Instance().Analytics("EnergyGet", "Item",
                "UserId", userId,
                "Count", count,
                "LogCategory", logCategory);
        }

        public void BonusEnergyGet(string userId, string count, string logCategory)
        {
            Logger.Instance().Analytics("BonusEnergyGet", "Item",
                "UserId", userId,
                "Count", count,
                "LogCategory", logCategory);
        }
    }
}
