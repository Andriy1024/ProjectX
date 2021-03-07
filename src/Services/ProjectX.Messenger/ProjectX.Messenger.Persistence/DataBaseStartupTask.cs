using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using ProjectX.Core;
using ProjectX.Core.Setup;
using ProjectX.Messenger.Application.Views;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Persistence
{
    public sealed class DataBaseStartupTask : IStartupTask
    {
        private readonly string _connectionString;
        private const string DbOwnerKey = "Username";
        private const string DbNameKey = "Database";
        private readonly IServiceProvider _serviceProvider;

        public DataBaseStartupTask(IOptions<ConnectionStrings> options, IServiceProvider serviceProvider)
        {
            _connectionString = options.Value.DbConnection ?? throw new ArgumentNullException(nameof(ConnectionStrings.DbConnection));
            _serviceProvider = serviceProvider;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            CreateDbIfNotExists();

            InitMarten();

            return Task.CompletedTask;
        }

        private void InitMarten() 
        {
            var documentSession = _serviceProvider.GetRequiredService<IDocumentSession>();

            documentSession.Query<ConversationView>().FirstOrDefault();
        }

        private void CreateDbIfNotExists() 
        {
            var splitedConnection = _connectionString.Split(';');

            var dbOwner = splitedConnection.FirstOrDefault(c => c.StartsWith(DbOwnerKey))?.Split('=').Last() ?? throw new ArgumentNullException($"Can't find {DbOwnerKey} in db connection string. {_connectionString}");

            var dbName = splitedConnection.FirstOrDefault(c => c.StartsWith(DbNameKey))?.Split('=').Last() ?? throw new ArgumentNullException($"Can't find {DbNameKey} in db connection string. {_connectionString}");

            var connectionString = string.Join(';', splitedConnection.Where(c => !c.TrimStart().StartsWith(DbNameKey, true, null)));

            using var connection = new NpgsqlConnection(connectionString);

            connection.Open();

            var checkSql = $"SELECT DATNAME FROM pg_catalog.pg_database WHERE DATNAME = '{dbName}'";

            var checkCommand = new NpgsqlCommand(checkSql, connection);

            var checkResult = checkCommand.ExecuteScalar();

            bool dbExist = checkResult?.ToString() == dbName;

            if (dbExist)
            {
                Console.WriteLine($"{dbName} already created.");
            }
            else
            {
                var createDbCommand = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\" ", connection);

                createDbCommand.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}
