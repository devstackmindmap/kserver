using AkaUtility;
using System;

namespace WebClient
{
    class Test
    {
        public static void Run()
        {
            for(int i = 0; i < 100; i++)
            {
                Console.WriteLine(AkaRandom.Random.NextUint(UInt32.MinValue, UInt32.MaxValue));
            }
        }
    }
}
