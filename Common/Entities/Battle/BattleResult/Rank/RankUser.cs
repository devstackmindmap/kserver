using AkaData;
using AkaDB.MySql;
using AkaEnum;
using CommonProtocol;
using System;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public abstract class RankUser
    {
        protected DBContext _db;
        protected uint  _userId;
        protected string _strRankType;
        protected ProtoRankData _rankData;
        protected int _changeRankPoint;
        protected int _nextSeasonChangeRankPoint;
        public string CountryCode = "";

        public bool IsAi { get; set; }

        protected abstract Task GetDbUserRankData();

        protected abstract Task UpdateUserRankInfo(uint newRankLevel, int newRankPoint, int nextSeasonRankPoint, int maxRankPoint, int addVictoryCount , uint winsCount);

        public RankUser(DBContext db, uint userId, RankType rankType, int changeRankPoint, int nextSeasonChangeRankPoint)
        {
            _db = db;
            _changeRankPoint = changeRankPoint;
            _nextSeasonChangeRankPoint = nextSeasonChangeRankPoint;
            _userId = userId;
            _strRankType = ((int)rankType).ToString();
        }

        public async Task<(uint RewardId, ProtoRankData RankData)> ApplyRank(int addVictoryCount)
        {
            await GetDbUserRankData();
            var newRankLevel = GetNextRankLevel();
            var newRankPoint = GetNextRankPoint();
            var maxRankPoint = GetMaxRankPoint();
            var nextSeasonRankPoint = _rankData.CurrentSeasonRankPoint + _nextSeasonChangeRankPoint;
            var winCount = IsAi ? _rankData.Wins : addVictoryCount == 0 ? 0 : _rankData.Wins + 1;

            await UpdateUserRankInfo(newRankLevel, newRankPoint, nextSeasonRankPoint, maxRankPoint, addVictoryCount, winCount);
            return (Data.GetUserRank(Data.GetUserRankLevelByPoint(_rankData.CurrentSeasonRankPoint)).RewardId,
                new ProtoRankData { MaxRankLevel = newRankLevel, CurrentSeasonRankPoint = newRankPoint , Wins = winCount });
        }

        public async Task<ProtoRankData> ApplyRankForDraw()
        {
            await GetDbUserRankData();

            return new ProtoRankData { MaxRankLevel = _rankData.MaxRankLevel, CurrentSeasonRankPoint = _rankData.CurrentSeasonRankPoint, Wins = _rankData.Wins };
        }

        public async Task<int> GetRankPoint()
        {
            await GetDbUserRankData();
            return _rankData.CurrentSeasonRankPoint;
        }

        public uint GetWinsCount()
        {
            return _rankData.Wins;
        }

        private uint GetNextRankLevel()
        {
            if (_changeRankPoint <= 0)
                return _rankData.MaxRankLevel;

            uint nextRankLevel = _rankData.MaxRankLevel;
            if ((int)Data.GetConstant(DataConstantType.MAX_USER_RANK).Value != _rankData.MaxRankLevel)
            {
                if (IsFullRankPointForRankLevelUp())
                    nextRankLevel += 1;
            }
            return nextRankLevel;
        }

        private bool IsFullRankPointForRankLevelUp()
        {
            return _changeRankPoint + _rankData.CurrentSeasonRankPoint >=
                Data.GetUserRank(_rankData.MaxRankLevel).NeedRankPointForNextLevelUp;
        }

        private int GetNextRankPoint()
        {
            var nextPoint = _rankData.CurrentSeasonRankPoint + _changeRankPoint;
            return nextPoint < 0 ? 0 : nextPoint;
        }

        private int GetMaxRankPoint()
        {
            return Math.Max(_rankData.MaxRankPoint, _rankData.CurrentSeasonRankPoint + _changeRankPoint);
        }
    }
}
