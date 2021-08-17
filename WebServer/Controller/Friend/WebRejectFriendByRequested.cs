using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using AkaEnum;
using WebLogic.Friend;
using AkaLogger;

namespace WebServer.Controller.Friend
{
    public class WebRejectFriendByRequested : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;

            var friendManager = new FriendManager();
            using (var db = new DBContext(req.UserId))
            {
                await friendManager.RemoveRequestFriend(req.UserId, req.TargetId, db);
            }

            Log.Friend.FriendReject(req.UserId, req.TargetId);
            return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
