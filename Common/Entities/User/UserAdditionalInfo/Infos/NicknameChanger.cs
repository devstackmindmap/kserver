using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Item;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class NicknameChanger : UserAdditionalInfo
    {
        public NicknameChanger(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType) : base(accountDb, userDb, userId, userInfoType)
        {

        }

        public override async Task<ResultType> Change(RequestValue requestValue)
        {
            var IsAlreadyFreeNicknameChange = (await GetAdditionalUserInfo()).IsAlreadyFreeNicknameChange;
            int cost = 0;
            if (IsAlreadyFreeNicknameChange)
                cost = (int)Data.GetConstant(DataConstantType.COST_USER_NAME_CHANGE_BY_RM).Value;

            var material = MaterialFactory.CreateMaterial(MaterialType.Gem, _userId, cost, _userDb);
            if (cost != 0 && false == await material.IsEnoughCount())
                return ResultType.NeedGem;

            if (await IsNicknameDuplicate(requestValue.StringValue))
                return ResultType.NicknameDuplicate;

            if (cost > 0)
                await material.Use("NicknameChange");

            await UpdateNickname(requestValue.StringValue);

            if (false == IsAlreadyFreeNicknameChange)
                await SetValue();

            Log.User.NicknameChange.Log(_userId, requestValue.StringValue);
            return ResultType.Success;
        }

        protected async Task SetValue()
        {
            _query.Clear();
            _query.Append("INSERT INTO user_info (userId, isAlreadyFreeNicknameChange) VALUES (")
                .Append(_userId).Append(", 1) ON DUPLICATE KEY UPDATE isAlreadyFreeNicknameChange = 1;");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task UpdateNickname(string nickname)
        {
            _query.Clear();
            _query.Append("UPDATE accounts SET nickname = '")
                .Append(nickname).Append("'")
                .Append(" WHERE userId = ")
                .Append(_userId).Append(";");

            await _accountDb.ExecuteNonQueryAsync(_query.ToString());
        }

        protected async Task<bool> IsNicknameDuplicate(string nickname)
        {
            _query.Clear();
            _query.Append("SELECT userId FROM accounts WHERE nickname = '").Append(nickname).Append("';");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return true;

                return false;
            }
        }
    }
}
