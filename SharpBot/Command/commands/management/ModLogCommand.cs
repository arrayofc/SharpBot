using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SharpBot.Cache;

namespace SharpBot.Command.commands.management {
    public class ModLogCommand : Command {
        public ModLogCommand() : base("modlog") { }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var channel = (SocketGuildChannel) message.Channel;
            var socketUser = channel.Guild.GetUser(user.Id);
            
            if (!socketUser.GuildPermissions.ToList().Contains(GuildPermission.ManageGuild)) {
                await message.Channel.SendMessageAsync("You must have permission `MANAGE_GUILD` to do this!");
                return;
            }
            
            var model = GuildCache.GetGuild(channel.Guild.Id);

            var modLogChannel = channel.Guild.GetTextChannel(model.Settings.ModerationLogChannel);
            
            if (model.Settings.ModLogEnabled() && modLogChannel != null) {
                await message.Channel.SendMessageAsync($"📢 The moderation log channel is currently set to {modLogChannel.Mention}.");
                
            } else if (model.Settings.ModLogEnabled()) {
                await message.Channel.SendMessageAsync("The moderation log channel appears to no longer be available. Please set a new one!");

            } else {
                await message.Channel.SendMessageAsync("📪 There is currently no moderation log set!");
            }
        }
    }
}