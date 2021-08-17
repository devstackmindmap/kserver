using AkaData;
using AkaDB.MySql;
using AkaEnum;
using Common.Entities.Item;
using System.Threading.Tasks;

namespace Common.Entities.User
{
    public class AddDeck : UserAdditionalInfo
    {
        public AddDeck(DBContext accountDb, DBContext userDb, uint userId, UserAdditionalInfoType userInfoType) : base(accountDb, userDb, userId, userInfoType)
        {

        }

        public override async Task<ResultType> Change(RequestValue requestValue)
        {
            var cost = (int)Data.GetConstant(DataConstantType.COST_ADD_DECK_RM).Value;
            var maxAddDeckCount = (int)Data.GetConstant(DataConstantType.MAX_ADD_DECK_COUNT).Value;
            var addDeck = await GetValue();

            if (maxAddDeckCount <= addDeck)
                return ResultType.Fail;

            var material = MaterialFactory.CreateMaterial(MaterialType.Gem, _userId, cost, _userDb);

            if (cost != 0 && false == await material.IsEnoughCount())
                return ResultType.NeedGem;

            await material.Use("AddDeck");

            await SetValue();
            return ResultType.Success;
        }

        private async Task SetValue()
        {
            _query.Clear();
            _query.Append("UPDATE user_info SET addDeck = addDeck + 1 WHERE userId =")
                .Append(_userId).Append(";");
            await _userDb.ExecuteNonQueryAsync(_query.ToString());
        }

        private async Task<int> GetValue()
        {
            _query.Clear();
            _query.Append("SELECT addDeck FROM user_info WHERE userId = ").Append(_userId).Append(";");
            using (var cursor = await _userDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return 0;
                return (int)cursor["addDeck"];
            }
        }
    }
}
