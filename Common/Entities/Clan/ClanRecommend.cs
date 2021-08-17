using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common
{
    public class ClanRecommend : Clan
    {
        public ClanRecommend(uint userId, DBContext accountDb) : base(userId, accountDb)
        {
        }

        public async Task<ProtoClanRecommend> GetRecommendClans()
        {
            ProtoClanRecommend recommendClan = new ProtoClanRecommend();

            var accountInfo = await GetAccountInfo();
            var minMaxClanId = await GetMinMaxClanId(accountInfo);
            if (minMaxClanId.minClanId == 0)
                return recommendClan;

            var randClanId = AkaRandom.Random.NextUint(minMaxClanId.minClanId, minMaxClanId.maxClanId + 1);

            await GetDBClans(accountInfo, randClanId, recommendClan);

            return recommendClan;
        }

        private async Task<(uint minClanId, uint maxClanId)> GetMinMaxClanId((string countryCode, int rankPoint) accountInfo)
        {
            _query.Clear();
            _query.Append("SELECT MIN(clanId) AS minClanId, MAX(clanId) AS maxClanId " +
                "FROM clans WHERE countryCode = '").Append(accountInfo.countryCode).Append("'")
                .Append(" AND clanPublicType = 1 AND joinConditionRankPoint <= ").Append(accountInfo.rankPoint).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return (0, 0);

                if (cursor.IsDBNull(0))
                    return (0, 0);

                return ((uint)cursor["minClanId"], (uint)cursor["maxClanId"]);
            }
        }

        private async Task GetDBClans((string countryCode, int rankPoint) accountInfo, uint baseClanId, 
            ProtoClanRecommend recommendClans)
        {
            var maxClanUserCount = (int)AkaData.Data.GetConstant(DataConstantType.CLAN_MAX_USER).Value;
            _query.Clear();
            _query.Append("(SELECT clanId, clanName, clanSymbolId, rankPoint, memberCount " +
                "FROM clans WHERE countryCode = '").Append(accountInfo.countryCode).Append("'")
                .Append(" AND clanPublicType = 1 AND joinConditionRankPoint <= ").Append(accountInfo.rankPoint)
                .Append(" AND clanId <= ").Append(baseClanId).Append(" AND memberCount < ").Append(maxClanUserCount)
                .Append(" ORDER BY clanId DESC LIMIT 10) UNION ");
            _query.Append("(SELECT clanId, clanName, clanSymbolId, rankPoint, memberCount " +
                "FROM clans WHERE countryCode = '").Append(accountInfo.countryCode).Append("'")
                .Append(" AND clanPublicType = 1 AND joinConditionRankPoint <= ").Append(accountInfo.rankPoint)
                .Append(" AND clanId >= ").Append(baseClanId + 1).Append(" AND memberCount < ").Append(maxClanUserCount)
                .Append(" ORDER BY clanId ASC LIMIT 10);");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while (cursor.Read())
                {
                    recommendClans.ProtoClanInfos.Add(new ProtoRecommendClanInfo
                    {
                        ClanId = (uint)cursor["clanId"],
                        ClanName = (string)cursor["clanName"],
                        ClanSymbolId = (uint)cursor["clanSymbolId"],
                        RankPoint = (int)cursor["rankPoint"],
                        MemberCount = (int)cursor["memberCount"]
                    });
                }
            }
        }
    }
}
