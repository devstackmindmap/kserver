using AkaDB.MySql;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebLogic.User
{
    public class CardProfile
    {
        private DBContext _userDb;
        private uint _userId;
        private ProtoOnCardProfile _cardProfile;
        private List<uint> _cardIds;

        public CardProfile(DBContext userDb, uint userId, List<uint> cardIds)
        {
            _userDb = userDb;
            _userId = userId;
            _cardProfile = new ProtoOnCardProfile();
            _cardIds = cardIds;
        }

        public async Task<ProtoOnCardProfile> GetCardProfile()
        {
            var strCardIds = _cardIds.Any() ? string.Join(",", _cardIds) : "0";
            var query = new StringBuilder();
            query.Append("SELECT id, level FROM cards WHERE userId = ").Append(_userId)
                .Append(" AND id IN (").Append(strCardIds).Append(");");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                while (cursor.Read())
                {
                    _cardProfile.CardProfiles.Add(
                        new ProtoCardProfile
                        {
                            CardId = (uint)cursor["id"],
                            Level = (uint)cursor["level"]
                        });
                }
            }
            return _cardProfile;
        }
    }
}
