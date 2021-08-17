using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using System.Text;
using AkaUtility;
using AkaEnum;
using WebLogic.Friend;
using AkaLogger;

namespace WebServer.Controller.Friend
{
    public class WebRequestFriendById : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoRequestFriend;
            var friendManager = new FriendManager();
            var resultType = await friendManager.RequestFriend(req.UserId, req.FriendId);

            Log.Friend.FriendRequestById(req.UserId, req.FriendId);
            return new ProtoResult { ResultType = resultType };
        }
    }
}
