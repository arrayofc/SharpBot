using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBot.Punishments {
    class Punishment {

        public string Id { get; set; }
        public PunishmentType Type { get; set; }

        public ulong Punished { get; set; }
        public ulong Punisher { get; set; }

        public string Reason { get; set; }

        public long PunishTime { get; set; }
        public long ExpirationTime { get; set; }
        public long Duration { get; set; }

        public ulong Guild { get; set; }

        public Punishment(PunishmentType type, SocketGuildUser punished, SocketGuildUser punisher, string reason, long duration) :
            this(Guid.NewGuid().ToString(), type, punished.Id, punisher.Id, reason, DateTimeOffset.Now.ToUnixTimeMilliseconds(), duration, DateTimeOffset.Now.ToUnixTimeMilliseconds() + duration, punished.Guild.Id) {
        }

        public Punishment(string id, PunishmentType type, ulong punished, ulong punisher, string reason, long pTime, long eTime, long dur, ulong guild) {
            this.Id = id;
            this.Type = type;
            this.Punished = punished;
            this.Punisher = punisher;
            this.Reason = reason;
            this.PunishTime = pTime;
            this.ExpirationTime = eTime;
            this.Duration = dur;
            this.Guild = guild;
        }


        public bool HasExpired() {
            return GetRemaining() <= 0;
        }

        public long GetRemaining() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ExpirationTime;
        }

        public SocketGuild GetGuild() {
            return Sharp.getInstance()._client.GetGuild(this.Guild);
        }

        public SocketUser getPunished() {
            return Sharp.getInstance()._client.GetUser(this.Punished);
        }

        public SocketUser getPunisher() {
            return Sharp.getInstance()._client.GetUser(this.Punisher);
        }
    }
}
