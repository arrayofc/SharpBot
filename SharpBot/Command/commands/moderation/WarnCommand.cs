using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Command.commands.moderation {
    public class WarnCommand : Command {

        private readonly IServiceProvider _services;
        public WarnCommand(IServiceProvider _services) : base("warn", CommandType.MODERATION) {
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
                await MessageUtil.SendError(message.Channel, "This member has a higher hierarchy than you, therefore you can't warn them.");
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

           
            var previousWarns = _services.GetRequiredService<PunishmentManager>().GetWarnAmount(target.Guild.Id, target.Id);

            if (previousWarns > 0) {
                await target.SendMessageAsync($"📢 You have been warned in {punisher.Guild.Name} for `{reason}`. This is your **{(previousWarns + 1).DisplayWithSuffix()}** warning in this server.");
            } else {
                await target.SendMessageAsync($"📢 You have been warned in {punisher.Guild.Name}. Reason: `{reason}`.");
            }
            
            await MessageUtil.SendSuccess(message.Channel, $"{target.Mention} has successfully been warned. Reason: `{reason}.`");

            var punishment = new Punishment(PunishmentType.Warn, target, punisher, reason, -1);
            _services.GetRequiredService<PunishmentManager>().InsertPunishment(punishment);
        }
    }
}
