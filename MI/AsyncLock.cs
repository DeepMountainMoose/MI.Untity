using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MI
{
    public class AsyncLock : SemaphoreSlim
    {
        private readonly Task<IDisposable> _releaserTask;
        private readonly IDisposable _releaser;
        public AsyncLock() : this(1) { }

        /// <param name="parallelCount">并行数量，默认1，最少为1</param>
        public AsyncLock(int parallelCount) : base(parallelCount, parallelCount)
        {
            if (parallelCount < 1) throw new ArgumentOutOfRangeException(nameof(parallelCount), parallelCount, "不能小于1");

            _releaser = new Releaser(this);
            _releaserTask = Task.FromResult(_releaser);
        }

        public IDisposable Lock() => Lock(new CancellationToken());

        public IDisposable Lock(CancellationToken cancellationToken)
        {
            Wait(cancellationToken);

            return _releaser;
        }

        public Task<IDisposable> LockAsync() => LockAsync(new CancellationToken());

        public Task<IDisposable> LockAsync(CancellationToken cancellationToken)
        {
            var waitTask = WaitAsync(cancellationToken);

            return waitTask.IsCompleted
                ? _releaserTask
                : waitTask.ContinueWith(
                    (_, releaser) => (IDisposable)releaser,
                    _releaser,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        private class Releaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            public Releaser(SemaphoreSlim semaphore) => _semaphore = semaphore;
            public void Dispose() => _semaphore.Release();
        }
    }
}
