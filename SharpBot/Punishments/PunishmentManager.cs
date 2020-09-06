using Discord;
using SharpBot.Cache;
using System;
using System.Collections.Immutable;
using System.Linq;
using SharpBot.Utility;

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

            var userModel = guildModel.GetUser(memberId);
            
            return !userModel.Punishments.Any() ? ImmutableList.Create<Punishment>() 
                : userModel.Punishments.ToImmutableList();
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

            var userModel = guildModel.GetUser(punishment.Punished);
            userModel.AddPunishment(punishment);

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
            embed.AddField("Reason", punishment.Reason);
            embed.AddField("Time", TimeUtil.WhenCompact(punishment.PunishTime), true);
            embed.AddField("Expiration", punishment.Duration == -1 ? "Never" : !IsTemporaryCompatible(punishment) ? "Not Compatible" :
                    TimeUtil.WhenCompact(punishment.ExpirationTime) + $" (in {TimeUtil.Convert(punishment.ExpirationTime)})", true);

            await channel.SendMessageAsync(null, false, embed.Build());
        }

        /// <summary>
        /// Retrieves the amount of previous warnings a member has received.
        /// </summary>
        /// <param name="guildId">The guild to check for.</param>
        /// <param name="memberId">The member to check for.</param>
        /// <returns></returns>
        public int GetWarnAmount(ulong guildId, ulong memberId) {
            var punishments = GetPunishments(guildId, memberId);
            return !punishments.Any() ? 0 : punishments.Count(punishment => punishment.Type == PunishmentType.Warn);
        }

        /// <summary>
        /// Checks whether or not this punishment type can be made into a temporary punishment with
        /// a duration and expiration time.
        /// </summary>
        /// <param name="punishment">The punishment to check for.</param>
        public static bool IsTemporaryCompatible(Punishment punishment) {
            return punishment.Type == PunishmentType.Ban || punishment.Type == PunishmentType.Mute;
        }
    }
}
