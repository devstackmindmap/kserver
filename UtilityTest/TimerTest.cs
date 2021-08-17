using AkaUtility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace UtilityTest
{
    class ThreadCheck
    {
        private Timer _timer = new Timer();
        private StringBuilder _log = new StringBuilder();
        private List<CustomTimer> _timers = new List<CustomTimer>();
        private int _index = 0;
        public ThreadCheck()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 1000;
            System.Threading.ThreadPool.GetMinThreads(out var workT, out var cpo);
            Console.WriteLine(workT + "   " + cpo);
            System.Threading.ThreadPool.SetMaxThreads(20, 4);
        }
        public void Run()
        {
            _timer.Start();
            while (true)
            {
                var command = Console.Read();
                if (command == 'q')
                    break;
                else if (command == 'x')
                {
                    foreach (var timer in _timers)
                    {
                        timer.Dispose();
                    }
                    _timers.Clear();
                }
                else if (command == 't')
                {
                    foreach (var timer in _timers)
                    {
                    }
                    _timers.Clear();
                }
                else if (command == 'v')
                {
                    GC.Collect();
                }
                else if (command == 'a')
                {
                    _timers.Add(new CustomTimer());
                }
            }
            foreach (var timer in _timers)
            {
                timer.Dispose();
            }





            _timer.Dispose();


            foreach( var timer in _timers)
            {
            }
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _log.Clear();
            var process = System.Diagnostics.Process.GetCurrentProcess();
            _log.Append("Threads:").Append(process.Threads.Count.ToString())
                .Append(", Timers:").Append(_timers.Count)
            //    .Append(", WorkingSet:").Append((process.WorkingSet64 / 1024).ToString())
            //    .Append(", WorPrivateMemkingSet:").Append((process.PrivateMemorySize64 / 1024).ToString())
                .Append(", HandleCount:").Append(process.HandleCount.ToString());
            Console.WriteLine(_log);
        }
    }
    class CustomTimer : IDisposable
    {
        private Timer _timer = new Timer();
        static int index = 0;
        int myindex = index++;
        public CustomTimer()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 1000;
            _timer.AutoReset = true;
            _timer.Start();
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"\t{myindex} in");
            var now = DateTime.Now;
            var checkTime = now.AddSeconds(10);
            while (false && true)
            {
                if (DateTime.Now > checkTime)
                    break;
                Task.Delay(100).Wait();
            }
            Console.WriteLine($"\t{myindex} Out");
        }
        private void Start()
        {
            _timer.Start();
        }
        public void Dispose()
        {
            _timer.Dispose();
        }
    }

}
