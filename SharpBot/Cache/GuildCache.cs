using SharpBot.Database.Models;
using System;
using System.Runtime.Caching;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SharpBot.Cache {
    public class GuildCache {
        private static readonly MemoryCache Cache = new MemoryCache("GuildCache");

        /// <summary>
        /// Get the <c>GuildModel</c> object for said guild ID.
        /// </summary>
        /// <param name="id">The guild id.</param>
        /// <returns>The representing <c>GuildModel</c>> for said guild id.</returns>
        public static GuildModel GetGuild(ulong id) {
            return AddOrGetExisting(id.ToString(), () =>  InitializeItem(id.ToString()));
        }

        /// <summary>
        /// Returns or receives the <c>GuildModel</c> inside of the memory cache.
        /// </summary>
        /// <param name="id">The guild id.</param>
        /// <param name="valueFactory">Function to insert new value.</param>
        /// <returns></returns>
        private static GuildModel AddOrGetExisting(string id, Func<GuildModel> valueFactory) {
            var newValue = new Lazy<GuildModel>(valueFactory);
            var oldValue = Cache.AddOrGetExisting(id, newValue, new CacheItemPolicy()) as Lazy<GuildModel>;

            try {
                return (oldValue ?? newValue).Value;
            } catch {
                Cache.Remove(id);
                throw;
            }
        }

        /// <summary>
        /// Initializes a <c>GuildModel</c> object to the cache. Attempts to find one
        /// in the database, and if none was found, it creates a new object.
        /// </summary>
        /// <param name="id">The guild id.</param>
        /// <returns>The fetched or created <c>GuildModel</c>.</returns>
        private static GuildModel InitializeItem(string id) {
            var document = Sharp.GetInstance().Mongo.Database.GetCollection<BsonDocument>("guilds")
                .Find(new BsonDocument("Id", id)).Limit(1).FirstOrDefault();

            GuildModel guildModel;

            if (document == null) {
                guildModel = new GuildModel(ulong.Parse(id));
                guildModel.Save().ContinueWith(task => {
                    if (!task.Result.IsAcknowledged) {
                        Console.WriteLine($"WARN: Could not save {id} to database.");
                    }
                });
                
            } else {
                guildModel = GuildModel.FromDocument(document);
            }

            return guildModel;
        }
    }
}