using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using System.Text;
using WebLogic.Friend;
using System;
using AkaEnum;
using AkaLogger;

namespace WebServer.Controller.Friend
{
    public class WebAddFriendByCode : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoAddInvite;

            FriendManager friendManager = new FriendManager();
            var res = new ProtoFriendInfo();
            using (var db = new DBContext("AccountDBSetting"))
            {
                if (req.InviteCode == "0")
                {
                    res.ResultType = ResultType.Fail;
                    return res;
                }

                var friendId = await friendManager.GetInviteCodeUserId(db, req.InviteCode);

                if (req.UserId == friendId || friendId == 0)
                {
                    res.ResultType = ResultType.Fail;
                    return res;
                }
                
                if (await friendManager.IsAlreadyFriend(req.UserId, friendId))
                {
                    res.ResultType = ResultType.AlreadyFriend;
                    return res;
                }

                if (await friendManager.IsMaxFriendCount(req.UserId, "friends", DataConstantType.MAX_FRIEND_COUNT))
                {
                    res.ResultType = ResultType.FullMyFriendCount;
                    return res;
                }

                if (await friendManager.IsMaxFriendCount(friendId, "friends", DataConstantType.MAX_FRIEND_COUNT))
                {
                    res.ResultType = ResultType.FullFriendsFriendCount;
                    return res;
                }

                await friendManager.AddFriend(req.UserId, friendId);
                await friendManager.AddFriend(friendId, req.UserId);
                res = await friendManager.GetUserInfo(friendId, db);
            }

            Log.Friend.FriendAddByCode(req.UserId, req.InviteCode);
            return res;
        }
    }
}
