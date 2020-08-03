using Discord;
using SharpBot.Cache;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SharpBot.Punishments {
    public class PunishmentManager {

        private readonly IServiceProvider _services;

        public PunishmentManager(IServiceProvider _services) {
            this._services = _services;
        }

        /// <summary>
        /// Returns an immutable list of the members punishment for a said guild. 
        /// </summary>
        /// <param name="guildId">The guild</param>
        /// <param name="memberId">The member to fetch history for</param>
        /// <returns>An unmodifiable list of <c>Punishment</c> objects for this member.</returns>
        public static ImmutableList<Punishment> GetPunishments(ulong guildId, ulong memberId) {
            var guildModel = GuildCache.GetGuild(guildId);

            return !guildModel.Punishments.Any() ? ImmutableList.Create<Punishment>() 
                : guildModel.Punishments.Where(punishment => punishment.Punished == memberId).ToImmutableList();
        }

        /// <summary>
        /// Registers and handles a new punishment.
        /// </summary>
        /// <param name="punishment">The punishment to insert.</param>
        public async void InsertPunishment(Punishment punishment) {
            var guild = punishment.GetGuild();
            if (guild == null) return;

            var guildModel = GuildCache.GetGuild(guild.Id);
            if (guildModel == null) return;

            guildModel.Punishments.Add(punishment);

            var result = await guildModel.Save();
            if (!result.IsAcknowledged) {
                Console.WriteLine("Could not insert punishment " + punishment.Id + " to database.");
            }

            if (!guildModel.Settings.ModLogEnabled()) return;
            
            var channel = guild.GetTextChannel(guildModel.Settings.ModerationLogChannel);
            if (channel == null) return;
                
            var embed = new EmbedBuilder();
            embed.WithDescription("A new punishment has been inserted.");
            embed.WithTitle("New Punishment");
            embed.AddField("Type", punishment.Type.ToString(), true);
            embed.AddField("Punished", punishment.GetPunished().Mention, true);
            embed.AddField("Punisher", punishment.GetPunisher().Mention, true);
            embed.AddField("Reason", punishment.Reason, true);
            embed.AddField("Time", DateTimeOffset.FromUnixTimeMilliseconds(punishment.PunishTime).DateTime.ToString("dddd, dd, MMMM HH:mm:ss"), true);
            embed.AddField("Expiration", punishment.Duration == 0 ? "N/A" : DateTimeOffset.FromUnixTimeMilliseconds(punishment.ExpirationTime).DateTime.ToString("dddd, dd, MMMM HH:mm:ss"), true);

            await channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
