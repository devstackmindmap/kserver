using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.User;
using CommonProtocol;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebLogic.Friend;

namespace WebServer.Controller.SquareObject
{
    public class WebGetSquareObjectFriends : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var db = new DBContext(req.UserId))
                {
                    var clanId = await Common.Entities.Clan.ClanManager.GetClanId(accountDb, req.UserId);
                    var clan = new Common.Clan(req.UserId, accountDb);
                    var clanMembers = await clan.GetClanMemberUserIds(clanId);

                    var friendManager = new FriendManager();
                    var friends = await friendManager.GetFriendsUserIds(req.UserId, db);

                    var ids = friends.Union(clanMembers);

                    var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
                    var utcNow = DateTime.UtcNow.AddHours(-refreshBaseHour);
                    var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);
                    var userInfoManager = new UserInfoManager();
                    var needHelpUserIds = await userInfoManager.GetSquareObjectNeedHelpUsers(req.UserId, ids, initDateTime, accountDb);
                    var donateUserIds = await userInfoManager.GetSquareObjectDonatedMeUsers(req.UserId, initDateTime, accountDb);

                    var donatedMeUserIds = donateUserIds.Where(user => user.sendUserId != req.UserId).Select(user => user.sendUserId);
                    var donatedUserIds = donateUserIds.Where(user => user.receivedUserId != req.UserId).Select(user => user.receivedUserId);

                    var donateUserIdForNeedHelp = await userInfoManager.GetSquareObjectNeedHelpUsers(req.UserId, donatedMeUserIds, initDateTime, accountDb);


                    ids = needHelpUserIds.Union(donatedMeUserIds);
                    var users = await friendManager.GetUserInfos(ids, accountDb);
                    return new ProtoOnGetSquareObjectFriends
                    {
                        NeedHelpFriends = users.Where(userInfo => true == userInfo.IsActivatedSquareObject && needHelpUserIds.Contains(userInfo.UserId)).ToList(),
                        DonatedFriends = users.Where(userInfo => donatedMeUserIds.Contains(userInfo.UserId)).ToList(),
                        HelpedFriends = donatedUserIds.ToList(),
                        EnableHelpFriendsForDonated = donateUserIdForNeedHelp.ToList()
                    };
                }
            }
        }
    }
}
