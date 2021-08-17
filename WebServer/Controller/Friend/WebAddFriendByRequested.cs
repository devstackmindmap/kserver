using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using AkaEnum;
using WebLogic.Friend;
using AkaLogger;

namespace WebServer.Controller.Friend
{
    public class WebAddFriendByRequested : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;

            if (req.UserId == req.TargetId || req.UserId == 0 || req.TargetId == 0)
                return new ProtoResult { ResultType = ResultType.Fail };

            var friendManager = new FriendManager();
            using (var db = new DBContext(req.UserId))
            {
                if (await friendManager.IsAlreadyFriend(req.UserId, req.TargetId, db))
                    return new ProtoResult { ResultType = ResultType.AlreadyFriend };

                if (false == await friendManager.IsRequestedFriend(req.UserId, req.TargetId, db))
                    return new ProtoResult { ResultType = ResultType.NotRequestedFriend };

                if (await friendManager.IsMaxFriendCount(req.UserId, "friends", DataConstantType.MAX_FRIEND_COUNT, db))
                    return new ProtoResult { ResultType = ResultType.FullMyFriendCount };
            }

            if (await friendManager.IsMaxFriendCount(req.TargetId, "friends", DataConstantType.MAX_FRIEND_COUNT))
                return new ProtoResult { ResultType = ResultType.FullFriendsFriendCount};

            await friendManager.AddFriend(req.UserId, req.TargetId);
            await friendManager.AddFriend(req.TargetId, req.UserId);

            Log.Friend.FriendAddByRequested(req.UserId, req.TargetId);
            return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
