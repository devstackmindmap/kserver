using AkaDB.MySql;
using Common;
using CommonProtocol;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebGetClanInviteCode : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var clan = new Common.Clan(req.UserId, accountDb);
                var memberGrade = await clan.GetClanMemberGrade();

                if (memberGrade == AkaEnum.ClanMemberGrade.Number4)
                    return null;

                var inviteCode = new ClanGetInviteCode(req.UserId, accountDb);
                return await inviteCode.GetInviteCode();
            }
        }
    }
}
