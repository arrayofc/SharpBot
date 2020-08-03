using MongoDB.Driver;

namespace SharpBot.Database {
    public class Mongo {

        private MongoClient MongoClient { get; }
        public IMongoDatabase Database { get; }

        public Mongo(Sharp sharp) {
            MongoClient = new MongoClient(sharp.ConfigManager.GetConfig().MongoUri);
            Database = MongoClient.GetDatabase(sharp.ConfigManager.GetConfig().MongoDatabase);
        }
    }
}
