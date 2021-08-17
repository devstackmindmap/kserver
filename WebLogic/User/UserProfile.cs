using AkaDB.MySql;
using AkaEnum;
using Common;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class UserProfile
    {
        private DBContext _accountDb;
        private DBContext _userDb;
        private uint _userId;
        private ProtoUserProfile _userProfile;

        public UserProfile(DBContext accountDb, DBContext userDb, uint userId)
        {
            _accountDb = accountDb;
            _userDb = userDb;
            _userId = userId;
            _userProfile = new ProtoUserProfile();
        }

        public async Task<ProtoUserProfile> GetUserProfile()
        {
            await GetAccount();
            await GetDeck();
            await GetUserLevelExp();
            await GetAdditionalUserInfo();
            return _userProfile;
        }

        public async Task GetAccount()
        {
            var query = new StringBuilder();

            query.Append("SELECT currentSeason, maxRankLevel, currentSeasonRankPoint, nextSeasonRankPoint, maxRankPoint," +
                " rankVictoryCount, profileIconId, b.clanId, b.memberGrade, c.clanName, c.clanSymbolId" +
                " FROM accounts a" +
                " LEFT OUTER JOIN clan_members b ON b.userId = a.userId" +
                " LEFT OUTER JOIN clans c ON c.clanId = b.clanId" +
                " WHERE a.userId = ").Append(_userId).Append("; ");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Invalid UserId, GetProfile");

                _userProfile.CurrentSeason = (int)cursor["currentSeason"];
                _userProfile.MaxRankLevel = (uint)cursor["maxRankLevel"];
                _userProfile.CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"];
                _userProfile.NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"];
                _userProfile.MaxRankPoint = (int)cursor["maxRankPoint"];
                _userProfile.RankVictoryCount = (int)cursor["rankVictoryCount"];
                _userProfile.ProfileIconId = (uint)cursor["profileIconId"];
                _userProfile.ClanId = cursor.IsDBNull(cursor.GetOrdinal("clanId")) ? 0 : (uint)cursor["clanId"];
                _userProfile.ClanMemberGrade = cursor.IsDBNull(cursor.GetOrdinal("memberGrade")) ? ClanMemberGrade.Number4 : (ClanMemberGrade)cursor["memberGrade"];
                _userProfile.ClanName = cursor.IsDBNull(cursor.GetOrdinal("clanName")) ? "" : (string)cursor["clanName"];
                _userProfile.ClanSymbolId = cursor.IsDBNull(cursor.GetOrdinal("clanSymbolId")) ? 0 : (uint)cursor["clanSymbolId"];
            }
        }

        public async Task GetDeck()
        {
            var deck = new Deck.Deck(_userDb, _userId);
            _userProfile.ProtoOnGetRecentDeck = await deck.GetRecentDeck(ModeType.RecentKnightLeagueDeck, 0);
        }

        private async Task GetUserLevelExp()
        {
            var query = new StringBuilder();
            query.Append("SELECT level, exp FROM users WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    throw new Exception("Invalid UserId, GetUser");

                _userProfile.Level = (uint)cursor["level"];
                _userProfile.Exp = (ulong)cursor["exp"];
            }
        }

        private async Task GetAdditionalUserInfo()
        {
            var query = new StringBuilder();
            query.Append("SELECT maxVirtualRankPoint, challengeClearCount FROM user_info WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _userDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return;

                _userProfile.MaxVirtualRankPoint = (int)cursor["maxVirtualRankPoint"];
                _userProfile.ChallengeClearCount = (int)cursor["challengeClearCount"];
            }
        }
    }
}
