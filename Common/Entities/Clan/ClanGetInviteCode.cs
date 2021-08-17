using AkaDB.MySql;
using AkaUtility;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common
{
    public class ClanGetInviteCode : ClanJoin
    {
        public ClanGetInviteCode(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ProtoInviteCode> GetInviteCode()
        {
            var clanInfo = await GetClanInfo();
            if (clanInfo == null)
                return null;

            string invateCode = clanInfo.InviteCode;
            if (invateCode == "0")
                invateCode = await GetNewInviteCode(clanInfo.ClanId);

            return new ProtoInviteCode
            {
                InviteCode = invateCode,
                CountryCode = clanInfo.CountryCode
            };
        }

        private async Task<string> GetNewInviteCode(uint clanId)
        {
            var newInviteCode = Utility.RandomString();
            _query.Clear();
            _query.Append("UPDATE clans SET inviteCode='").Append(newInviteCode)
                .Append("' WHERE clanId=").Append(clanId).Append(";");
            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
            return newInviteCode;
        }
    }
}
