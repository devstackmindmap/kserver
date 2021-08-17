using AkaDB.MySql;
using AkaUtility;
using CommonProtocol;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controller.Clan
{
    public class WebClanSearch : BaseController
    {
        public override async Task<BaseProtocol> DoPipeline(BaseProtocol requestInfo)
        {
            var req = requestInfo as ProtoString;

            var slangFilter = new SlangFilter();
            if (slangFilter.IsInvalidWord(req.StringValue))
                return null;

            var query = new StringBuilder();
            query.Append("SELECT clanId, clanName, clanSymbolId, rankPoint, memberCount " +
                "FROM clans WHERE clanName like '").Append(req.StringValue).Append("%' LIMIT 50;");

            var res = new ProtoClanRecommend();
            using (var accountDb = new DBContext("AccountDBSetting"))
            {
                using (var cursor = await accountDb.ExecuteReaderAsync(query.ToString()))
                {
                    while (cursor.Read())
                    {
                        res.ProtoClanInfos.Add(new ProtoRecommendClanInfo
                        {
                            ClanId = (uint)cursor["clanId"],
                            ClanName = (string)cursor["clanName"],
                            ClanSymbolId = (uint)cursor["clanSymbolId"],
                            MemberCount = (int)cursor["memberCount"],
                            RankPoint = (int)cursor["rankPoint"]
                        });
                    }
                }
            }
            return res;
        }
    }
}
