using AkaDB.MySql;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebGetClanProfileAndMembers : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdTargetId;
            
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new Common.Clan(req.UserId, accountDb);
                return await clan.GetClanProfileAndMembers(req.TargetId);
            }
        }
    }
}
