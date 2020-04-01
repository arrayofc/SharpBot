using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SharpBot.Utility {
    public class UserUtil {

        public static SocketGuildUser GetMemberFrom(SocketUserMessage message, string input) {
            SocketGuildChannel channel = (SocketGuildChannel)message.Channel;
            if (channel == null) return null;
            if (input == null) return null;

            if (input.StartsWith("<@") && input.EndsWith(">")) { // We know the input is a tag
                string tagId = input.Substring(input.StartsWith("<@!") ? 3 : 2).Replace(">", "");

                ulong id;

                try {
                    id = ulong.Parse(tagId);
                } catch (FormatException) {
                    return null;
                }

                return channel.Guild.GetUser(id);
            }

            string name;
            int descriminator;

            if (input.Contains("#")) {
                name = input.Split("#")[0];
                try {
                    descriminator = int.Parse(input.Split("#")[1]);
                } catch (FormatException) {
                }
            } else {
                name = input;
            }

            foreach (SocketGuildUser guildUser in channel.Guild.Users) {
                if (guildUser.Username.Equals(name)) return guildUser;
                if (guildUser.Nickname != null && guildUser.Nickname.Equals(name)) return guildUser;
            }

            return null;
        }
    }
}