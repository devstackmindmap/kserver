namespace Common
{
    public static class InitialValue
    {
        public const string START_PIECE_LEVEL = "1";
        public const string START_PIECE_COUNT = "0";
        public const int STAGE_CLEAR_COUNT = 0;
        public const string START_UNIT_RANK_POINT = "0";
        public const string START_UNIT_RANK_LEVEL = "1";
        public const int START_BOX_ENERGY = 0;
        public const string START_WEAPON_LEVEL = "1";
        public const string START_WEAPON_COUNT = "0";
    }

    public static class ConstValue
    {
        public const double FLOAT_EPSILON = 0.000001d;
        public const int NORMAL_LEAGUE_PROBABILITY_GROUP_ID = 1;
        public const int MAX_MATCHING_TARGET_EXPANSION_COUNT = 3;
        public const int ENTER_ROOM_WAITING_SECOND = 10;
        public const int MATCHING_CANCEL_WAITING_MILLISECOND = 700;
        public const int BATTLE_STAGE_EXPIRE_TIME_MILLISEC = 1000000;
        public const int BATTLE_EXPIRED_STAGE_COLLECT_TIME_MILLISEC = 1000000;

        public const int SERVERINFO_ORDERING_MULTIPLE_NUM = 10000000;
        public const int SCHEDULE_BUFFER_HOUR = 4;
        public const int MAX_SQUARE_OBJECT_INVASION_COUNT = short.MaxValue / 50 - 10;
        public const int SQUARE_OBJECT_INVADING_SECOND = 30;
    }

    public static class TableName
    {
        public const string UNIT = "units";
        public const string CARD = "cards";
        public const string WEAPON = "weapons";
        public const string INFUSION_BOX = "infusion_boxes";
        public const string CORRECTION = "corrections";
        public const string EMOTICON = "emoticons";
        public const string QUEST = "quests";
        public const string PROFILE = "profiles";
        public const string USER_MAIL_PUBLIC = "user_mail_public";
        public const string USER_MAIL_PRIVATE = "user_mail_private";
        public const string USER_MAIL_SYSTEM = "user_mail_system";
        public const string CHALLENGE_STAGE = "challenge_stage";
        public const string CHALLENGE_EVENT_STAGE = "challenge_event_stage";
    }

    public static class ColumnName
    {
        public const string PIECE_ID = "id";
        public const string PIECE_LEVEL = "level";
        public const string PIECE_COUNT = "count";
        public const string USER_GOLD = "gold";
        public const string STAR_COIN = "starCoin";
        public const string USER_GEM = "gem";
        public const string USER_GEM_PAID = "gemPaid";
        public const string BOX_ENERGY = "boxEnergy";
        public const string USER_ENERGY = "userEnergy";
        public const string USER_BONUS_ENERGY = "userBonusEnergy";
        public const string USER_ENERGY_RECENT_UPDATE_DATETIME = "userEnergyRecentUpdateDatetime";
        public const string SQUARE_OBJECT_START_TICKET = "soStartTicket";
        public const string EVENT_COIN = "eventCoin";
        public const string CHALLENGE_COIN = "challengeCoin";
    }

    public static class StoredProcedure
    {
        public const string GET_ACCOUNT = "p_getAccount";
        public const string GET_LOGIN_INFO = "p_getLoginInfo";
        public const string UNIT_LEVEL_UP = "p_unitLevelUp";
        public const string CARD_LEVEL_UP = "p_cardLevelUp";
        public const string WEAPON_LEVEL_UP = "p_weaponLevelUp";
        public const string UPDATE_STAGECLEAR_COUNT = "p_upsertStageClearCount";
        public const string ADD_BATTLE_RECORD = "p_addBattleRecord";
        public const string SQUAREOBJECT_LEVEL_UP = "p_squareObjectLevelUp";
        public const string SQUAREOBJECT_CORE_LEVEL_UP = "p_squareObjectCoreLevelUp";
        public const string SQUAREOBJECT_AGENCY_LEVEL_UP = "p_squareObjectAgencyLevelUp";
        public const string CREATE_CLAN = "p_createClan";
        public const string JOIN_CLAN = "p_joinClan";
        public const string OUT_CLAN = "p_outClan";
        public const string MODIFY_MEMBER_GRADE_CLAN = "p_modifyMemberGrade";
        public const string MODIFY_PROFILE_CLAN = "p_modifyProfileClan";
        public const string GET_PRODUCTS = "p_getProducts";
        public const string GET_MAIL_INFO = "p_getMailInfo";
    }
}
