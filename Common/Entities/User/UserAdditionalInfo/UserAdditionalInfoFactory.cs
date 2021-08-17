using AkaDB.MySql;
using AkaEnum;

namespace Common.Entities.User
{
    public class UserAdditionalInfoFactory
    {
        public static IUserInfoChanger CreateUserInfoChanger(DBContext accountInfo, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType)
        {
            switch(userInfoType)
            {
                case UserAdditionalInfoType.NicknameChange:
                    return new NicknameChanger(accountInfo, userDb, userId, userInfoType);
                case UserAdditionalInfoType.CountryCodeChange:
                    return new CountryChanger(accountInfo, userDb, userId, userInfoType);
                case UserAdditionalInfoType.DailyGoldRewardByRankVictory:
                    return new DailyGoldRewardByRankVictory(accountInfo, userDb, userId, userInfoType);
                case UserAdditionalInfoType.RewardedRankSeason:
                    return new RewardedRankSeason(null, userDb, userId, userInfoType);
                case UserAdditionalInfoType.AddDeck:
                    return new AddDeck(null, userDb, userId, userInfoType);
                default:
                    throw new System.Exception("Wrong UserInfoType");
            }
        }
    }
}
