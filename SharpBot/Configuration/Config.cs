// ReSharper disable MemberCanBePrivate.Global
namespace SharpBot.Configuration {
    /// <summary>
    /// The bot configuration.  
    /// </summary>
    public class Config {

        /// <summary>
        /// Token this bot runs on.
        /// </summary>
        public string BotToken { get; set; }
        
        /// <summary>
        /// Command prefix this bot listens for. 
        /// </summary>
        public char BotPrefix { get; set; }
        
        /// <summary>
        /// The MongoDB connection URI.
        /// </summary>
        public string MongoUri { get; set; }
        
        /// <summary>
        /// The MongoDB database.
        /// </summary>
        public string MongoDatabase { get; set; }
        
        public Config() {
            BotToken = "";
            BotPrefix = '!';
            MongoUri = "";
            MongoDatabase = "";
        }
    }
}
