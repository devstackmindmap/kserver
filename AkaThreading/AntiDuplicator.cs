
using System;
using System.IO;
using System.Threading;

namespace AkaThreading
{
    public class AntiDuplicator
    {
        private static Mutex _mutex;

        public static void AppRunning(int port = 0)
        {
            var myModule = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
            _mutex = new Mutex(true, myModule + port, out var createdNew);
            if (createdNew == false)
            {
                Console.WriteLine(myModule + "is already running. so this app will exit.");
                Environment.Exit(0);
            }
            _mutex.ReleaseMutex();
        }
    }
}