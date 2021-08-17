using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using AkaEnum;
using WebLogic.Friend;
using AkaLogger;

namespace WebServer.Controller.Friend
{
    public class WebRemoveFriend: BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;

            var friendManager = new FriendManager();
            using (var db = new DBContext(req.UserId))
            {
                if (false == await friendManager.IsAlreadyFriend(req.UserId, req.TargetId, db))
                    return new ProtoResult { ResultType = ResultType.Fail };

                await friendManager.RemoveFriendJobs(req.UserId, req.TargetId, db);
            }

            using (var db = new DBContext(req.TargetId))
            {
                await friendManager.RemoveFriendJobs(req.TargetId, req.UserId, db);
            }

            Log.Friend.FriendRemove(req.UserId, req.TargetId);
            return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
