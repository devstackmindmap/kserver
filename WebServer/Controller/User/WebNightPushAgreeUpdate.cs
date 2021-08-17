using AkaDB.MySql;
using AkaEnum;
using Common.Entities.User;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.User
{
    public class WebNightPushAgreeUpdate : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdByteValue;

            using (var userDb  = new DBContext(req.UserId))
            {
                var pushKeyManager = new PushKeyManager(userDb, req.UserId, req.Value);
                await pushKeyManager.UpdateNightPushAgree();
            }

            return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
