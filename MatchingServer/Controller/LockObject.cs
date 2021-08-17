using AkaThreading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatchingServer
{
    public static class LockObject
    {
        private static SemaphoreSlim _lockEnterRoom = new SemaphoreSlim(1, 1);
        private static SemaphoreSlim _lockMakeRoom = new SemaphoreSlim(1, 1);

        public static async Task<DisposableSemaphore> LockEnterRoomAsync()
        {
            return await SemaphoreManager.LockAsync(_lockEnterRoom);
        }

        public static async Task<DisposableSemaphore> LockMakeRoomAsync()
        {
            return await SemaphoreManager.LockAsync(_lockMakeRoom);
        }
    }
}
