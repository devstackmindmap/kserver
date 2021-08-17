using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class VirtualRank : BattleResult
    {
        private DBContext _userDb;
        private uint _userId;
        private byte _deckNum;
        private ModeType _modeType;
        private RankType _rankType;

        private VirtualRankUnit _rankUnit;
        private RankUser _rankUser;

        private ProtoOnBattleResultRankData _protoOnBattleResult;
        private UnitsVirtualRankChangeData _unitsRankChangeData;
        private string _userCountryCode;

        public VirtualRank (DBContext userDb, uint userId, byte deckNum, ModeType modeType,
            ProtoOnBattleResultRankData protoOnBattleResult, RankType rankType = RankType.AllUnitRankPoint)
        {
            _userDb = userDb;
            _userId = userId;
            _deckNum = deckNum;
            _modeType = modeType;
            _rankType = rankType;
            _rankUnit = new VirtualRankUnit(_userDb, _userId, _deckNum, _modeType, protoOnBattleResult);
            _protoOnBattleResult = protoOnBattleResult;
        }

        public VirtualRank( DBContext userDb, uint userId, byte deckNum, ModeType modeType, RankType rankType = RankType.AllUnitRankPoint)
            : this( userDb, userId, deckNum, modeType, null, rankType)
        {
        }

        public async Task<int> GetSumOfUnitsRankPoint()
        {
            return await _rankUnit.GetSumOfUnits();
        }

        public async Task<int> GetUserRankPoint()
        {
            return await RankUserFactory.CreateRankUser(_userDb, _userId, _rankType, 0, 0).GetRankPoint();
        }


        protected override async Task Win()
        {
            _unitsRankChangeData = await _rankUnit.ApplyRankForWinAndLose(BattleResultType.Win);
            _rankUser = RankUserFactory.CreateRankUser(_userDb, _userId, _rankType, _unitsRankChangeData.ChangePoint, 0);

            var userRankApplyData = await _rankUser.ApplyRank(1);
            _userCountryCode = _rankUser.CountryCode;

            SetBattleResultResponse(
                BattleResultType.Win,
                _unitsRankChangeData.UnitsRankData,
                userRankApplyData.RankData,
                _unitsRankChangeData.ChangePoint
                );

            new Quest.QuestIO(_userId).UpdateQuest(
                new Dictionary<QuestProcessType, int>() {
                    
                    { QuestProcessType.FinalVirtualRankPoint,  _protoOnBattleResult.UserRankData.CurrentSeasonRankPoint},
                    
                }, 0 ,null);
        }

        protected override async Task Lose()
        {
            _unitsRankChangeData = await _rankUnit.ApplyRankForWinAndLose(BattleResultType.Lose);
            _rankUser = RankUserFactory.CreateRankUser(_userDb, _userId, _rankType, _unitsRankChangeData.ChangePoint, 0);

            SetBattleResultResponse(
                BattleResultType.Lose,
                _unitsRankChangeData.UnitsRankData,
                (await _rankUser.ApplyRank(0)).RankData,
                _unitsRankChangeData.ChangePoint);

            _userCountryCode = _rankUser.CountryCode;
        }

        protected override async Task Draw()
        {
            _rankUser = RankUserFactory.CreateRankUser(_userDb, _userId, _rankType, 0, 0);

            SetBattleResultResponse(
                BattleResultType.Draw, 
                await _rankUnit.ApplyRankForDraw(), 
                await _rankUser.ApplyRankForDraw(), 
                0);
            _userCountryCode = _rankUser.CountryCode;
        }

        private void SetBattleResultResponse(BattleResultType battleResultType, 
            Dictionary<uint, ProtoRankData> unitsRankData, 
            ProtoRankData rankData, 
            int changeRankPoint)
        {
            _protoOnBattleResult.BattleResultType = battleResultType;
            _protoOnBattleResult.UnitsRankData = unitsRankData;
            _protoOnBattleResult.UserRankData = rankData;
            _protoOnBattleResult.ChangedRankPoint = changeRankPoint;
        }

        public override bool HasRedisJob()
        {
            return false;
        }

        public override async Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode)
        {
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

    public class UnitsVirtualRankChangeData
    {
        public int ChangePoint;
        public Dictionary<uint, ProtoRankData> UnitsRankData = new Dictionary<uint, ProtoRankData>();

        public Dictionary<uint, int > UnitsChangePoints = new Dictionary<uint, int >();
    }
}
