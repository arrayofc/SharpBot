using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Command.commands;

namespace SharpBot.Command {
    public class CommandManager {
        public static List<Command> Commands = new List<Command>();

        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandManager(IServiceProvider services) {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            this.RegisterCommands();
        }

        private void RegisterCommands() {
            // Moderation
            new KickCommand(_services).SetGuildOnly().SetPermission(Discord.GuildPermission.KickMembers);
            new VoiceKickCommand(_services).SetGuildOnly().SetPermission(Discord.GuildPermission.KickMembers).PutAliases((new string[] { "vkick" }).ToList());

            // Misc Commands
            new CatCommand(_services).PutAliases((new string[] { "kitten" }).ToList());
            new DogCommand(_services).PutAliases((new string[] { "doggy" }).ToList());
        }

        public static List<Command> GetCommands() {
            return Commands;
        }

        public static Command GetCommand(String invoke) {
            Command found = null;

            foreach (Command command in Commands) {
                if (command.GetInvoke().ToLower().Equals(invoke) || command.GetAliases().Contains(invoke)) {
                    found = command;
                    break;
                }
            }

            return found;
        }

        public string[] ParseCommand(SocketUserMessage message) {
            string raw;

            int num = 0;
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
