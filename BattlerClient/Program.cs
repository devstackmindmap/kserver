using AkaConfig;
using AkaLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattlerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.InitConfig("Local");

            var runs = new List<Run>();

            for (int i = 0; i < 2000; i++)
            {
                var run = new Run();
                run.Connect();
                runs.Add(run);
            }

            for (int i = 0; i < 2000; i++)
            {
                ThreadPool.QueueUserWorkItem(runs[i].Execute2, i);
            }

            while (true)
            {
                System.Threading.Thread.Sleep(20);
            }
        }

        private static void fdsafads()
        {
            throw new NotImplementedException();
        }
    }
}
