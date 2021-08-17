using System;

namespace AkaLogger.Users
{
    public sealed class LogPieceLevel
    {
        public void Log(uint userId, byte pieceType, uint classId, uint newLevel, string cardList,int nowGoldCount, int requireGoldCount,int nowPieceCount, int requirePieceCountForNextLevelUp)
        {
            Logger.Instance().Analytics("PieceLevel", "Piece",
                "UserId", userId.ToString(),
                "PieceType", pieceType.ToString(),
                "ClassId", classId.ToString(),
                "NewLevel", newLevel.ToString(),
                "NowGold", nowGoldCount.ToString(),
                "RequireGold", requireGoldCount.ToString(),
                "NowPiece", nowPieceCount.ToString(),
                "RequirePiece", requirePieceCountForNextLevelUp.ToString(),
                "CardList", cardList);
        }
    }
}
