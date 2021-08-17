using AkaDB.MySql;
using AkaEnum;
using Common.Entities.User;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.User
{
    public class WebTermsAgreeUpdate : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdByteValue;
            if (req.Value != 1)
                return new ProtoResult { ResultType = ResultType.DeniedTerms };

            using (var userDb  = new DBContext(req.UserId))
            {
                var pushKeyManager = new PushKeyManager(userDb, req.UserId);
                await pushKeyManager.UpdateTermsAgree();
            }

            return new ProtoResult { ResultType = ResultType.Success };
        }
    }
}
