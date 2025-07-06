using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Library.Interfaces;
using DynamicConfiguration.Library.Models;
using MongoDB.Driver;

namespace DynamicConfiguration.Library.Storage
{
    public class MongoConfigurationStore : IConfigurationStore
    {
        private readonly IMongoCollection<ConfigurationItem> _collection;

        public MongoConfigurationStore(string connectionString, string databaseName = "ConfigurationDb")
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);
            _collection = db.GetCollection<ConfigurationItem>("ConfigurationItems");
        }

        public async Task<List<ConfigurationItem>> GetActiveConfigs(string applicationName,
            CancellationToken cancellationToken = default)
        {
            var configurationItemsCursor = await _collection
                .FindAsync(x => x.ApplicationName == applicationName && x.IsActive,
                    cancellationToken: cancellationToken);

            return await configurationItemsCursor.ToListAsync(cancellationToken);
        }
        
    }
}