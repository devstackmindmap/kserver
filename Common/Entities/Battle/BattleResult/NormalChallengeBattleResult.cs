using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkaEnum.Battle;
using Common.Entities.Stage;

namespace Common.Entities.Battle
{
    public class NormalChallengeBattleResult : BattleResult
    {
        private DBContext _userDb;
        private uint _userId;
        private string _strUserId;
        private BattleType _battleType;
        private ProtoOnBattleResult _protoOnBattleResult;

        private StageInfo _stageInfo;

        public NormalChallengeBattleResult(DBContext userDb, uint userId, uint stageLevelId, ProtoOnBattleResult protoOnBattleResult, BattleType battleType = BattleType.None)
        {
            _userDb = userDb;
            _userId = userId;
            _battleType = battleType;
            _strUserId = userId.ToString();
            _protoOnBattleResult = protoOnBattleResult;
            _stageInfo = new StageInfo(userDb, userId, stageLevelId, battleType);
        }

        protected override async Task Win()
        {
            await SetClearCount();
            ProtoOnStageLevelExit protoOnStageLevelExit = new ProtoOnStageLevelExit();
            
            _protoOnBattleResult.BattleResultType = BattleResultType.Win;


        }

        private async Task SetClearCount()
        {
            //var manager = ChallengeFactory.CreateNormalChallengeManager(null, _userDb, _userId, )
        }

        protected override async Task Lose()
        {
            _protoOnBattleResult.BattleResultType = BattleResultType.Lose;
        }

        protected override async Task Draw()
        {
            await Lose();
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
