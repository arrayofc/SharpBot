using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SharpBot.Cache;

namespace SharpBot.Command.commands.management {
    public class SetModLogChannel : Command {
        public SetModLogChannel() : base("setmodlog") { }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var channel = (SocketGuildChannel) message.Channel;
            var socketUser = channel.Guild.GetUser(user.Id);

            var textChannel = (SocketTextChannel) channel;

            if (!socketUser.GuildPermissions.ToList().Contains(GuildPermission.ManageGuild)) {
                await message.Channel.SendMessageAsync($"You must have permission `MANAGE_GUILD` to do this!");
                return;
            }

            var model = GuildCache.GetGuild(channel.Guild.Id);
            model.Settings.ModerationLogChannel = channel.Id;
            
            await message.Channel.SendMessageAsync($"📢 The moderation log channel has been changed to {textChannel.Mention}.`");

            await model.Save();
        }
    }
}