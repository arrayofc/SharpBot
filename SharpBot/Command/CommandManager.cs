using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Command.commands;
using SharpBot.Command.commands.management;

namespace SharpBot.Command {
    public class CommandManager {
        public static readonly List<Command> Commands = new List<Command>();

        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandManager(IServiceProvider services) {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            RegisterCommands();
        }

        /// <summary>
        /// Register the command classes to the bot.
        /// </summary>
        private void RegisterCommands() {
            // Management
            new SetModLogChannel().SetGuildOnly().SetPermission(Discord.GuildPermission.ManageGuild);
            new ModLogCommand().SetGuildOnly().SetPermission(Discord.GuildPermission.ManageGuild);
            new SetCommandPrefixCommand().SetGuildOnly().PutAliases(new[] { "setprefix", "commandprefix" }.ToList()).SetPermission(Discord.GuildPermission.ManageGuild);

            // Moderation
            new HistoryCommand(_services).SetGuildOnly().SetPermission(Discord.GuildPermission.BanMembers);
            new KickCommand(_services).SetGuildOnly().SetPermission(Discord.GuildPermission.KickMembers);
            new VoiceKickCommand(_services).SetGuildOnly().SetPermission(Discord.GuildPermission.KickMembers).PutAliases(new[] { "vkick" }.ToList());

            // Misc Commands
            new CatCommand(_services).PutAliases(new[] { "kitten" }.ToList());
            new DogCommand(_services).PutAliases(new[] { "doggy" }.ToList());
        }

        /// <summary>
        /// Returns the list of registered <c>Command</c> objects for this bot.
        /// </summary>
        /// <returns>The list of registered commands.</returns>
        public static List<Command> GetCommands() {
            return Commands;
        }

        /// <summary>
        /// Attempt to find a registered command from it's invoke. 
        /// </summary>
        /// <param name="invoke">The command invoke.</param>
        /// <returns>The <c>Command</c> if found, otherwise null.</returns>
        public static Command GetCommand(string invoke) {
            return Commands.FirstOrDefault(command => command.GetInvoke().ToLower().Equals(invoke) || command.GetAliases().Contains(invoke));
        }

        /// <summary>Parses a <c>SocketUserMessage</c> into a command with arguments.</summary>
        /// <param name="message">The socket message to parse.</param>
        /// <returns>A parsed command with arguments.</returns>
        public string[] ParseCommand(SocketUserMessage message) {
            string raw;

            var num = 0;
            if (message.HasMentionPrefix(_discord.CurrentUser, ref num)) {
                Console.WriteLine(message.Content);

                raw = message.Content.Substring(_discord.CurrentUser.Id.ToString().Length + 5);
            } else {
                raw = message.Content.Substring(1);
            }

            return raw.Split(" ");
        }
    }
}
