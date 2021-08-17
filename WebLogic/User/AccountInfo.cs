using System;

namespace WebLogic.User
{
    public class AccountInfo
    {
        public uint UserId;
        public uint InitDataVersion;
        public string Nickname;
        public bool IsNicknameDuplicate;
        public uint ProfileIconId;
        public string CountryCode;
        public DateTime LimitLoginDateTime;
        public string LimitLoginReason;
        public uint Wins;
    }
}
