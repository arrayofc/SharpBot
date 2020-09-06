using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SharpBot.Cache;
using SharpBot.Utility;

namespace SharpBot.Command.commands.management {
    public class SetCommandPrefixCommand : Command {
        public SetCommandPrefixCommand() : base("prefix", CommandType.MANAGEMENT) { }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var channel = (SocketGuildChannel) message.Channel;
            var socketUser = channel.Guild.GetUser(user.Id);
            
            if (!socketUser.GuildPermissions.ToList().Contains(GuildPermission.ManageGuild)) {
                await MessageUtil.SendError(message.Channel, "You must have permission `MANAGE_GUILD` to do this!");
                return;
            }

            if (args.Length == 1) {
                await MessageUtil.InvalidCommandUsage(message.Channel, this, "<char>");
                return;
            }

            char prefix;
            
            try {
                prefix = char.Parse(args[1]);
            } catch (FormatException) {
                await MessageUtil.InvalidCommandUsage(message.Channel, this, "<char>");
                return;
            }

            var model = GuildCache.GetGuild(channel.Guild.Id);

            if (model.Settings.CommandPrefix == prefix.ToString()) {
                await MessageUtil.SendWarning(message.Channel, $"The command prefix was already set to `{prefix}`.");
                return;
            }
            
            model.Settings.CommandPrefix = prefix.ToString();
            
            await MessageUtil.SendSuccess(message.Channel, $"The command prefix has been changed to `{prefix}`.");

            await model.Save();
        }
    }
}