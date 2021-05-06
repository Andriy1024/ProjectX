using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectX.Core;
using ProjectX.FileStorage.Persistence.Database.Setup;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Persistence.Database
{
    public sealed class MongoStartupTask : IStartupTask
    {
        private readonly IMongoDatabase _db;
        
        private readonly MongoOptions _options;

        private readonly ILogger<MongoStartupTask> _logger;

        public MongoStartupTask(IMongoDatabase db, 
            IOptions<MongoOptions> options,
            ILogger<MongoStartupTask> logger)
        {
            _db = db;
            _options = options.Value;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var databases = await _db.Client.ListDatabasesAsync();

            foreach (var database in await databases.ToListAsync())
            {
                _logger.LogInformation($"Databases: {database}"); 
            }

            //var collections = await _db.ListCollectionsAsync();

            //foreach (var collection in await collections.ToListAsync())
            //{
            //    _logger.LogInformation($"Collections: {string.Join(' ', collection.Names)}");
            //}

            if (_options.Collections != null && _options.Collections.Length != 0) 
            {
                foreach (var collection in _options.Collections) 
                {
                    var filter = new BsonDocument("name", collection);
                    
                    //filter by collection name
                    var collections = await _db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
                    
                    if(await collections.AnyAsync())
                    {
                        continue;
                    }

                    _logger.LogInformation($"Create collection: {collection}");
                    _db.CreateCollection(collection);
                }
            }
        }
    }
}
