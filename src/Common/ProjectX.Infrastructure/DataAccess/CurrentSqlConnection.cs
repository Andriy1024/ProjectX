using ProjectX.Core.DataAccess;
using System;
using System.Data;

namespace ProjectX.Infrastructure.DataAccess
{
    public sealed class CurrentSqlConnection : IDisposable
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        
        private IDbConnection _dbConnection;

        public CurrentSqlConnection(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IDbConnection GetConnection() 
        {
            if (_dbConnection == null) 
            {
                _dbConnection = _connectionFactory.GetConnection();
            }

            return _dbConnection;
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }
    }
}
