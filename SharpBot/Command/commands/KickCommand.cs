using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Command.commands {
    public class KickCommand : Command {

        private readonly IServiceProvider _services;
        public KickCommand(IServiceProvider _services) : base("kick") {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await message.Channel.SendMessageAsync($"Invalid command usage. Correct usage is `{Sharp.GetCommandPrefix(message)}kick <@User> [reason]`.");
                return;
            }

            var target = UserUtil.GetMemberFrom(message, args[1]);
            if (target == null) {
                await message.Channel.SendMessageAsync($"Could not find a member with the input `{args[1]}`.");
                return;
            }

            var punisher = target.Guild.GetUser(user.Id);
            if (punisher == null) return;

            if (target.Guild.Owner.Equals(target) || target.Hierarchy >= punisher.Hierarchy) {
                await message.Channel.SendMessageAsync($"This members has a higher hierarchy than you, therefore you can't kick them.");
                return;
            }

            var reason = "";
            if (args.Length > 1) {
                var argsList = new List<string>(args);

                for (var i = 0; i <= 1; i++) {
                    argsList.RemoveAt(0);
                }

                reason = string.Join(" ", argsList.ToArray());
            }

            reason = reason == "" ? "Not Specified" : reason;

            await target.SendMessageAsync($"📢 You have been kicked from {punisher.Guild.Name}. Reason: `{reason}`");

            await target.KickAsync(reason).ContinueWith(async task => {
                if (task.IsFaulted) {
                    await message.Channel.SendMessageAsync("Something went wrong when kicking this user." + $" {task.Exception} {task.IsCompletedSuccessfully} {task.Id}");
                    return;
                }
                
                await message.Channel.SendMessageAsync($"📢 {target.Mention} has been kicked from the server. Reason: `{reason}.`");

                var punishment = new Punishment(PunishmentType.Kick, target, punisher, reason, 0);
                _services.GetRequiredService<PunishmentManager>().InsertPunishment(punishment);
            });
        }
    }
}
