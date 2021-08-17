using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaLogger;
using Common.Entities.Clan;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Battle
{
    public class RankClan
    {
        private DBContext _accountDb;
        private uint  _userId;
        private int _changeRankPoint;
        private uint? _clanId;

        public RankClan(DBContext accountDb, uint userId, int changeRankPoint)
        {
            _accountDb = accountDb;
            _changeRankPoint = changeRankPoint;
            _userId = userId;

            Init().Wait();
        }

        private async Task Init()
        {
            _clanId = await ClanManager.GetClanId(_accountDb, _userId);
            if (0 == _clanId)
                return;
        }

        public async Task ApplyRankForWin()
        {
            if (false == _clanId.HasValue)
                return;

            await UpdateClanRankInfo();
        }

        public async Task ApplyRankForLose()
        {
            if (false == _clanId.HasValue)
                return;

            await UpdateClanRankInfo();
        }

        private async Task UpdateClanRankInfo()
        {
            var query = new StringBuilder();

            query.Append("UPDATE clans SET rankPoint = rankPoint + ").Append(_changeRankPoint)
                .Append(" WHERE clanId=").Append(_clanId).Append(";");
            await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }
    }
}
