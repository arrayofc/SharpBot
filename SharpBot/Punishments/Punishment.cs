using Discord.WebSocket;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SharpBot.Punishments {
    public class Punishment {

        [BsonId]
        public string Id { get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public PunishmentType Type { get; }

        [BsonElement]
        [BsonRepresentation(BsonType.Int64)]
        public ulong Punished { get; }

        [BsonElement]
        [BsonRepresentation(BsonType.Int64)]
        public ulong Punisher { get; }

        [BsonElement]
        [BsonRepresentation(BsonType.String)]
        public string Reason { get; }

        [BsonElement]
        [BsonRepresentation(BsonType.Int64)]
        public long PunishTime { get; }
        
        [BsonElement] 
        [BsonRepresentation(BsonType.Int64)]
        public long ExpirationTime { get; } 
        
        [BsonElement]
        [BsonRepresentation(BsonType.Int64)]
        public long Duration { get; }

        [BsonElement]
        [BsonRepresentation(BsonType.Int64)]
        public ulong Guild { get; }

        public Punishment(PunishmentType type, SocketGuildUser punished, SocketGuildUser punisher, string reason, long duration) :
            this(Guid.NewGuid().ToString().Replace("-", "").Substring(0, 12), type, punished.Id, punisher.Id, reason, DateTimeOffset.Now.ToUnixTimeMilliseconds(), duration, DateTimeOffset.Now.ToUnixTimeMilliseconds() + duration, punished.Guild.Id) {
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
                _ => ""
            };
        }

        public bool IsPermanent() {
            return Duration == -1;
        }
        
        public bool HasExpired() {
            return !IsPermanent() && GetRemaining() <= 0;
        }

        public long GetRemaining() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ExpirationTime;
        }

        public SocketGuild GetGuild() {
            return Sharp.GetInstance().Client.GetGuild(Guild);
        }

        public SocketUser GetPunished() {
            return Sharp.GetInstance().Client.GetUser(Punished);
        }

        public ulong GetPunishedId() {
            return Punished;
        }
        
        public SocketUser GetPunisher() {
            return Sharp.GetInstance().Client.GetUser(Punisher);
        }
        
        public ulong GetPunisherId() {
            return Punisher;
        }

    }
}
