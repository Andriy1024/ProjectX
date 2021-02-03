using System;
using System.Threading;

namespace ProjectX.Common.Threading
{
    public readonly struct ReadLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLockSlim;

        public ReadLock(ReaderWriterLockSlim readerWriterLockSlim)
        {
            _readerWriterLockSlim = readerWriterLockSlim;
            _readerWriterLockSlim.EnterReadLock();
        }

        public void Dispose()
        {
            _readerWriterLockSlim.ExitReadLock();
        }
    }
}
