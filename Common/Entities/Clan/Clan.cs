using AkaDB.MySql;
using Common.Entities.Season;
using System.Text;
using System.Threading.Tasks;
using CommonProtocol;
using System.Collections.Generic;
using AkaEnum;
using System;

namespace Common
{
    public class Clan
    {
        protected DBContext _accountDb;
        protected uint _userId;
        protected StringBuilder _query = new StringBuilder();

        public Clan(uint userId, DBContext accountDb)
        {
            _userId = userId;
            _accountDb = accountDb;
            _query.Clear();
        }

        public async Task<ProtoClanInfo> GetClanInfo(string inviteCode = "")
        {
            _query.Clear();
            if (inviteCode == "")
            {
                _query.Append("SELECT clanId, countryCode, clanName, clanSymbolId, rankPoint, memberCount" +
                ", clanMasterUserId, inviteCode " +
                "FROM clans WHERE clanId = (SELECT clanId FROM clan_members WHERE userId = ")
                .Append(_userId).Append(");");
            }
            else
            {
                _query.Append("SELECT clanId, countryCode, clanName, clanSymbolId, rankPoint, memberCount" +
                ", clanMasterUserId, inviteCode " +
                "FROM clans WHERE inviteCode = '").Append(inviteCode).Append("' LIMIT 1;");
            }

            return await  ExecuteReaderAsync();
        }

        public async Task<ClanMemberGrade> GetClanMemberGrade()
        {
            _query.Clear();
            _query.Append("SELECT memberGrade FROM clan_members WHERE userId = ").Append(_userId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return ClanMemberGrade.Number4;

                var num = (int)cursor["memberGrade"];

                if (num >= 1 && num <= 4)
                    return (ClanMemberGrade)cursor["memberGrade"];

                return ClanMemberGrade.Number4;
            }
        }

        public async Task<ProtoClanInfo> GetClanInfo(uint clanId)
        {
            _query.Clear();
            _query.Append("SELECT clanId, countryCode, clanName, clanSymbolId, rankPoint, memberCount" +
            ", clanMasterUserId, inviteCode " +
            "FROM clans WHERE clanId = ").Append(clanId).Append(";");

            return await ExecuteReaderAsync();
        }

        private async Task<ProtoClanInfo> ExecuteReaderAsync()
        {
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return null;

                return new ProtoClanInfo
                {
                    ClanId = (uint)cursor["clanId"],
                    CountryCode = (string)cursor["countryCode"],
                    ClanName = (string)cursor["clanName"],
                    ClanSymbolId = (uint)cursor["clanSymbolId"],
                    RankPoint = (int)cursor["rankPoint"],
                    MemberCount = (int)cursor["memberCount"],
                    ClanMasterUserId = (uint)cursor["clanMasterUserId"],
                    InviteCode = (string)cursor["inviteCode"]
                };
            }
        }

        protected async Task<bool> IsInClan()
        {
            _query.Clear();
            _query.Append("SELECT userId FROM clan_members WHERE userId=").Append(_userId).Append(";");
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (cursor.Read())
                    return true;
                return false;
            }
        }

        protected async Task<(string countryCode, int rankPoint)> GetAccountInfo()
        {
            _query.Clear();
            _query.Append("SELECT currentSeason, currentSeasonRankPoint, nextSeasonRankPoint, countryCode FROM accounts WHERE userId=").Append(_userId).Append(";");
            int rankPoint;
            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return ("", 0);

                var countryCode = (string)cursor["countryCode"];
                rankPoint 
                    = await GetRankPoint((int)cursor["currentSeason"], (int)cursor["currentSeasonRankPoint"], 
                    (int)cursor["nextSeasonRankPoint"]);

                return (countryCode, rankPoint);                
            }
        }

        private async Task<int> GetRankPoint(int myCurrentSeason, int currentSeasonRankPoint, int nextSeasonRankPoint)
        {
            var serverSeason = await GetServerSeason();

            if (serverSeason == myCurrentSeason)
                return currentSeasonRankPoint;
            else if (serverSeason - 1 == myCurrentSeason)
                return nextSeasonRankPoint;
            else
                return 0;
        }

        private async Task<uint> GetServerSeason()
        {
            ServerSeason serverSeason = new ServerSeason(_accountDb);
            var seasonInfo = await serverSeason.GetKnightLeagueSeasonInfo();
            return seasonInfo.CurrentSeason;
        }

        public async Task<ProtoClanProfileAndMembers> GetClanProfileAndMembers(uint clanId)
        {
            if (clanId == 0)
                return null;

            return new ProtoClanProfileAndMembers
            {
                ClanProfile = await GetClanProfile(clanId),
                ClanMembers = await GetClanMembers(clanId)
            };
        }

        public async Task<ProtoClanProfile> GetClanProfile(uint clanId)
        {
            _query.Clear();
            _query.Append("SELECT clanPublicType, joinConditionRankPoint, clanName" +
                ", clanSymbolId, rankPoint, memberCount, clanExplain, countryCode FROM clans " +
                "WHERE clanId = ").Append(clanId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                if (false == cursor.Read())
                    return null;

                return new ProtoClanProfile
                {
                    ClanExplain = (string)cursor["ClanExplain"],
                    JoinConditionRankPoint = (int)cursor["joinConditionRankPoint"],
                    ClanName = (string)cursor["clanName"],
                    ClanSymbolId = (uint)cursor["clanSymbolId"],
                    RankPoint = (int)cursor["rankPoint"],
                    MemberCount = (int)cursor["memberCount"],
                    ClanId = clanId,
                    ClanPublicType = (ClanPublicType)cursor["clanPublicType"],
                    CountryCode = (string)cursor["countryCode"]
                };
            }
        }

        public async Task<List<uint>> GetClanMemberUserIds(uint clanId)
        {
            List<uint> members = new List<uint>();
            if (clanId != 0)
            {
                _query.Clear();
                _query.Append("SELECT userId FROM clan_members WHERE clanId =").Append(clanId).Append(";");

                using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
                {
                    while (cursor.Read())
                    {
                        var userId = (uint)cursor["userId"];
                        if (userId != _userId)
                            members.Add(userId);
                    }
                }
            }
            return members;
        }

        private async Task<List<ProtoClanMember>> GetClanMembers(uint clanId)
        {
            List<ProtoClanMember> members = new List<ProtoClanMember>();

            _query.Clear();
            _query.Append("SELECT a.memberGrade, b.userId, b.nickName, b.loginDateTime, " +
                "b.currentSeason, b.currentSeasonRankPoint, b.nextSeasonRankPoint, b.profileIconId " +
                "FROM clan_members a " +
                "LEFT OUTER JOIN accounts b ON b.userId = a.userId " +
                "WHERE a.clanId = ").Append(clanId).Append(";");

            using (var cursor = await _accountDb.ExecuteReaderAsync(_query.ToString()))
            {
                while(cursor.Read())
                {
                    members.Add(new ProtoClanMember
                    {
                        CurrentSeason = (int)cursor["currentSeason"],
                        CurrentSeasonRankPoint = (int)cursor["currentSeasonRankPoint"],
                        MemberGrade = (ClanMemberGrade)cursor["memberGrade"],
                        NextSeasonRankPoint = (int)cursor["nextSeasonRankPoint"],
                        Nickname = (string)cursor["nickname"],
                        ProfileIconId = (uint)cursor["profileIconId"],
                        RecentLoginDateTime = ((DateTime)cursor["loginDateTime"]).Ticks,
                        UserId = (uint)cursor["userId"]
                    });
                }
            }
            return members;
        }
    }
}
