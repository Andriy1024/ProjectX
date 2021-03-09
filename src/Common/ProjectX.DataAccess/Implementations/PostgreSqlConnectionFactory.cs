using Microsoft.Extensions.Options;
using Npgsql;
using ProjectX.Core.Setup;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.DataAccess
{
    public sealed class PostgreSqlConnectionFactory : ISqlConnectionFactory
    {
        readonly string _connectionString;

        public PostgreSqlConnectionFactory(IOptions<ConnectionStrings> options)
        {
            _connectionString = options.Value.DbConnection;
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
