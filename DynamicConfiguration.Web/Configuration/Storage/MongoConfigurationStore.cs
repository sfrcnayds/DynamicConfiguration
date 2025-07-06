using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration.Web.Configuration.Interfaces;
using DynamicConfiguration.Web.Configuration.Models;
using MongoDB.Driver;

namespace DynamicConfiguration.Web.Configuration.Storage
{
    public class MongoConfigurationStore : IConfigurationStore
    {
        private readonly IMongoCollection<ConfigurationItem> _collection;

        public MongoConfigurationStore(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);
            _collection = db.GetCollection<ConfigurationItem>("ConfigurationItems");
        }

        public async Task<List<ConfigurationItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return (await _collection
                    .FindAsync(_ => true, cancellationToken: cancellationToken))
                .ToList(cancellationToken);
        }

        public async Task<ConfigurationItem> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return (await _collection
                    .FindAsync(item => item.Id == id,
                        cancellationToken: cancellationToken))
                .FirstOrDefault(cancellationToken);
        }


        public async Task AddAsync(ConfigurationItem item, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(item,
                cancellationToken: cancellationToken);
        }


        public async Task<bool> UpdateAsync(string id, ConfigurationItem newItem, CancellationToken cancellationToken = default)
        {
            var oldVersion = newItem.Version;
            newItem.Version   = oldVersion + 1;

            var filter = Builders<ConfigurationItem>.Filter.And(
                Builders<ConfigurationItem>.Filter.Eq(x => x.Id, id),
                Builders<ConfigurationItem>.Filter.Eq(x => x.Version, oldVersion)
            );
            
            var result = await _collection.ReplaceOneAsync(
                filter,
                newItem,
                cancellationToken: cancellationToken);
            
            return result.ModifiedCount == 1;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var existing = await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing is null)
                return false;

            var expectedVersion = existing.Version;
            
            var filter = Builders<ConfigurationItem>.Filter.And(
                Builders<ConfigurationItem>.Filter.Eq(x => x.Id, id),
                Builders<ConfigurationItem>.Filter.Eq(x => x.Version, expectedVersion)
            );

            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            
            return result.DeletedCount == 1;
        }
    }
}