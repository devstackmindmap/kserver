using AkaDB.MySql;
using CommonProtocol;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class UnitProfile
    {
        private DBContext _userDb;
        private uint _userId;
        private ProtoOnUnitProfile _unitProfile;

        public UnitProfile(DBContext userDb, uint userId)
        {
            _userDb = userDb;
            _userId = userId;
            _unitProfile = new ProtoOnUnitProfile();
        }

        public async Task<ProtoOnUnitProfile> GetUnitProfile()
        {
            var query = new StringBuilder();
            query.Append("SELECT id, level, maxRankLevel, currentSeasonRankPoint, nextSeasonRankPoint, skinId, maxVirtualRankLevel, currentVirtualRankLevel, currentVirtualRankPoint " +
                "FROM units WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    _unitProfile.UnitProfiles.Add(
                    new ProtoUnitProfile
                    {
                        UnitId = (uint)cursor["id"],
                        Level = (uint)cursor["level"],
                        MaxRankLevel = (uint)cursor["maxRankLevel"],
                        CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                        NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                        SkinId = (uint)cursor["skinId"],
                        MaxVirtualRankLevel = (uint)cursor["maxVirtualRankLevel"],
                        CurrentVirtualRankLevel = (uint)cursor["currentVirtualRankLevel"],
                        CurrentVirtualRankPoint = (int)cursor["currentVirtualRankPoint"],
                    });
                }
            }

            return _unitProfile;
        }
    }
}
