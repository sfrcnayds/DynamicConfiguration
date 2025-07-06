using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DynamicConfiguration.Web.Configuration.Models
{
    public class ConfigurationItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")] public string Name { get; set; }

        [BsonElement("type")] public string Type { get; set; }

        [BsonElement("value")] public string Value { get; set; }

        [BsonElement("isActive")] public bool IsActive { get; set; }

        [BsonElement("applicationName")] public string ApplicationName { get; set; }
        
        [BsonElement("version")] public int Version { get; set; } = 1;
    }
}