using Npgsql;
using ProjectX.Core.DataAccess;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.DataAccess
{
    public sealed class PostgreSqlConnectionFactory : ISqlConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection() => GetNpgsqlConnection();

        public IDbConnection GetOpenedConnection()
        {
            var connection = GetConnection();
            
            connection.Open();

            return connection;
        }

        public async Task<IDbConnection> GetOpenedConnectionAsync(CancellationToken token = default)
        {
            var connection = GetNpgsqlConnection();
            
            await connection.OpenAsync(token).ConfigureAwait(false);

            return connection;
        }

        private NpgsqlConnection GetNpgsqlConnection() => new NpgsqlConnection(_connectionString);
    }
}
