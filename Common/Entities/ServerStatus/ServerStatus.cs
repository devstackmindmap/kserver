using AkaEnum;
using AkaRedisLogic;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Entities.ServerStatus
{
    public class ServerStatus
    {
        public static async Task<bool> IsServerDown(IDatabase redis)
        {
            var redisValue = await redis.StringGetAsync(RedisKeyType.SIsServerDown.ToString());

            if (false == redisValue.HasValue)
                return false;

            try
            {
                return JsonConvert.DeserializeObject<string>(redisValue) == "1";
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<bool> IsDeveloperIp(IDatabase redis, string ip)
        {
            var redisValue = await redis.StringGetAsync(RedisKeyType.SDeveloperList.ToString());

            if (false == redisValue.HasValue)
                return false;

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(redisValue).Contains(ip);
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static  async Task<string> GetServerStatusInfo(IDatabase redis, RedisKeyType serverStateRedisKeyType, int groupCode)
        {
            var result = await ServerStatusInfoJob.GetServerStateInfo(redis, serverStateRedisKeyType, groupCode);

            if (result.Length > 0)
            {
                string ip = result[0].Element;
                int score = (int)result[0].Score;

                //if (((byte)ServerStateType.Running + 1) * ConstValue.SERVERINFO_ORDERING_MULTIPLE_NUM > score)
                return ip;
            }
            return "";
        }
    }
}
