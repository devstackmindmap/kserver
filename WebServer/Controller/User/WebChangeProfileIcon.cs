using AkaDB.MySql;
using CommonProtocol;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.User
{
    public class WebChangeProfileIcon : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoChangeProfileIcon;

            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                
                if (req.ProfileIconType == AkaEnum.ProfileIconType.User)
                {
                    var query = new StringBuilder();
                    query.Append("UPDATE accounts SET profileIconId = ")
                        .Append(req.ProfileIconId)
                        .Append(" WHERE userId=").Append(req.UserId).Append(";");

                    await accountDb.ExecuteNonQueryAsync(query.ToString());
                }
            }

            return new ProtoUserId { UserId = req.UserId };
        }
    }
}
