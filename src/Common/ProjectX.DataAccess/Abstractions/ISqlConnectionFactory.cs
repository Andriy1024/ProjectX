﻿using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.DataAccess
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetConnection();
        IDbConnection GetOpenedConnection();
        Task<IDbConnection> GetOpenedConnectionAsync(CancellationToken token);
    }
}
