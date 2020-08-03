using System;
using Discord.WebSocket;

namespace SharpBot.Utility {
    public static class UserUtil {

        public static SocketGuildUser GetMemberFrom(SocketUserMessage message, string input) {
            var channel = (SocketGuildChannel)message.Channel;
            if (channel == null) return null;
            if (input == null) return null;

            if (input.StartsWith("<@") && input.EndsWith(">")) { // We know the input is a tag
                var tagId = input.Substring(input.StartsWith("<@!") ? 3 : 2).Replace(">", "");

                ulong id;

                try {
                    id = ulong.Parse(tagId);
                } catch (FormatException) {
                    return null;
                }

                return channel.Guild.GetUser(id);
            }

            var name = input.Contains("#") ? input.Split("#")[0].ToLower() : input.ToLower();

            foreach (var guildUser in channel.Guild.Users) {
                if (guildUser.Username.ToLower().Equals(name)) return guildUser;
                if (guildUser.Nickname != null && guildUser.Nickname.ToLower().Equals(name)) return guildUser;
            }

            return null;
        }
    }
}