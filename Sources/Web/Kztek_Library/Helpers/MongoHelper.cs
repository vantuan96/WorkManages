using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Kztek_Library.Helpers
{
    public class MongoHelper
    {
        public static BsonDocument ConvertQueryStringToDocument(string query)
        {

            return BsonSerializer.Deserialize<BsonDocument>(query.ToString());

        }

        public static IMongoCollection<T> GetConnect<T>(string connect = "DefaultConnection")
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "connectstring.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            var _connectionString = root.GetSection("ConnectionStrings").GetSection(connect).Value;

            var mongoUrl = new MongoUrl(_connectionString);

            var _MongoClient = new MongoClient(mongoUrl.ToString().Replace(@"/" + mongoUrl.DatabaseName, ""));

            var _MongoDatabase = _MongoClient.GetDatabase(mongoUrl.DatabaseName);

            return _MongoDatabase.GetCollection<T>(typeof(T).Name);
        }
    }
}