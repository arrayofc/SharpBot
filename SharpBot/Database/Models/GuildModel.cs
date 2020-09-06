using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SharpBot.Modules;
using SharpBot.Modules.welcomer;

namespace SharpBot.Database.Models {
    public class GuildModel {
        public ulong Id { get; }
        public GuildSettingsModel Settings { get; }
        public List<GuildUserModel> Users { get; }

        public Dictionary<Module, ModuleSettings> Modules { get; }

        public GuildModel(ulong id) {
            Id = id;
            Settings = new GuildSettingsModel();
            Users = new List<GuildUserModel>();
            Modules = new Dictionary<Module, ModuleSettings>();
        }

        private GuildModel(ulong id, GuildSettingsModel settings, List<GuildUserModel> users,
            Dictionary<Module, ModuleSettings> modules) {
            Id = id;
            Settings = settings;
            Users = users;
            Modules = modules;
        }

        /// <summary>
        /// Add a user to the user list.
        /// </summary>
        /// <param name="user">User to add</param>
        public GuildUserModel AddUser(GuildUserModel user) {
            Users.Add(user);
            return user;
        }

        /// <summary>
        /// Remove a user from the user list.
        /// </summary>
        /// <param name="user">User to remove</param>
        public void RemoveUser(GuildUserModel user) {
            Users.Remove(user);
        }

        /// <summary>
        /// Retrieve a <c>GuildUserModel</c> by user id. 
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The retrieved user model, or a
        /// newly created one if none was found.</returns>
        public GuildUserModel GetUser(ulong id) {
            var user = Users.FirstOrDefault(userModel => userModel.Id == id);

            if (user != null) return user;
            {
                var model = AddUser(new GuildUserModel(id));

                Save().ContinueWith(task => {
                    if (!task.Result.IsAcknowledged) Console.WriteLine($"WARN: Could not update guild model {Id}!");
                });

                return model;
            }
        }

        /// <summary>
        /// Save this <c>GuildModel</c> to the database async.
        /// </summary>
        /// <returns>The result of the update request.</returns>
        public async Task<ReplaceOneResult> Save() {
            return await Sharp.GetInstance().Mongo.Database.GetCollection<BsonDocument>("guilds")
                .ReplaceOneAsync(
                    new BsonDocument("Id", Id.ToString()),
                    ToDocument(),
                    new ReplaceOptions {IsUpsert = true});
        }

        /// <summary>
        /// Serialize this <c>GuildModel</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The serialized <c>BsonDocument</c></returns>
        private BsonDocument ToDocument() {
            var users = new BsonArray();

            foreach (var user in Users) {
                users.Add(user.ToDocument());
            }

            var settings = Settings.ToDocument();

            
            var doc = new BsonDocument {
                {"Id", BsonString.Create(Id.ToString())},
                {"Settings", settings},
                {"Users", users},
                {"Modules", ModuleManager.SerializeModules(this)}
            };

            return doc;
        }

        /// <summary>
        /// Deserialize the <c>BsonDocument</c> to it's normal <c>GuildModel</c> form.
        /// </summary>
        /// <returns>The deserialized <c>GuildModel</c></returns>
        public static GuildModel FromDocument(BsonDocument document) {
            var users = document.GetValue("Users").AsBsonArray;

            var deserializedUsers = users.Select((t, index) =>
                    users[(Index) index]).Select(punishment => GuildUserModel.FromDocument(punishment.ToBsonDocument()))
                .ToList();
            
            return new GuildModel(
                (ulong) document.GetValue("Id").ToInt64(),
                GuildSettingsModel.FromDocument(document.GetValue("Settings").ToBsonDocument()),
                deserializedUsers,
                !document.ContainsValue("modules") ? ModuleManager.NewDefault() : ModuleManager.DeserializeModules(document.GetValue("modules").AsBsonDocument));
        }
    }
}