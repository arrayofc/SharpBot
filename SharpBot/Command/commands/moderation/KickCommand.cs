using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Command.commands.moderation {
    public class KickCommand : Command {

        private readonly IServiceProvider _services;
        public KickCommand(IServiceProvider _services) : base("kick", CommandType.MODERATION) {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await MessageUtil.InvalidCommandUsage(message.Channel, this, "<@User> [reason]");
                return;
            }

            var target = UserUtil.GetMemberFrom(message, args[1]);
            if (target == null) {
                await MessageUtil.SendWarning(message.Channel, $"Could not find a member with the input `{args[1]}`");
                return;
            }

            var punisher = target.Guild.GetUser(user.Id);
            if (punisher == null) return;

            if (target.Guild.Owner.Equals(target) || target.Hierarchy >= punisher.Hierarchy) {
                await MessageUtil.SendError(message.Channel, "This member has a higher hierarchy than you, therefore you can't kick them.");
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

            if (!Sharp.CheckPermissions(target.Guild, GuildPermission.KickMembers)) {
                await MessageUtil.MissingPermissions(message.Channel, GuildPermission.KickMembers);
                return;
            }
            
            await target.SendMessageAsync($"📢 You have been kicked from {punisher.Guild.Name}. Reason: `{reason}`");

            await target.KickAsync(reason).ContinueWith(async task => {
                if (task.IsFaulted) {
                    await MessageUtil.SendError(message.Channel, "Something went wrong when kicking this user." + $" {task.Exception} {task.IsCompletedSuccessfully} {task.Id}");
                    return;
                }
                
                await MessageUtil.SendSuccess(message.Channel, $"{target.Mention} has successfully been kicked from the server. Reason: `{reason}.`");
                
                var punishment = new Punishment(PunishmentType.Kick, target, punisher, reason, -1);
                _services.GetRequiredService<PunishmentManager>().InsertPunishment(punishment);
            });
        }
    }
}
