using MongoDB.Bson;

namespace SharpBot.Database.Models {
    public class GuildSettingsModel {

        public string CommandPrefix { get; set; }
        public ulong ModerationLogChannel { get; set; }
        
        public GuildSettingsModel() : this("!", 0) { }

        private GuildSettingsModel(string prefix, ulong modLog) {
            CommandPrefix = prefix;
            ModerationLogChannel = modLog;
        }

        /// <summary>
        /// Returns whether or not a moderation log channel has been
        /// registered for this <c>GuildSettingsModel</c>.
        /// </summary>
        public bool ModLogEnabled() {
            return ModerationLogChannel > 0;
        }
        
        /// <summary>
        /// Serialize this <c>GuildSettingsModel</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The serialized <c>BsonDocument</c></returns>
        public BsonDocument ToDocument() {
            return new BsonDocument {
                {"CommandPrefix", BsonString.Create(CommandPrefix)},
                {"ModerationLogChannel", BsonInt64.Create(ModerationLogChannel.ToString())},
            };
        }
        
        /// <summary>
        /// Deserialize the <c>BsonDocument</c> to its normal <c>GuildSettingsModel</c> form.
        /// </summary>
        /// <returns>The deserialized <c>GuildSettingsModel</c></returns>
        public static GuildSettingsModel FromDocument(BsonDocument document) {
            return new GuildSettingsModel(
                document.GetValue("CommandPrefix").AsString,
                (ulong) document.GetValue("ModerationLogChannel").AsInt64);
        }
    }
}
