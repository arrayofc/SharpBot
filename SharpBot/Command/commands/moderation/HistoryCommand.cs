using Discord;
using Discord.WebSocket;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SharpBot.Command.commands.moderation {
    public class HistoryCommand : Command {

        private readonly IServiceProvider _services;
        public HistoryCommand(IServiceProvider _services) : base("history", CommandType.MODERATION) {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await MessageUtil.InvalidCommandUsage(message.Channel, this, "<@User|Id> [page]");
                return;
            }

            var target = UserUtil.GetMemberFrom(message, args[1]);
            
            ulong targetId;
            
            if (target == null) {
                try {
                    targetId = ulong.Parse(args[1]);
                } catch (FormatException) {
                    await MessageUtil.SendWarning(message.Channel, $"Could not find a member with the input `{args[1]}`");
                    return;
                }
            } else {
                targetId = target.Id;
            }

            var channel = (SocketGuildChannel) message.Channel;

            var punishments = PunishmentManager.GetPunishments(channel.Guild.Id, targetId);

            if (!punishments.Any()) {
                await MessageUtil.SendWarning(message.Channel, "No punishment records were found for this user");
                return;
            }

            int page;

            if (args.Length == 2) {
                try {
                    page = int.Parse(args[2]);
                } catch (FormatException) {
                    await MessageUtil.InvalidCommandUsage(message.Channel, this, "<@User|Id> [page]");
                    return;
                }
            } else {
                page = 1;
            }

            var chopped = punishments.ChopList(3);
            
            if (page < 1 || page > chopped.Count) {
                await MessageUtil.SendWarning(message.Channel, "The page you tried to access does not exist.");
                return;
            }

            var display = chopped[page - 1];
            
            var i = 1;
            
            foreach (var punishment in display) {
                var embed = new EmbedBuilder();
                
                embed.WithTitle("Punishment #" + punishment.Id);

                embed.WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(punishment.PunishTime));
                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Type")
                    .WithValue(punishment.GetNamedType())
                    .WithIsInline(true));
                
                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Reason")
                    .WithValue(punishment.Reason)
                    .WithIsInline(true));
                
                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Punished by")
                    .WithValue(punishment.GetPunisher() == null
                        ? punishment.GetPunisherId().ToString()
                        : punishment.GetPunisher().Mention)
                    .WithIsInline(true));
                
                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Punishment Time")
                    .WithValue(TimeUtil.WhenSmall(punishment.PunishTime))
                    .WithIsInline(true));

                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Duration")
                    .WithValue(punishment.IsPermanent() ? "Permanent" : !PunishmentManager.IsTemporaryCompatible(punishment) ? "Not Compatible" : TimeUtil.Convert(punishment.Duration))
                    .WithIsInline(true));

                embed.AddField(new EmbedFieldBuilder()
                    .WithName("Expiration Time")
                    .WithValue(punishment.HasExpired() ? "Expired " + TimeUtil.WhenCompact(punishment.GetExpirationTime()) : punishment.IsPermanent() ? "Never" : !PunishmentManager.IsTemporaryCompatible(punishment) ? "Not Compatible" : TimeUtil.WhenSmall(punishment.GetExpirationTime()))
                    .WithIsInline(true));
                
                if (punishment.HasExpired() || !PunishmentManager.IsTemporaryCompatible(punishment)) {
                    embed.WithDescription("This punishment expired at **" + TimeUtil.WhenDetailed(punishment.GetExpirationTime()) + "**.");
                    
                } else {
                    if (punishment.IsPermanent()) {
                        embed.WithDescription("This punishment is active and does not have an expiry date (is permanent).");
                    }
                    else {
                        embed.WithDescription("This punishment is active and expires in **" + TimeUtil.Convert(punishment.GetRemaining()) + "**.");
                    }
                }

                var first = i == 1;
                if (first) {
                    await message.Channel.SendMessageAsync(
                        "> Displaying **" + display.Count + " " + $"out of {punishments.Count}** " + (display.Count > 1 ? "Punishments" : "Punishment") + "  🡒  Page " + page + "/" + chopped.Count, false,
                        embed.Build());
                } else {
                    await message.Channel.SendMessageAsync(null, false, embed.Build());
                }

                i--;
            }
        }
    }
}
