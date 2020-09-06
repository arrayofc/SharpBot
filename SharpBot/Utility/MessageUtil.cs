using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SharpBot.Cache;

namespace SharpBot.Utility {
    public static class MessageUtil {
        private static readonly Random _random = new Random();
        private static readonly string[] Error_Titles = { "Oops, we ran into problems.", "That doesn't seem right...", "A problem occurred!", "Uh-oh, something went wrong." };
        private static readonly string[] Warn_Titles = { "Oops, we ran into problems.", "That doesn't seem right...", "Couldn't finish your request!", "A warning has occured." };
        private static readonly string[] Command_Titles = { "That doesn't seem right...", "Invalid command usage!", "Hm, that didn't work." };
        
        /// <summary>
        /// Send an error message to a channel.
        /// </summary>
        /// <param name="channel">The channel to send to.</param>
        /// <param name="context">The message of the error.</param>
        public static Task<RestUserMessage> SendError(ISocketMessageChannel channel, string context) {
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(255, 51, 51));
            builder.WithTitle(Error_Titles[_random.Next(Error_Titles.Length)]);
            builder.WithDescription(context);

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Send a warning message to a channel.
        /// </summary>
        /// <param name="channel">The channel to send to.</param>
        /// <param name="context">The message of the warning.</param>
        public static Task<RestUserMessage> SendWarning(ISocketMessageChannel channel, string context) {
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(255, 128, 0));
            builder.WithTitle(Warn_Titles[_random.Next(Warn_Titles.Length)]);
            builder.WithDescription(context);

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Send a success message to a channel.
        /// </summary>
        /// <param name="channel">The channel to send to.</param>
        /// <param name="context">A message of what succeeded.</param>
        public static Task<RestUserMessage> SendSuccess(ISocketMessageChannel channel, string context) {
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(0, 179, 90));
            builder.WithTitle("Success!");
            builder.WithDescription(context);

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Send an information message to a channel.
        /// <remarks>does not provide a title of the embed</remarks>
        /// </summary>
        /// <param name="channel">The channel to send to.</param>
        /// <param name="context">The message.</param>
        public static Task<RestUserMessage> SendInfo(ISocketMessageChannel channel, string context) {
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(0, 179, 90));
            builder.WithDescription(context);

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Sends a message about an invalid command usage.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="command">The <c>Command</c></param>
        /// <param name="usage">The usage of this command</param>
        public static Task<RestUserMessage> InvalidCommandUsage(ISocketMessageChannel channel, Command.Command command, string usage) {
            var prefix = channel is IPrivateChannel ? Sharp.GetInstance().ConfigManager.GetConfig().BotPrefix :
                char.Parse(GuildCache.GetGuild(((SocketGuildChannel) channel).Guild.Id).Settings.CommandPrefix);
            
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(255, 51, 51));
            builder.WithTitle(Command_Titles[_random.Next(Command_Titles.Length)]);
            builder.WithDescription($"The correct command usage is `{prefix}{command.GetInvoke()} {usage}`.");

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Sends a message about the bot missing required permissions.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="permissions">The missing permissions.</param>
        public static Task<RestUserMessage> MissingPermissions(ISocketMessageChannel channel, params GuildPermission[] permissions) {
            var builder = new EmbedBuilder();

            builder.WithColor(new Color(255, 51, 51));
            builder.WithTitle("I'm missing permissions!");

            var perms = permissions.Select(permission => "**" + permission + "**").ToList();
            
            builder.WithDescription($"I require the following permissions to perform this action: {string.Join(", ", perms)}.");

            return channel.SendMessageAsync(null, false, builder.Build());
        }
        
        /// <summary>
        /// Formats a string and capitalizes the first letter for each word. 
        /// </summary>
        /// <param name="str">The string to format</param>
        public static string MakeSentence(string str) {
            var ch = str.ToCharArray();
            for (var i = 0; i < str.Length; i++) {
                if (i == 0 && ch[i] != ' ' ||
                    ch[i] != ' ' && ch[i - 1] == ' ') {
                    if (ch[i] >= 'a' && ch[i] <= 'z') {
                        ch[i] = (char) (ch[i] - 'a' + 'A');
                    }
                } else if (ch[i] >= 'A' && ch[i] <= 'Z')
                    ch[i] = (char) (ch[i] + 'a' - 'A');
            }

            return new string(ch);
        }
    }
}