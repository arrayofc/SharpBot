using MongoDB.Bson;

namespace SharpBot.Modules.welcomer {
    public class WelcomerModuleSettings : ModuleSettings {

        public string WelcomeMessage { get; set; }
        public ulong WelcomeChannel { get; set; }

        public WelcomerModuleSettings(string message, ulong channel, bool enabled) :
            base(Module.WELCOMER, enabled) {

            WelcomeMessage = message;
            WelcomeChannel = channel;
        }

        public static WelcomerModuleSettings NewDefault() {
            return new WelcomerModuleSettings("Welcome {user} to the Discord Server!", 0, true);
        }
        
        /// <summary>
        /// Serialize this <c>WelcomerModuleSettings</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The serialized <c>BsonDocument</c></returns>
        public BsonDocument ToDocument() {
            return new BsonDocument {
                {"WelcomeMessage", BsonString.Create(WelcomeMessage)},
                {"WelcomeChannel", BsonInt64.Create(WelcomeChannel)},
                {"Enabled", BsonBoolean.Create(Enabled)}
            };
        }
        
        /// <summary>
        /// Deserialize the <c>BsonDocument</c> to it's normal <c>WelcomerModuleSettings</c> form.
        /// </summary>
        /// <returns>The deserialized <c>WelcomerModuleSettings</c></returns>
        public static WelcomerModuleSettings FromDocument(BsonDocument document) {
            return new WelcomerModuleSettings(
                document.GetValue("WelcomeMessage").AsString,
                (ulong) document.GetValue("WelcomeChannel").ToInt64(),
                document.GetValue("Enabled").AsBoolean);
        }
    }
}