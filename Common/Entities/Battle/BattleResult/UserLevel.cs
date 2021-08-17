using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaUtility;
using CommonProtocol;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{

    public class UserLevel : BattleResult
    {
        public uint Level { get; set; }
        public ulong exp { get; set; }

        private DBContext _db;
        private uint _userId;
        private BattleType _battleType;
        private DataContentsConstant _contentsConstant;
        private ProtoOnBattleResult _protoOnBattleResult;

        public UserLevel(DBContext db, BattleType battleType , uint userId, ProtoOnBattleResult protoOnBattleResult)
        {
            _db = db;
            _userId = userId;
            _battleType = battleType;
            _contentsConstant = Data.GetContentsConstant(_battleType);
            _protoOnBattleResult = protoOnBattleResult;
        }

        protected override async Task Win()
        {
            await ExecuteAsync(_contentsConstant.UserExpWithWin);
        }

        protected override async Task Lose()
        {
            await ExecuteAsync(_contentsConstant.UserExpWithLose);
        }

        protected override async Task Draw()
        {
            await ExecuteAsync(_contentsConstant.UserExpWithDraw);
        }

        private async Task<DbDataReader> Select()
        {
            var strUserId = _userId.ToString();
            var cursor = await _db.ExecuteReaderAsync($"SELECT level, exp FROM users WHERE userId = {strUserId} ;");
            cursor.Read();
            return cursor;
        }

        private async Task Update(uint level, ulong exp)
        {
            var strLevel = level.ToString();
            var strExp = exp.ToString();
            var strUserId = _userId.ToString();
            await _db.ExecuteNonQueryAsync($"UPDATE users set level = {strLevel}, exp = {strExp} where userId = {strUserId}");
            AkaLogger.Log.User.UserLevel.Log(_userId, level, exp);
        }

        private async Task ExecuteAsync(uint gettingExp)
        {
            if (gettingExp == 0)
            {
                _protoOnBattleResult.UserLevelAndExp = new ProtoUserExp();
                return;
            }

            using (var cursor = await Select())
            {
                var protoUserExp = new ProtoUserExp
                {
                    OldLevel = (uint)cursor["level"],
                    OldExp = (ulong)cursor["exp"],
                    GettingExp = gettingExp,
                };

                protoUserExp.Exp = protoUserExp.OldExp + gettingExp;
                protoUserExp.Level = protoUserExp.OldLevel;

                var userLevelInfo = Data.GetUserLevel(protoUserExp.OldLevel);

                if (protoUserExp.Exp >= userLevelInfo.NeedExpForNextLevelUp)
                {
                    var rewards = await Reward.Reward.GetRewards(_db, _userId, userLevelInfo.RewardId, "UserLevelUp");
                    if (null == _protoOnBattleResult.ItemResults)
                        _protoOnBattleResult.ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>(RewardCategoryTypeComparer.Comparer);

                    _protoOnBattleResult.ItemResults.Add(RewardCategoryType.UserLevelUp, rewards);
                    protoUserExp.Level++;
                    protoUserExp.Exp -= userLevelInfo.NeedExpForNextLevelUp;
                }

                await Update(protoUserExp.Level, protoUserExp.Exp);
                _protoOnBattleResult.UserLevelAndExp = protoUserExp;
            }
        }

        public override bool HasRedisJob()
        {
            return false;
        }


        public override async Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode)
        {
            //Nothing
            return true;
        }

        public override async Task<uint> SeasonJob()
        {
            return 0;
        }

        public override string GetCountryCode()
        {
            return "";
        }

        public override async Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType)
        {
            return null;
        }
    }
}
