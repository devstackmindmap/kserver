using AkaDB.MySql;
using Common;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebGetClanRecommend : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clanRecommend = new ClanRecommend(req.UserId, accountDb);
                return await clanRecommend.GetRecommendClans();
            }
        }
    }
}
