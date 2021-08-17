using AkaDB.MySql;
using AkaEnum;
using System.Threading.Tasks;

namespace Common
{
    public class ClanModifyMemberGrade : Clan
    {
        public ClanModifyMemberGrade(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ResultType> SetMemberGrade(uint targetId, ClanMemberGrade clanMemberGrade)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$userId", _userId),
                new InputArg("$targetId", targetId),
                new InputArg("$memberGrade", (int)clanMemberGrade)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outResultCode", MySql.Data.MySqlClient.MySqlDbType.Int32)
                );

            using (await _accountDb.CallStoredProcedureAsync(StoredProcedure.MODIFY_MEMBER_GRADE_CLAN, paramInfo))
            {
                var resultCode = paramInfo.GetOutValue<int>("$outResultCode");

                if (resultCode == 1)
                    return ResultType.Fail;

                return ResultType.Success;
            }
        }
    }
}
