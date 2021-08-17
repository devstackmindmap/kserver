using AkaDB.MySql;
using System.Threading.Tasks;
using System.Data.Common;
using System.Linq;
using AkaData;
using System.Collections.Generic;
using System.Text;
using Common.Quest;
using AkaEnum;

namespace Common.Pass
{
    public class SeasonPassManager
    {
        private DBContext _db;
        private uint _userId;
        private uint _currentSeason;

        public SeasonPassManager(uint userId, uint currentSeason, DBContext db)
        {
            _db = db;
            _userId = userId;
            _currentSeason = currentSeason;
        }

        public SeasonPassManager()
        {
        }

        public async Task Update()
        {
            var activeSeasonPassList = GetActivePassList();

            IEnumerable<uint> enablePassList = await GetEnablePassList();
            await UpdateSeasonPass(enablePassList, activeSeasonPassList);
        }

        public List<DataSeasonPass> GetActivePassList()
        {
            List<DataSeasonPass> currentSeasonPassList = null;
            if (_currentSeason != 0)
            {
                currentSeasonPassList = Data.GetSeasonPassListForSeason(_currentSeason);
            }
            return currentSeasonPassList == null ? new List<DataSeasonPass>() : currentSeasonPassList;
        }

        public async Task<IEnumerable<uint>> GetEnablePassList()
        {
            using (var cursor = await SelectEnableSeason())
            {
                if (cursor.Read())
                {
                    return GetEnablePassList(cursor);
                }
            }
            return Enumerable.Empty<uint>();
        }

        public IEnumerable<uint> GetEnablePassList(DbDataReader cursor)
        {
            var dbEnablePassList = (string)cursor["enablePassList"];
            return dbEnablePassList.Split('/')
                                    .Select(passport => uint.TryParse(passport, out var passId) ? passId : 0)
                                    .Where(passport => passport != 0);
        }

        private async Task UpdateSeasonPass(IEnumerable<uint> enablePassList, List<DataSeasonPass> activeSeasonPassList)
        {
            var endSeasonPassQuestGroupIdList = enablePassList.Except(activeSeasonPassList.Select(seasonPass => seasonPass.SeasonPassId))
                                                              .Select(seasonPassId => Data.GetSeasonPass(seasonPassId))
                                                              .Where(seasonPass => seasonPass != null)
                                                              //   .Select(seasonPass => seasonPass.QuestGroupId)
                                                              .ToList();
            await SendEndSeasonReward(endSeasonPassQuestGroupIdList);



            if (enablePassList.Any() == false || endSeasonPassQuestGroupIdList.Any())
            {
                var initTargetQuestGroupIdList = activeSeasonPassList.Select(seasonPass => seasonPass.QuestGroupId);
                await UpdateInitQuestForSeasonPass(initTargetQuestGroupIdList);
                await UpdateSeasonPassInfo(activeSeasonPassList);

                //     Console.WriteLine($"EndPassList{string.Join(".", endSeasonPassQuestGroupIdList.Select(pass=>pass.QuestGroupId))}  InitPassList{ string.Join(",", initTargetQuestGroupIdList)}");
            }
        }

        private async Task UpdateSeasonPassInfo(List<DataSeasonPass> activeSeasonPassList)
        {
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE user_info SET enablePassList = '0/")
                 .Append(string.Join("/", activeSeasonPassList.Where(seasonPass => seasonPass.SeasonPassType == AkaEnum.SeasonPassType.SeasonPass).Select(seasonPass => seasonPass.SeasonPassId)))
                 .Append("' WHERE userId = ")
                 .Append(_userId.ToString())
                 .Append(";");

            await _db.ExecuteNonQueryAsync(query.ToString());
        }

        private async Task SendEndSeasonReward(IEnumerable<DataSeasonPass> endSeasonPassQuestGroupIdList)
        {
            var firstSeasonPass = endSeasonPassQuestGroupIdList.FirstOrDefault();
            if (firstSeasonPass == null)
                return;

            await new QuestIO(_userId, firstSeasonPass.Season, _db).SendToMailAllCompletedRewards(endSeasonPassQuestGroupIdList);
        }

        private async Task UpdateInitQuestForSeasonPass(IEnumerable<uint> initTargetQuestGroupIdList)
        {
            if (initTargetQuestGroupIdList.Any() == false)
                return;

            await new QuestIO(_userId, _db).UpdateInitQuests(initTargetQuestGroupIdList);
        }

        public async Task<DbDataReader> SelectEnableSeason()
        {
            var cursor = await _db.ExecuteReaderAsync($"SELECT enablePassList FROM user_info WHERE userid = " + _userId.ToString() + ";");
            return cursor;
        }

        public async Task<List<uint>> GetBeforeAndCurrentPurchasedSeasonPassList()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT season FROM get_seasonpass " +
                "WHERE userId = ").Append(_userId).Append(" AND seasonPassType = ").Append((int)SeasonPassType.SeasonPassPremium)
                .Append(" AND season = ").Append(_currentSeason - 1)
                .Append(" UNION SELECT season FROM get_seasonpass " +
                "WHERE userId = ").Append(_userId).Append(" AND seasonPassType = ").Append((int)SeasonPassType.SeasonPassPremium)
                .Append(" AND season = ").Append(_currentSeason);

            var purchasedSeasons = new List<uint>();
            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    purchasedSeasons.Add((uint)cursor["season"]);
                }
            }
            return purchasedSeasons;
        }

        public async Task<bool> IsPurchasedCurrentSeasonPass()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT season FROM get_seasonpass " +
                "WHERE userId = ").Append(_userId).Append(" AND seasonPassType = ").Append((int)SeasonPassType.SeasonPassPremium)
                .Append(" AND season = ").Append(_currentSeason);

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                return cursor.Read();
            }
        }
    }
}
