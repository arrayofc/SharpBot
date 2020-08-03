using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SharpBot.Cache;

namespace SharpBot.Command.commands.management {
    public class SetCommandPrefixCommand : Command {
        public SetCommandPrefixCommand() : base("prefix") { }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var channel = (SocketGuildChannel) message.Channel;
            var socketUser = channel.Guild.GetUser(user.Id);
            
            if (!socketUser.GuildPermissions.ToList().Contains(GuildPermission.ManageGuild)) {
                await message.Channel.SendMessageAsync("You must have permission `MANAGE_GUILD` to do this!");
                return;
            }

            if (args.Length == 1) {
                await message.Channel.SendMessageAsync($"Invalid command usage! Correct usage is `{Sharp.GetCommandPrefix(message)}prefix [char]`.");
                return;
            }

            char prefix;
            
            try {
                prefix = char.Parse(args[1]);
            } catch (FormatException) {
                await message.Channel.SendMessageAsync($"Invalid command usage! The input `{args[0]}` was not a valid prefix character.");
                return;
            }

            var model = GuildCache.GetGuild(channel.Guild.Id);

            if (model.Settings.CommandPrefix == prefix) {
                await message.Channel.SendMessageAsync($"📢 The command prefix was already set to `{prefix}`.");
                return;
            }
            
            model.Settings.CommandPrefix = prefix;
            
            await message.Channel.SendMessageAsync($"📢 The command prefix has been changed to `{prefix}`.");

            await model.Save();
        }
    }
}