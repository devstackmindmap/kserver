
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AkaUtility;

namespace AkaThreading
{
    public class DisposableSemaphore : IDisposable
    {
        private SemaphoreSlim _semaphore;

        internal DisposableSemaphore(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            Release();
        }

        internal void WaitOnce()
        {
            _semaphore?.Wait();
        }

        internal async Task WaitOnceAsync()
        {
            if (_semaphore != null)
                await _semaphore.WaitAsync();
            else
                await Task.Yield();
        }

        public void Release()
        {
            _semaphore?.Release();
        }
    }


    public class SemaphoreManager 
    {
        private static Dictionary<SemaphoreType, SemaphoreSlim> _semaphores = new Dictionary<SemaphoreType, SemaphoreSlim>();
        
        public static void Add(SemaphoreType key, int initialCount, int maxCount)
        {
            if (initialCount > 0 && maxCount > 0)
            {
                _semaphores.Add(key, new SemaphoreSlim(initialCount, maxCount));
            }
        }

        public static DisposableSemaphore Lock(SemaphoreType key)
        {
            var lockObj = new DisposableSemaphore(_semaphores.SafeGet(key));
            lockObj.WaitOnce();
            return lockObj;
        }


        public static async Task<DisposableSemaphore> LockAsync(SemaphoreType key)
        {
            return await LockAsync(_semaphores.SafeGet(key));
        }

        public static async Task<DisposableSemaphore> LockAsync(SemaphoreSlim semaphore)
        {
            var lockObj = new DisposableSemaphore(semaphore);
            await lockObj.WaitOnceAsync();
            return lockObj;
        }

        public static int Count(SemaphoreType key)
        {
            return _semaphores.SafeGet(key)?.CurrentCount ?? -1;
        }
    }
}
