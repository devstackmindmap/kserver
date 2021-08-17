using AkaEnum;

namespace AkaLogger.Clan
{
    public sealed class LogClan
    {
        public void ClanCreate(uint userId, uint clanId, string clanName, uint clanSymbolId, ClanPublicType clanPublicType,
            int joinConditionRankPoint, string countryCode, string clanExplain)
        {
            Logger.Instance().Analytics("ClanCreate", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString(),
                "ClanName", clanName,
                "ClanSymbolId", clanSymbolId.ToString(),
                "ClanPublicType", ((int)clanPublicType).ToString(),
                "JoinConditionRankPoint", joinConditionRankPoint.ToString(),
                "CountryCode", countryCode,
                "ClanExplain", clanExplain);
        }

        public void ClanJoin(uint userId, uint clanId)
        {
            Logger.Instance().Analytics("ClanJoin", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString());
        }

        public void ClanJoinByCode(uint userId, uint clanId)
        {
            Logger.Instance().Analytics("ClanJoinByCode", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString());
        }

        public void ClanOut(uint userId, uint clanId)
        {
            Logger.Instance().Analytics("ClanOut", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString());
        }

        public void ClanBanish(uint userId, uint clanId, uint targetUserId)
        {
            Logger.Instance().Analytics("ClanBanish", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString(),
                "TargetUserId", targetUserId.ToString());
        }

        public void ClanModifyGrade(uint userId, uint targetUserId, ClanMemberGrade clanMemberGrade)
        {
            Logger.Instance().Analytics("ClanModifyGrade", "Clan",
                "UserId", userId.ToString(),
                "TargetUserId", targetUserId.ToString(),
                "clanMemberGrade", ((int)clanMemberGrade).ToString());
        }

        public void ClanProfileModify(uint userId, uint clanId, uint clanSymbolId, ClanPublicType clanPublicType,
            int joinConditionRankPoint, string countryCode, string clanExplain)
        {
            Logger.Instance().Analytics("ClanProfileModify", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString(),
                "ClanSymbolId", clanSymbolId.ToString(),
                "ClanPublicType", ((int)clanPublicType).ToString(),
                "JoinConditionRankPoint", joinConditionRankPoint.ToString(),
                "CountryCode", countryCode,
                "ClanExplain", clanExplain);
        }

        public void ClanDelete(uint userId, uint clanId)
        {
            Logger.Instance().Analytics("ClanDelete", "Clan",
                "UserId", userId.ToString(),
                "ClanId", clanId.ToString());
        }

    }
}
