using AkaDB.MySql;
using CommonProtocol;
using System.Threading.Tasks;

namespace Common.Entities.Unit
{
    public class DBGetUnitInfo
    {
        public string StrUserId;
        public string StrUnitId;

        public async Task<uint> GetLevel(DBContext db)
        {
            using (var cursor = await db.ExecuteReaderAsync($"SELECT level FROM units WHERE userId = {StrUserId} AND id = {StrUnitId};"))
            {
                if (false == cursor.Read())
                    throw new System.Exception();

                var unitLevel = (uint)cursor["level"];
                return unitLevel;

            }
        }

        public async Task GetInfos(DBContext db, ProtoUnitInfoForRecentDeck protoUnitInfoForRecentDeck)
        {
            using (var cursor = await db.ExecuteReaderAsync($"SELECT id, level, maxRankLevel, currentSeasonRankPoint, nextSeasonRankPoint, maxVirtualRankLevel, currentVirtualRankLevel, currentVirtualRankPoint, skinId " +
                $"FROM units " +
                $"WHERE userId = {StrUserId} AND id = {StrUnitId};"))
            {
                if (false == cursor.Read())
                    throw new System.Exception();

                protoUnitInfoForRecentDeck.UnitId = (uint)cursor["id"];
                protoUnitInfoForRecentDeck.Level = (uint)cursor["level"];
                protoUnitInfoForRecentDeck.MaxRankLevel = (uint)cursor["maxRankLevel"];
                protoUnitInfoForRecentDeck.CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"];
                protoUnitInfoForRecentDeck.NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"];
                protoUnitInfoForRecentDeck.MaxVirtualRankLevel = (uint)cursor["maxVirtualRankLevel"];
                protoUnitInfoForRecentDeck.CurrentVirtualRankLevel = (uint)cursor["currentVirtualRankLevel"];
                protoUnitInfoForRecentDeck.CurrentVirtualRankPoint = (int)cursor["currentVirtualRankPoint"];
                protoUnitInfoForRecentDeck.SkinId = (uint)cursor["skinId"];
            }
        }
    }
}
