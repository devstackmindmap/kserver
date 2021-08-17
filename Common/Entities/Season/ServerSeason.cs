using AkaData;
using AkaDB.MySql;
using AkaEnum;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.Season
{
    public class CommonValueData
    {
        public uint CommonValue;
        public uint CommonValue2;
    }

    public class ServerSeason
    {
        private DBContext _accountDb;

        public ServerSeason(DBContext db)
        {
            _accountDb = db;
        }

        public async Task<ServerSeasonInfo> GetKnightLeagueSeasonInfo()
        {
            return await GetCommonSeasonInfo((int)ServerCommonTable.KnightLeagueSeason);
        }

        public async Task<ServerSeasonInfo> GetSeasonPassInfo()
        {
            return await GetCommonSeasonInfo((int)ServerCommonTable.SeasonPass);
        }

        public async Task<ServerSeasonInfo> GetChallengeSeasonInfo()
        {
            return await GetCommonSeasonInfo((int)ServerCommonTable.ChallengeSeason);
        }

        private async Task<ServerSeasonInfo> GetCommonSeasonInfo(int commonId)
        {
            var commonTableSeasonInfo = await GetCommonSeasonData(commonId);

            var serverSeasonInfo = new ServerSeasonInfo();

            if (DateTime.UtcNow >= commonTableSeasonInfo.CurrentSeasonStartDateTime)
            {
                serverSeasonInfo.CurrentSeason = commonTableSeasonInfo.CurrentSeason;
                serverSeasonInfo.NextSeasonStartDateTime = commonTableSeasonInfo.NextSeasonStartDateTime;
                serverSeasonInfo.CurrentSeasonStartDateTime = commonTableSeasonInfo.CurrentSeasonStartDateTime;
            }
            else
            {
                serverSeasonInfo.CurrentSeason = commonTableSeasonInfo.CurrentSeason - 1;
                serverSeasonInfo.NextSeasonStartDateTime = commonTableSeasonInfo.CurrentSeasonStartDateTime;
                serverSeasonInfo.CurrentSeasonStartDateTime = commonTableSeasonInfo.BeforeSeasonEndDateTime;
            }

            return serverSeasonInfo;
        }

        public async Task<ServerSeasonInfoWithSeasonYearNum> GetKnightLeagbueSeasonInfoWithSeasonYearNum()
        {
            var commonTableSeasonInfo = await GetCommonSeasonData((int)ServerCommonTable.KnightLeagueSeason);

            var serverSeasonInfo = new ServerSeasonInfoWithSeasonYearNum();

            if (DateTime.UtcNow >= commonTableSeasonInfo.CurrentSeasonStartDateTime)
            {
                serverSeasonInfo.CurrentSeason = commonTableSeasonInfo.CurrentSeason;
                serverSeasonInfo.NextSeasonStartDateTime = commonTableSeasonInfo.NextSeasonStartDateTime;
                serverSeasonInfo.CurrentSeasonStartDateTime = commonTableSeasonInfo.CurrentSeasonStartDateTime;
            }
            else
            {
                serverSeasonInfo.CurrentSeason = commonTableSeasonInfo.CurrentSeason - 1;
                serverSeasonInfo.NextSeasonStartDateTime = commonTableSeasonInfo.CurrentSeasonStartDateTime;
                serverSeasonInfo.CurrentSeasonStartDateTime = commonTableSeasonInfo.BeforeSeasonEndDateTime;
            }

            if (commonTableSeasonInfo.BaseCurrentSeason == serverSeasonInfo.CurrentSeason)
            {
                serverSeasonInfo.SeasonYear = commonTableSeasonInfo.SeasonYear;
                serverSeasonInfo.SeasonYearNum = commonTableSeasonInfo.SeasonYearNum;
            }
            else
            {
                serverSeasonInfo.SeasonYear = commonTableSeasonInfo.BeforeSeasonYear;
                serverSeasonInfo.SeasonYearNum = commonTableSeasonInfo.BeforeSeasonYearNum;
            }

            return serverSeasonInfo;
        }

        private async Task<(uint CurrentSeason,
            int SeasonYear,
            uint SeasonYearNum,
            uint BaseCurrentSeason,
            int BeforeSeasonYear,
            uint BeforeSeasonYearNum,
            DateTime BeforeSeasonEndDateTime,
            DateTime CurrentSeasonStartDateTime,
            DateTime NextSeasonStartDateTime)>
            GetCommonSeasonData(int commonId)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT commonValue, commonValue2, commonValue3, commonValue4, commonValue5, commonValue6, ")
                .Append("commonBeforeStartDateTime, commonStartDateTime, commonNextStartDateTime FROM _common WHERE commonId = ")
                .Append(commonId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Check the _common table");

                return (
                    (uint)cursor["commonValue"],
                    (int)(uint)cursor["commonValue2"],
                    (uint)cursor["commonValue3"],
                    (uint)cursor["commonValue4"],
                    (int)cursor["commonValue5"],
                    (uint)cursor["commonValue6"],
                    (DateTime)cursor["commonBeforeStartDateTime"],
                    (DateTime)cursor["commonStartDateTime"],
                    (DateTime)cursor["commonNextStartDateTime"]
                    );
            }
        }

        public async Task<CommonValueData> GetCommonValueAll(ServerCommonTable serverCommonTable)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT commonValue, commonValue2 FROM _common WHERE commonId = ")
                .Append((int)serverCommonTable).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Check the _common table");

                return new CommonValueData
                {
                    CommonValue = (uint)cursor["commonValue"],
                    CommonValue2 = (uint)cursor["commonValue2"]
                };
            }
        }

        public async Task<uint> GetCommonValue1(ServerCommonTable serverCommonTable)
        {
            var values = await GetCommonValueAll(serverCommonTable);
            return values.CommonValue;
        }

        public static async Task<(uint currentSeason, int currentSeasonInterval)> GetCurrentSeasonPassInfo()
        {
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                var serverSeason = new ServerSeason(accountDb);
                var seasonInfo = await serverSeason.GetSeasonPassInfo();
                var currentSeasonInterval = (int)(DateTime.UtcNow - seasonInfo.CurrentSeasonStartDateTime).TotalDays + 1;
                var currentSeason = seasonInfo.CurrentSeason;
                return (currentSeason, currentSeasonInterval);
            }
        }
    }
}
