using AkaDB.MySql;
using Common.Entities.Season;
using CommonProtocol;
using System.Threading.Tasks;
using WebLogic.User;

namespace WebServer.Controller.User
{
    public class WebGetCardProfile : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserIdAndId;

            var cards = AkaData.Data.GetCardByUnit(req.Id);

            using (var userDb = new DBContext(req.UserId))
            {
                CardProfile userProfile = new CardProfile(userDb, req.UserId, cards);
                var res = await userProfile.GetCardProfile();
                return res;
            }
        }
    }
}
