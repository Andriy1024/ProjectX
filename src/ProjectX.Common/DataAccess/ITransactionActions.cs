﻿using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace ProjectX.Common.DataAccess
{
    public interface ITransactionActions
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync();
        IDbContextTransaction GetCurrentTransaction();
        bool HasActiveTransaction { get; }
    }
}