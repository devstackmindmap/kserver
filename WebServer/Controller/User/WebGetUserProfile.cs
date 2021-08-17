using AkaDB.MySql;
using CommonProtocol;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer.Controller.User
{
    public class WebGetUserProfile : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var userDb = new DBContext(req.UserId))
                {
                    UserProfile userProfile = new UserProfile(accountDb, userDb, req.UserId);
                    return await userProfile.GetUserProfile();
                }
            }
        }
    }
}
