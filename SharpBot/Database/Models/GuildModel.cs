using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SharpBot.Punishments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Database.Models {
    public class GuildModel {

        [BsonId]
        public ulong Id { get; }

        public GuildSettingsModel Settings { get; }

        public List<Punishment> Punishments { get; }

        public GuildModel(ulong id) :
            this(id, new GuildSettingsModel(), new List<Punishment>()) {
        }

        public GuildModel(ulong id, GuildSettingsModel settings, List<Punishment> punishments) {
            Id = id;
            Settings = settings;
            Punishments = punishments;
        }
        
        /// <summary>
        /// Save this <c>GuildModel</c> to the database async.
        /// </summary>
        /// <returns>The result of the update request.</returns>
        public async Task<UpdateResult> Save() {
            return await Sharp.GetInstance().Mongo.Database.GetCollection<GuildModel>("guilds").UpdateOneAsync(model => model.Id == Id,
                Builders<GuildModel>.Update
                    .Set(guild => guild.Id, Id)
                    .Set(guild => guild.Settings, Settings)
                    .Set(guild => guild.Punishments, Punishments),
                new UpdateOptions { IsUpsert = true});
        }
    }
}
