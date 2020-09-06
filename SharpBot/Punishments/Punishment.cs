using Discord.WebSocket;
using MongoDB.Bson;
using System;

namespace SharpBot.Punishments {
    public class Punishment {

        public string Id { get; }
        
        public PunishmentType Type { get; }

        public ulong Punished { get; }
        public ulong Punisher { get; }

        public string Reason { get; }

        public long PunishTime { get; }
        public long ExpirationTime { get; }
        public long Duration { get; }
        public ulong Guild { get; }

        public Punishment(PunishmentType type, SocketGuildUser punished, SocketGuildUser punisher, string reason, long duration) :
            this(Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10), type, punished.Id, punisher.Id, reason, DateTimeOffset.Now.ToUnixTimeMilliseconds(), duration, DateTimeOffset.Now.ToUnixTimeMilliseconds() + duration, punished.Guild.Id) {
        }

        public Punishment(string id, PunishmentType type, ulong punished, ulong punisher, string reason, long pTime, long eTime, long dur, ulong guild) {
            Id = id;
            Type = type;
            Punished = punished;
            Punisher = punisher;
            Reason = reason;
            PunishTime = pTime;
            ExpirationTime = eTime;
            Duration = dur;
            Guild = guild;
        }

        public string GetNamedType() {
            return Type switch {
                PunishmentType.Ban => "banned",
                PunishmentType.Kick => "kicked",
                PunishmentType.Warn => "warned",
                PunishmentType.Voice_Kick => "voice-kicked",
                PunishmentType.Mute => "muted",
                _ => ""
            };
        }

        /// <summary>
        /// Returns true if this punishment is permanent.
        /// </summary>
        public bool IsPermanent() {
            return Duration == -1;
        }
        
        /// <summary>
        /// Check whether or not this punishment has expired.
        /// </summary>
        public bool HasExpired() {
            return !IsPermanent() && GetRemaining() <= 0;
        }

        /// <summary>
        /// Get the remaining time in millis for this punishment.
        /// </summary>
        public long GetRemaining() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ExpirationTime;
        }

        /// <summary>
        /// Get the origin <c>SocketGuild</c> of this punishment. 
        /// </summary>
        public SocketGuild GetGuild() {
            return Sharp.GetInstance().Client.GetGuild(Guild);
        }

        /// <summary>
        /// Get the punished <c>SocketUser</c>. 
        /// </summary>
        public SocketUser GetPunished() {
            return Sharp.GetInstance().Client.GetUser(Punished);
        }

        /// <summary>
        /// Returns the user id of the punished user.
        /// </summary>
        public ulong GetPunishedId() {
            return Punished;
        }
        
        /// <summary>
        /// Get the <c>SocketUser</c> of the punisher. 
        /// </summary>
        public SocketUser GetPunisher() {
            return Sharp.GetInstance().Client.GetUser(Punisher);
        }
        
        /// <summary>
        /// Get the user id of the punisher.
        /// </summary>
        public ulong GetPunisherId() {
            return Punisher;
        }

        /// <summary>
        /// Gives the expiration time in millis for this punishment.
        /// </summary>
        /// <returns>
        /// The expiration time for this punishment in millis.
        /// <remarks>
        /// If the punishment type is not compatible for a set time,
        /// it will return the <see cref="PunishTime"/>> instead.
        /// <seealso cref="PunishmentManager.IsTemporaryCompatible(Punishment)"/>
        /// </remarks></returns>
        public long GetExpirationTime() {
            if (Type == PunishmentType.Kick || Type == PunishmentType.Voice_Kick || Type == PunishmentType.Warn) {
                return PunishTime;
            }

            return ExpirationTime;
        }
       
        /// <summary>
        /// Serialize this <c>Punishment</c> to a <c>BsonDocument</c>.
        /// </summary>
        /// <returns>The <c>BsonDocument</c></returns>
        public BsonDocument ToDocument() {
            return new BsonDocument {
                {"Id", BsonString.Create(Id)},
                {"Type", BsonString.Create(Type.ToString())},
                {"Punished", BsonInt64.Create(Punished.ToString())},
                {"Punisher", BsonInt64.Create(Punisher.ToString())},
                {"Reason", BsonString.Create(Reason)},
                {"PunishTime", BsonInt64.Create(PunishTime.ToString())},
                {"ExpirationTime", BsonInt64.Create(ExpirationTime.ToString())},
                {"Duration", BsonInt64.Create(Duration.ToString())},
                {"Guild", BsonInt64.Create(Guild.ToString())}
            };
        }

        /// <summary>
        /// Deserializes a <c>BsonDocument</c> and creates a new
        /// <c>Punishment</c> object from the document values.
        /// </summary>
        /// <param name="document">The document to deserialize.</param>
        public static Punishment FromDocument(BsonDocument document) {
            return new Punishment(
                document.GetValue("Id").AsString,
                (PunishmentType) Enum.Parse(typeof(PunishmentType), document.GetValue("Type").AsString),
                (ulong) document.GetValue("Punished").AsInt64,
                (ulong) document.GetValue("Punisher").AsInt64,
                document.GetValue("Reason").AsString,
                document.GetValue("PunishTime").AsInt64,
                document.GetValue("ExpirationTime").AsInt64,
                document.GetValue("Duration").AsInt64,
                (ulong) document.GetValue("Guild").AsInt64);
        }
    }
}
