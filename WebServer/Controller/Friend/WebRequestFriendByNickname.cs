using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using AkaEnum;
using WebLogic.Friend;
using AkaLogger;
using System;
using AkaUtility;
using Common.Entities.User;

namespace WebServer.Controller.Friend
{
    public class WebRequestFriendByNickname : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoAddInvite;

            var slangFilter = new SlangFilter();
            if (slangFilter.IsInvalidWord(req.InviteCode))
                return new ProtoResultWithFriendInfo { ResultType = ResultType.Fail };

            using (var db = new DBContext("AccountDBSetting"))
            {
                var userInfoManager = new UserInfoManager();
                var friendId = await userInfoManager.GetUserId(req.InviteCode, db);
                if (friendId == 0 || friendId == req.UserId)
                    return new ProtoResultWithFriendInfo { ResultType = ResultType.Fail };

                var friendManager = new FriendManager();
                var resultType = await friendManager.RequestFriend(req.UserId, friendId);
                    Log.Friend.FriendRequestByNickname(req.UserId, req.InviteCode);

                return new ProtoResultWithFriendInfo
                {
                    ResultType = resultType,
                    FriendInfo = resultType == ResultType.Success 
                    ? await friendManager.GetUserInfo(friendId, db)
                    :null
                };
            }
        }
    }
}
