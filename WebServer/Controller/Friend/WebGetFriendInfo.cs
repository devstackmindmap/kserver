using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using WebLogic.Friend;

namespace WebServer.Controller.Friend
{
    public class WebGetFriendInfo : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            var friendManager = new FriendManager();

            using (var db = new DBContext("AccountDBSetting"))
            {
                var userInfo = await friendManager.GetUserInfo(req.UserId, db);

                if (userInfo == null)
                    throw new System.Exception();

                return userInfo;
            }
        }
    }
}
