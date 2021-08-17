using AkaEnum;
using CommonProtocol.Login;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnLogin : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public ProtoUserInfo UserInfo = new ProtoUserInfo();

        [Key(3)]
        public List<ProtoUnitInfo> UnitInfoList = new List<ProtoUnitInfo>();

        [Key(4)]
        public List<ProtoCardInfo> CardInfoList = new List<ProtoCardInfo>();

        [Key(5)]
        public List<ProtoWeaponInfo> WeaponInfoList = new List<ProtoWeaponInfo>();

        [Key(6)]
        public List<uint> SkinList = new List<uint>();

        [Key(7)]
        public List<ProtoEmoticonInfo> EmoticonList = new List<ProtoEmoticonInfo>();

        [Key(8)]
        public List<ProtoUserProfileInfo> UserProfileList = new List<ProtoUserProfileInfo>();

        [Key(9)]
        public List<ProtoQuestInfo> QuestList = new List<ProtoQuestInfo>();

        [Key(10)]
        public List<ProtoInfusionBox> InfusionBoxList = new List<ProtoInfusionBox>();
        
        [Key(11)]
        public ProtoBattlePlayingInfo BattlePlayingInfo;
        
        [Key(12)]
        public List<ProtoFriendInfo> Friends = new List<ProtoFriendInfo>();

        [Key(13)]
        public List<ProtoFriendInfo> RequestedFriends;

        [Key(14)]
        public List<ProtoFriendInfo> RecommendFriends;

        [Key(15)]
        public long NowServerDateTime;

        [Key(16)]
        public uint CurrentSeason;
        
        [Key(17)]
        public long NextSeasonStartDateTime;

        [Key(18)]
        public ProtoSquareObject SquareObjectInfo;

        [Key(19)]
        public ResultType ResultType = ResultType.Success;

        [Key(20)]
        public string Nickname;

        [Key(21)]
        public List<ProtoEvent> Events = new List<ProtoEvent>();

        [Key(22)]
        public List<ProtoEventChallengeDate> EventsChallenge = new List<ProtoEventChallengeDate>();

        [Key(23)]
        public uint ProfileIconId;

        [Key(24)]
        public string CountryCode;

        [Key(25)]
        public int GroupCode;

        [Key(26)]
        public ProtoClanProfileAndMembers ClanProfileAndMembers;

        [Key(27)]
        public ProtoOnGetProducts Products = new ProtoOnGetProducts();

        [Key(28)]
        public ProtoAdditionalUserInfo AddtionalUserInfo;

        [Key(29)]
        public string PubSubServerIp;

        [Key(30)]
        public int PubSubServerPort;

        [Key(31)]
        public int SeasonYear;

        [Key(32)]
        public uint SeasonYearNum;

        [Key(33)]
        public List<ProtoStoreInfo> PendingStoreInfos;

        [Key(34)]
        public ProtoMailInfo MailInfo;

        [Key(35)]
        public uint CurrentSeasonPass;

        [Key(36)]
        public long CurrentSeasonPassStartDateTime;

        [Key(37)]
        public long NextSeasonPassStartDateTime;

        [Key(38)]
        public List<uint> PurchasedSeasons;

        [Key(39)]
        public int PushAgree;

        [Key(40)]
        public int NightPushAgree;

        [Key(41)]
        public int SquareObjectDonationCount;

        [Key(42)]
        public uint Wins;

        [Key(43)]
        public ProtoChallengeStageList ChallengeStageList = new ProtoChallengeStageList();

        [Key(44)]
        public string NoticeMessage;
    }
}
