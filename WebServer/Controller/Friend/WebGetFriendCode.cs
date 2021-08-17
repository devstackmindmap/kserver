using System.Threading.Tasks;
using CommonProtocol;
using AkaDB.MySql;
using System.Text;
using AkaUtility;

namespace WebServer.Controller.Friend
{
    public class WebGetFriendCode : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoUserId;

            using (var db = new DBContext("AccountDBSetting"))
            {
                var query = new StringBuilder();
                query.Append("SELECT friendCode, countryCode FROM accounts WHERE userId=").Append(req.UserId).Append(";");
                var cursor = await db.ExecuteReaderAsync(query.ToString());
                if (cursor.Read() == false)
                    throw new System.Exception();

                var inviteCode = cursor.GetString(0);
                var countryCode = cursor.GetString(1);

                if (inviteCode == "0")
                    inviteCode = await GetNewFriendCode(db, req.UserId);

                return new ProtoInviteCode
                {
                    InviteCode = inviteCode,
                    CountryCode = countryCode
                };
            }
        }

        private async Task<string> GetNewFriendCode(DBContext db, uint userId)
        {
            var newFriendCode = Utility.RandomString();
            var query = new StringBuilder();
            query.Append("UPDATE accounts SET friendCode='").Append(newFriendCode).Append("' WHERE userId=").Append(userId).Append(";");
            await db.ExecuteNonQueryAsync(query.ToString());
            return newFriendCode;
        }
    }
}
