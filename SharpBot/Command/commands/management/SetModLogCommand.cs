using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SharpBot.Cache;
using SharpBot.Utility;

namespace SharpBot.Command.commands.management {
    public class SetModLogChannel : Command {
        public SetModLogChannel() : base("setmodlog", CommandType.MANAGEMENT) { }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var channel = (SocketGuildChannel) message.Channel;
            var socketUser = channel.Guild.GetUser(user.Id);

            var textChannel = (SocketTextChannel) channel;

            if (!socketUser.GuildPermissions.ToList().Contains(GuildPermission.ManageGuild)) {
                await MessageUtil.SendError(message.Channel, "You must have permission `MANAGE_GUILD` to do this!");
                return;
            }

            var model = GuildCache.GetGuild(channel.Guild.Id);
            model.Settings.ModerationLogChannel = channel.Id;
            
            await MessageUtil.SendInfo(message.Channel, $"The moderation log channel has been changed to {textChannel.Mention}.");
            
            await model.Save();
        }
    }
}