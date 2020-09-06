using System.Net.NetworkInformation;
using MongoDB.Bson;

namespace SharpBot.Modules.automod {
    public class AutoModModuleSettings : ModuleSettings {

        public bool BlockUrls { get; set; }
        public bool BlockInvites { get; set; }
        public bool BlockPings { get; set; }
        
        public AutoModModuleSettings(bool urls, bool invites, bool pings, bool enabled) :
            base(Module.WELCOMER, enabled) {

            BlockUrls = urls;
            BlockInvites = invites;
            BlockPings = pings;
        }

        public static AutoModModuleSettings NewDefault() {
            return new AutoModModuleSettings(false, true, false, true);
        }
        
        /// <summary>
        /// Serialize this <c>AutoModModuleSettings</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The serialized <c>BsonDocument</c></returns>
        public BsonDocument ToDocument() {
            return new BsonDocument {
                {"BlockUrls", BsonBoolean.Create(BlockUrls)},
                {"BlockInvites", BsonBoolean.Create(BlockInvites)},
                {"BlockPings", BsonBoolean.Create(BlockPings)},
                {"Enabled", BsonBoolean.Create(Enabled)}
            };
        }
        
        /// <summary>
        /// Deserialize the <c>BsonDocument</c> to it's normal <c>AutoModModuleSettings</c> form.
        /// </summary>
        /// <returns>The deserialized <c>AutoModModuleSettings</c></returns>
        public static AutoModModuleSettings FromDocument(BsonDocument document) {
            return new AutoModModuleSettings(
                document.GetValue("BlockUrls").AsBoolean,
                document.GetValue("BlockInvites").AsBoolean,
                document.GetValue("BlockPings").AsBoolean,
                document.GetValue("Enabled").AsBoolean);
        }
    }
}