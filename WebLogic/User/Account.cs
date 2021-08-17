using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using Common;
using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WebLogic.User
{
    public class Account
    {
        private DBContext _accountDb;
        private string _socialAccount;
        private string _nickName;

        public Account(DBContext accountDb, string socialAccount, string nickName)
        {
            _accountDb = accountDb;
            _socialAccount = socialAccount;
            _nickName = nickName;
        }

        public Account(DBContext accountDb, string socialAccount)
        {
            _accountDb = accountDb;
            _socialAccount = socialAccount;
        }

        public async Task<AccountInfo> GetAccountInfo(uint currentSeason, string countryCode)
        {
            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$socialAccount", _socialAccount),
                new InputArg("$nickName", _nickName),
                new InputArg("$joinDateTime", DateTime.UtcNow.ToTimeString()),
                new InputArg("$serverSeason", currentSeason),
                new InputArg("$countryCode", countryCode)
                );

            paramInfo.SetOutputParam(
                new OutputArg("$outUserId", MySqlDbType.UInt32),
                new OutputArg("$outInitDataVersion", MySqlDbType.UInt32),
                new OutputArg("$outNicknameDuplicate", MySqlDbType.Bit));

            using (var cursor = await _accountDb.CallStoredProcedureAsync(StoredProcedure.GET_ACCOUNT, paramInfo))
            {
                var isNicknameDuplicate = paramInfo.GetOutValue<bool>("$outNicknameDuplicate");
                if (isNicknameDuplicate)
                    return new AccountInfo { IsNicknameDuplicate = isNicknameDuplicate };

                return new AccountInfo
                {
                    UserId = paramInfo.GetOutValue<uint>("$outUserId"),
                    IsNicknameDuplicate = isNicknameDuplicate,
                    InitDataVersion = paramInfo.GetOutValue<uint>("$outInitDataVersion"),
                    ProfileIconId = 1
                };
            }
        }

        public async Task<AccountInfo> GetAccountInfo()
        {
            var query = new StringBuilder();
            query.Append("SELECT userId, nickName, initDataVersion, profileIconId, countryCode, limitLoginDate, limitLoginReason, wins " +
                "FROM accounts WHERE socialAccount ='")
                .Append(_socialAccount).Append("';");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return null;

                return new AccountInfo
                {
                    UserId = (uint)cursor["userId"],
                    InitDataVersion = (uint)cursor["initDataVersion"],
                    Nickname = (string)cursor["nickName"],
                    ProfileIconId = (uint)cursor["profileIconId"],
                    CountryCode = (string)cursor["countryCode"],
                    LimitLoginDateTime = (DateTime)cursor["limitLoginDate"],
                    LimitLoginReason = (string)cursor["limitLoginReason"],
                    Wins = (uint)cursor["wins"],
                };
            }
        }

        public async Task<(string countryCode, int groupCode)> GetCountryInfo(string ip)
        {
            var ipInfo = Dot2LongIP(ip);

            var query = new StringBuilder();
            if (ipInfo.Ipv4ORIpv6 == Ipv4ORIpv6.Ipv4)
            {
                query.Append("SELECT country_code, group_code FROM _ip2location " +
                    "WHERE ip_from <= ").Append(ipInfo.ipnum)
                    .Append(" AND ip_to >= ").Append(ipInfo.ipnum).Append(";");
            }
            else
            {
                query.Append("SELECT country_code, group_code FROM _ip2location_ipv6 " +
                    "WHERE ip_from <= '").Append(ipInfo.ipnum)
                    .Append("' AND ip_to >= '").Append(ipInfo.ipnum).Append("';");
            }

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (false == cursor.Read())
                    return ("KR", 1);

                return (
                    (string)cursor["country_code"],
                    (int)cursor["group_code"]
                    );
            }
        }

        private (System.Numerics.BigInteger ipnum, Ipv4ORIpv6 Ipv4ORIpv6) Dot2LongIP(string ip)
        {
            var ipv4ORIpv6 = Ipv4ORIpv6.Ipv4;

            System.Net.IPAddress address;
            System.Numerics.BigInteger ipnum;

            if (System.Net.IPAddress.TryParse(ip, out address))
            {
                byte[] addrBytes = address.GetAddressBytes();

                if (System.BitConverter.IsLittleEndian)
                {
                    System.Collections.Generic.List<byte> byteList = new System.Collections.Generic.List<byte>(addrBytes);
                    byteList.Reverse();
                    addrBytes = byteList.ToArray();
                }

                if (addrBytes.Length > 8)
                {
                    //IPv6
                    ipnum = System.BitConverter.ToUInt64(addrBytes, 8);
                    ipnum <<= 64;
                    ipnum += System.BitConverter.ToUInt64(addrBytes, 0);

                    ipv4ORIpv6 = Ipv4ORIpv6.Ipv6;
                }
                else
                {
                    //IPv4
                    ipnum = System.BitConverter.ToUInt32(addrBytes, 0);

                    ipv4ORIpv6 = Ipv4ORIpv6.Ipv4;
                }
                return (ipnum, ipv4ORIpv6);
            }
            return (0, ipv4ORIpv6);
        }
    }
}
