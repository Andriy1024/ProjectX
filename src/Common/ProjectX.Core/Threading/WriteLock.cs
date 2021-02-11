using System;
using System.Threading;

namespace ProjectX.Core.Threading
{
    public readonly struct WriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLockSlim;

        public WriteLock(ReaderWriterLockSlim readerWriterLockSlim)
        {
            _readerWriterLockSlim = readerWriterLockSlim;
            _readerWriterLockSlim.EnterWriteLock();
        }

        public void Dispose()
        {
            _readerWriterLockSlim.ExitWriteLock();
        }
    }
}
