using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using AkaRedisLogic;
using AkaUtility;
using Common.Entities.InfusionBox;
using Common.Entities.Season;
using Common.Entities.User;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class RankResult : BattleResult
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private uint _userId;
        private byte _deckNum;
        private ModeType _modeType;
        private RankType _rankType;

        private RankUnit _rankUnit;
        private RankUser _rankUser;
        private RankClan _rankClan;

        private ProtoOnBattleResultRank _protoOnBattleResult;
        private UnitsRankChangeData _unitsRankChangeData;
        private string _userCountryCode;
        private List<ProtoActionStatus> _actionStatusLog;
        private bool _isAi;

        public RankResult(DBContext accountDb, DBContext userDb, uint userId, byte deckNum,  ModeType modeType,
            ProtoOnBattleResultRank protoOnBattleResult, RankType rankType = RankType.AllUnitRankPoint, bool isAi = false) 
            : this(accountDb, userDb, userId, deckNum, 0, modeType,protoOnBattleResult, null, rankType, isAi)
        {
        }

        public RankResult(DBContext accountDb, DBContext userDb, uint userId, byte deckNum, int enemyTeamRankPoint, 
            ModeType modeType, ProtoOnBattleResultRank protoOnBattleResult, List<ProtoActionStatus> actionStatusLog, 
            RankType rankType = RankType.AllUnitRankPoint, bool isAi = false)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _deckNum = deckNum;
            _modeType = modeType;
            _rankType = rankType;
            _rankUnit = new RankUnit(_userDb, _userId, _deckNum.ToString(), enemyTeamRankPoint, ((byte)_modeType).ToString(), protoOnBattleResult, isAi);
            _protoOnBattleResult = protoOnBattleResult;
            _actionStatusLog = actionStatusLog;
            _isAi = isAi;
        }

        public async Task<(int rankPoint, uint winsCount)> GetUserRankPoint()
        {
            var rankUser = RankUserFactory.CreateRankUser(_accountDb, _userId, _rankType, 0, 0);
            var rankPoint = await rankUser.GetRankPoint();
            return (rankPoint, rankUser.GetWinsCount());
        }

        public override async Task<int> GetSumOfUnitsRankPoint()
        {
            return await _rankUnit.GetSumOfUnits();
        }

        protected override async Task Win()
        {
            _unitsRankChangeData = await _rankUnit.ApplyRankForWinAndLose(BattleResultType.Win);
            _rankUser = RankUserFactory.CreateRankUser(_accountDb, _userId, _rankType,
                _unitsRankChangeData.ChangePoint, _unitsRankChangeData.NextSeasonChangePoint);

            _rankUser.IsAi = _isAi;

            var userRankApplyData = await _rankUser.ApplyRank(1);

            _userCountryCode = _rankUser.CountryCode;
            _rankClan = new RankClan(_accountDb, _userId, _unitsRankChangeData.ChangePoint);
            await _rankClan.ApplyRankForWin();

            var userInfoChanger 
                = UserAdditionalInfoFactory.CreateUserInfoChanger(_accountDb, _userDb, _userId, 
                UserAdditionalInfoType.DailyGoldRewardByRankVictory);

            var resultType = await userInfoChanger.Change(null);

            SetBattleResultResponse(
                BattleResultType.Win,
                _unitsRankChangeData.UnitsRankData,
                userRankApplyData.RankData,
                _unitsRankChangeData.ChangePoint,
                resultType == ResultType.Success ? 
                await Reward.Reward.GetRewards(_userDb, _userId, userRankApplyData.RewardId, "RankWin") : null
                );

            var questProcessTypes = new Dictionary<QuestProcessType, int>() {
                    { QuestProcessType.KnightLeagueVictory , 1 }
                    ,{ QuestProcessType.DailyKnightLeagueVictory , 1 }
                    ,{ QuestProcessType.FinalRankPoint,  _protoOnBattleResult.UserRankData.CurrentSeasonRankPoint}
                };

            foreach( var actionStatus in _actionStatusLog)
            {
                if (actionStatus.ActionStatusType == ActionStatusType.KillUnit)
                {
                    actionStatus.Value++;
                    break;
                }
            }
            _ = UpdateQuestWithActions(questProcessTypes);
        }

        protected override async Task Lose()
        {
            _unitsRankChangeData = await _rankUnit.ApplyRankForWinAndLose(BattleResultType.Lose);
            _rankUser = RankUserFactory.CreateRankUser(_accountDb, _userId, _rankType,
                _unitsRankChangeData.ChangePoint, _unitsRankChangeData.NextSeasonChangePoint);
            _rankUser.IsAi = _isAi;

            _rankClan = new RankClan(_accountDb, _userId, _unitsRankChangeData.ChangePoint);
            await _rankClan.ApplyRankForLose();

            SetBattleResultResponse(
                BattleResultType.Lose,
                _unitsRankChangeData.UnitsRankData,
                (await _rankUser.ApplyRank(0)).RankData,
                _unitsRankChangeData.ChangePoint);

            _userCountryCode = _rankUser.CountryCode;

            _ = UpdateQuestWithActions(new Dictionary<QuestProcessType, int>());
        }

        protected override async Task Draw()
        {
            _rankUser = RankUserFactory.CreateRankUser(_accountDb, _userId, _rankType, 0, 0);
            _rankUser.IsAi = _isAi;

            SetBattleResultResponse(
                BattleResultType.Draw, 
                await _rankUnit.ApplyRankForDraw(), 
                await _rankUser.ApplyRankForDraw(), 
                0);
            _userCountryCode = _rankUser.CountryCode;

            _ = UpdateQuestWithActions(new Dictionary<QuestProcessType, int>());
        }

        private async Task UpdateQuestWithActions(Dictionary<QuestProcessType, int> targetQuest)
        {

            var serverSeason = new ServerSeason(_accountDb);
            var seasonInfo = await serverSeason.GetSeasonPassInfo();
            new Quest.QuestIO(_userId, seasonInfo.CurrentSeason, _userDb).UpdateQuest(targetQuest, 0, _actionStatusLog);
        }

        private void SetBattleResultResponse(BattleResultType battleResultType, 
            Dictionary<uint, ProtoRankData> unitsRankData, 
            ProtoRankData rankData, 
            int changeRankPoint, 
            List<ProtoItemResult> itemResults = null)
        {
            _protoOnBattleResult.BattleResultType = battleResultType;
            _protoOnBattleResult.UnitsRankData = unitsRankData;
            _protoOnBattleResult.UserRankData = rankData;
            _protoOnBattleResult.ChangedRankPoint = changeRankPoint;

            if (itemResults != null)
            {
                if (null == _protoOnBattleResult.ItemResults)
                    _protoOnBattleResult.ItemResults = new Dictionary<RewardCategoryType, List<ProtoItemResult>>(RewardCategoryTypeComparer.Comparer);
                _protoOnBattleResult.ItemResults.Add(RewardCategoryType.DailyVictory, itemResults);
            }
                
        }

        public override bool HasRedisJob()
        {
            return true;
        }

        public override async Task<bool> RedisJob(uint serverCurrentSeason, uint clanId, string clanCountryCode)
        {
            if (_unitsRankChangeData == null)
                return true;

            var redis = AkaRedis.AkaRedis.GetDatabase();

            return await GameBattleRankRedisJob.ChangePointKnightLeagueAsync(redis,
                _userId,
                _unitsRankChangeData.ChangePoint,
                _unitsRankChangeData.NextSeasonChangePoint,
                _unitsRankChangeData.UnitsChangePoints,
                serverCurrentSeason, clanId, _userCountryCode, clanCountryCode);
        }

        public override async Task<uint> SeasonJob()
        {
            SeasonUpdator seasonUpdator = new SeasonUpdator(_accountDb, _userDb, _userId);
            await seasonUpdator.SeasonUpdate();
            return await seasonUpdator.GetCurrentSeason();
        }

        public override string GetCountryCode()
        {
            return "";
        }

        public override async Task<ProtoNewInfusionBox> InfusionBoxJob(BattleResultType battleResultType)
        {
            var eventManager = new EventManager(_accountDb);
            var infusion = await InfusionBoxInfusionFactory.CreateInfusionBoxInfusion(_userId, InfusionBoxType.LeagueBox, _userDb, _accountDb);

            var additionalEnergy = GetAdditionalInfusionDefiniteEnergy();
            var isEnergyDoubleEvent = await eventManager.IsInEventProgress(EventType.DoubleEnergy);
            var newInfusionInfo 
                = await infusion.GetNewInfusionBox(
                    battleResultType == BattleResultType.Win ? true : false,
                    additionalEnergy,
                    await eventManager.IsInEventProgress(EventType.DoubleEnergy));
            await infusion.SetNewInfusionBox(newInfusionInfo);

            AkaLogger.Log.User.InfusionBox.Log(_userId,
                newInfusionInfo.Id,
                newInfusionInfo.UseUserEnergy,
                newInfusionInfo.UseUserBonusEnergy,
                newInfusionInfo.NewTotalBoxEnergy,
                newInfusionInfo.NewTotalUserBonusEnergy,
                newInfusionInfo.NewTotalUserEnergy,
                additionalEnergy,
                isEnergyDoubleEvent);
            return newInfusionInfo;
        }

        private int GetAdditionalInfusionDefiniteEnergy()
        {
            int sum = 0;
            if (null == _protoOnBattleResult.ItemResults)
                return sum;

            foreach (var items in _protoOnBattleResult.ItemResults)
            {
                foreach (var item in items.Value)
                {
                    if (item.ItemType == ItemType.Energy)
                        sum += item.Count;
                }
            }
            return sum;
        }
    }

    public class UnitsRankChangeData
    {
        public int ChangePoint;
        public int NextSeasonChangePoint;
        public Dictionary<uint, ProtoRankData> UnitsRankData = new Dictionary<uint, ProtoRankData>();

        public Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)> UnitsChangePoints 
            = new Dictionary<uint, (int UnitsChangePoint, int UnitsNextSeasonChangePoint)>();
    }
}
