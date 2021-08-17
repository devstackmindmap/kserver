using System;

namespace AkaLogger.Users
{
    public sealed class LogEmoticon
    {
        public void Log(uint userId, uint unitId, uint emoticonId, int orderNum)
        {
            Logger.Instance().Analytics("SetEmoticons", "Emoticon",
                "UserId", userId.ToString(),
                "UnitId", unitId.ToString(),
                "EmoticonId", emoticonId.ToString(),
                "OrderNum", orderNum.ToString()
                );
        }

        public void Log(string roomId, bool isPlayer, string playerType, uint emoticonId, uint player1UserId, uint player2UserId)
        {
            Logger.Instance().Analytics("UseEmoticon", "Emoticon",
                "RoomId", roomId,
                "UsePlayer", playerType,
                "IsPlayer", isPlayer.ToString(),
                "EmoticonId", emoticonId.ToString(),
                "Player1UserId", player1UserId.ToString(),
                "player2UserId", player1UserId.ToString()
                );
        }
    }
}
