using BattleServer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattleLogicTest
{
    public static class CompetitionTestJob
    {
        public static Object lockObject = new Object();
        public static Dictionary<int, int> dic = new Dictionary<int, int>();
        public static void Run(int userId, int number)
        {
            //lock (lockObject)
            //{
                if(dic.ContainsKey(userId))
                    dic[userId] = dic[userId] + 1;
                else
                    dic.Add(userId, 1);
                //AkaLogger.Logger.Instance().Info("UserID:{0}, Number:{1}", userId, number);
            //}
        }
    }

    [TestFixture]
    class CompetitionTest
    {
        [Test]
        public void RunCompetitionTestJob()
        {
            List<int> nums = new List<int>();
            for (int i = 0; i < 1100; ++i)
            {
                nums.Add(i);
            }

            //for (int i = 0; i < 1100; ++i)
            foreach(var num in nums)
            {
                Thread thread = new Thread(() => CompetitionTestJob.Run(num, num));
                thread.Start();
            }

            var until = DateTime.UtcNow.AddSeconds(15);
            while (until > DateTime.UtcNow)
            //while (true)
            {
                Thread.Sleep(2);
            }

            foreach (var item in CompetitionTestJob.dic)
            {
                AkaLogger.Logger.Instance().Info("UserID:{0}, Count:{1}", item.Key, item.Value);
            }
        }


        [Test]
        public void RoomManagerTest()
        {

        }
    }
}
