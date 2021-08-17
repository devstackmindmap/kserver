using AkaConfig;
using AkaData;
using AkaDB;
using AkaDB.MySql;
using AkaLogger;
using BattleLogic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AkaUtility;

namespace BattleLogicTest
{
    [TestFixture]
    public class NormalTest
    {
        [Test]
        public void RoundTestByInt()
        {
            for (int i = 0; i < 1000000; ++i)
            {
                var num = (int)(1.9);
                var num2 = (int)(1.4);
            }
        }

        [Test]
        public void RoundTestByTruncate()
        {
            for (int i = 0; i < 1000000; ++i)
            {
                var num = Math.Truncate(1.9);
                var num2 = Math.Truncate(1.4);
            }
        }

        [Test]
        public void RoundTest()
        {
            for (int i = 0; i < 100000; ++i)
            {
                float floatNum = 1.9f;
                int intNum = 20;

                //var num = Math.Round((floatNum * intNum);

                var num = Math.Round(1.9);
                var num2 = Math.Round(1.4);

                var num3 = Math.Ceiling(1.9);
                var num4 = Math.Ceiling(1.4);

                var num5 = Math.Truncate(1.9);
                var num6 = Math.Truncate(1.4);

                var num7 = (int)(1.9);
                var num8 = (int)(1.4);
            }
        }

        [Test]
        public void TimerTest()
        {
            AkaTimer.Timer timer = new AkaTimer.Timer(3000, false, Call);
            timer.Start();


            var start = DateTime.UtcNow;
            var now = DateTime.UtcNow;
            bool one = true;
            while (true)
            {
                if (one && start.AddMilliseconds(10000) < now)
                {
                    one = false;
                    timer.SetAttribute(1000, false);
                }
                now = DateTime.UtcNow;
                System.Threading.Thread.Sleep(20);
            }
        }

        private void Call()
        {
            Logger.Instance().Info("Go");
        }


        [Test]
        public void TimerTest2()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += Call2;
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Start();


            var start = DateTime.UtcNow;
            var now = DateTime.UtcNow;
            bool one = true;
            while (true)
            {
                if (one && start.AddMilliseconds(10000) < now)
                {
                    one = false;
                    timer.Interval = 1000;
                    //timer.AutoReset = false;
                }
                now = DateTime.UtcNow;
                System.Threading.Thread.Sleep(20);
            }
        }

        [Test]
        public void TimerTest3()
        {
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(20);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Logger.Instance().Info("Go3");
        }

        private void Call2(object sender, ElapsedEventArgs e)
        {
            Logger.Instance().Info("Go2");
        }

        [Test]
        public void LINQTest()
        {
            int[] array = { 1, -1, 2, -2, 3 };
            Logger.Instance().Info(array.Where(item => item < 0).ElementAt(1));
        }

        [Test]
        public void AsyncTest()
        {
            var a = CountDown();
            var b = CountDown();

            Task.WaitAll(a, b);
        }

        private static async Task CountDown()
        {
            for (int i = 9; i >= 0; i--)
            {
                Logger.Instance().Info(i);
                await Task.Delay(10);
            }
        }

        [Test]
        public void SimpleRateTest()
        {
            int rate = 30;
            int success = 0;
            int fail = 0;
            for (int i = 0; i < 100000; i++)
            {
                var result = AkaRandom.Random.Next(0, 100);
                
                if (result < rate)
                    success++;
                else
                    fail++;
            }
            Logger.Instance().Info("success:" + success + " fail:" + fail);
        }

        [Test]
        public void SimpleRateTestByUnitFunc()
        {
            int success = 0;
            int fail = 0;
            for (int i = 0; i < 100000; i++)
            {
                if (AkaRandom.Random.IsSuccess(30))
                    success++;
                else
                    fail++;
            }
            Logger.Instance().Info("success:" + success + " fail:" + fail);
        }

        [Test]
        public void DictionaryDuplicateKeyTest()
        {
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
            keyValuePairs.Add("a", 1);
            keyValuePairs.Add("a", 1);
        }


        [Test]
        public void QueueTest()
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            foreach(var item in queue)
            {

            }

            var re = queue.Dequeue();
        }
    }
}
