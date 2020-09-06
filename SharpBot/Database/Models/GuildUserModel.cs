using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using SharpBot.Punishments;

namespace SharpBot.Database.Models {
    public class GuildUserModel {

        public ulong Id { get; }
        public List<Punishment> Punishments { get; }
        
        public GuildUserModel(ulong id) {
            Id = id;
            Punishments = new List<Punishment>();
        }

        private GuildUserModel(ulong id, List<Punishment> punishments) {
            Id = id;
            Punishments = punishments;
        }

        /// <summary>
        /// Add a <c>Punishment</c> to this user.
        /// </summary>
        /// <param name="punishment">Punishment to add.</param>
        public void AddPunishment(Punishment punishment) {
            Punishments.Add(punishment);
        }

        /// <summary>
        /// Remove a <c>Punishment</c> from this user.
        /// </summary>
        /// <param name="punishment">Punishment to remove.</param>
        public void RemovePunishment(Punishment punishment) {
            Punishments.Remove(punishment);
        }
        
        /// <summary>
        /// Serialize this <c>GuildUserModel</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The serialized <c>BsonDocument</c></returns>
        public BsonDocument ToDocument() {
            var punishments = new BsonArray();

            foreach (var punishment in Punishments) {
                punishments.Add(punishment.ToDocument());
            }
            
            return new BsonDocument {
                {"Id", BsonString.Create(Id.ToString())},
                {"Punishments", punishments},
            };
        }
        
        /// <summary>
        /// Deserialize the <c>BsonDocument</c> to it's normal <c>GuildUserModel</c> form.
        /// </summary>
        /// <returns>The deserialized <c>GuildUserModel</c></returns>
        public static GuildUserModel FromDocument(BsonDocument document) {
            var punishments = document.GetValue("Punishments").AsBsonArray;

            var deserializedPunishments = punishments.Select((t, index) => 
                punishments[(Index) index]).Select(punishment => Punishment.FromDocument(punishment.AsBsonDocument)).ToList();

            return new GuildUserModel(
                (ulong) document.GetValue("Id").ToInt64(),
                deserializedPunishments);
        }
    }
}
