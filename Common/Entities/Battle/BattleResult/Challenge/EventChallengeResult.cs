using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common.Entities.Challenge;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class EventChallengeResult : BattleResult
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private uint _userId;

        private ProtoBattleResultEventChallenge _protoBattleResult;
        private ProtoOnBattleResult _protoOnBattleResult;

        public EventChallengeResult(DBContext accountDb, DBContext userDb, uint userId,
            ProtoBattleResultEventChallenge protoBattleResult ,ProtoOnBattleResult protoOnBattleResult)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _protoBattleResult = protoBattleResult;
            _protoOnBattleResult = protoOnBattleResult;
        }

        protected override async Task Win()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Win;
            var challenge = ChallengeFactory.CreateEventChallengeManager(_accountDb, _userDb, _userId,
                _protoBattleResult.ChallengeEventId, _protoBattleResult.DifficultLevel);

            var itemResult = await challenge.Clear(_protoBattleResult.PlayerInfoList[0].DeckNum);

            if (itemResult != null)
            {
                if (null == _protoOnBattleResult.ItemResults)
                    _protoOnBattleResult.ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>(RewardCategoryTypeComparer.Comparer);
                _protoOnBattleResult.ItemResults.Add(RewardCategoryType.StageClear, itemResult);
            }
                
        }

        protected override async Task Lose()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Lose;
        }

        protected override async Task Draw()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Draw;
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
