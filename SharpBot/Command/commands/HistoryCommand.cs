using Discord;
using Discord.WebSocket;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBot.Command.commands {
    public class HistoryCommand : Command {

        private readonly IServiceProvider _services;
        public HistoryCommand(IServiceProvider _services) : base("history") {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await message.Channel.SendMessageAsync($"Invalid command usage. Correct usage is `{Sharp.GetCommandPrefix(message)}history <@User|Id>`.");
                return;
            }

            var target = UserUtil.GetMemberFrom(message, args[1]);
            
            ulong targetId;
            
            if (target == null) {
                try {
                    targetId = ulong.Parse(args[1]);
                } catch (FormatException) {
                    await message.Channel.SendMessageAsync($"Could not find a member with the input `{args[1]}`.");
                    return;
                }
            } else {
                targetId = target.Id;
            }

            var channel = (SocketGuildChannel) message.Channel;

            var punishments = PunishmentManager.GetPunishments(channel.Guild.Id, targetId);

            if (!punishments.Any()) {
                await message.Channel.SendMessageAsync("No punishment records found for this user.");
                return;
            }

            var builder = new StringBuilder();

            for (var i = 0; i < punishments.Count; i++) {
                var punishment = punishments[i];
                var isLast = i == punishments.Count;

                var targetUser = channel.Guild.GetUser(targetId);

                builder.Append("`").Append(punishment.Id).Append("`").Append(" ")
                    .Append(targetUser == null
                        ? targetId.ToString()
                        : targetUser.Username + "#" + targetUser.Discriminator + " `(" + targetId + ")` ")
                    .Append("was ").Append(punishment.GetNamedType()).Append(" ").Append("by ")
                    .Append(punishment.GetPunisher() == null
                        ? punishment.GetPunisherId().ToString()
                        : punishment.GetPunisher().Username + "#" + punishment.GetPunisher().Discriminator + " `(" +
                          punishment.GetPunisherId() + ")` ")
                    .Append("at ").Append("**")
                    .Append(DateTimeOffset.FromUnixTimeMilliseconds(punishment.PunishTime).DateTime
                        .ToString("dddd, dd, MMMM HH:mm:ss")).Append("**").Append(" ");

                if (punishment.HasExpired() || punishment.Type == PunishmentType.Kick || punishment.Type == PunishmentType.Voice_Kick) {
                    builder.Append("[Expired]");
                }
                else {
                    var timeSpan = new TimeSpan(punishment.GetRemaining());

                    builder.Append("[Expires ").Append(punishment.IsPermanent()
                        ? "never"
                        : $"{timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes, {timeSpan.Seconds} seconds").Append("]");
                }

                if (!isLast) {
                    builder.Append(" \n");
                }
            }

            await message.Channel.SendMessageAsync(builder.ToString());
        }
    }
}
