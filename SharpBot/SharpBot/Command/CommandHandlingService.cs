using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpBot.Command {
    class CommandHandlingService {

        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly CommandManager _commandManager;

        public CommandHandlingService(IServiceProvider services) {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commandManager = services.GetRequiredService<CommandManager>();
            _services = services;

            _discord.MessageReceived += MessageReceivedAsync;
        }

        public void Initialize() => Console.WriteLine("Initialized Command Handling service.");

        public async Task MessageReceivedAsync(SocketMessage rawMessage) {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) && !message.ToString().StartsWith(Sharp.getInstance()._configManager.getConfig().BotPrefix)) return;

            string[] args = _commandManager.ParseCommand(message);

            Command command = CommandManager.GetCommand(args[0].ToLower());

            if (command == null) {
                await message.Channel.SendMessageAsync($"Oops, I could not seem to find the command {args[0]}.").ContinueWith(async msg => {
                    await Task.Delay(3 * 1000);
                    await msg.Result.DeleteAsync();
                });
                return;
            }

            if (command.IsGuildOnly() && rawMessage.Channel is IPrivateChannel) {
                await message.Channel.SendMessageAsync("Sorry, this command must be performed in a guild.");
                return;
            }

            if (command.GetPermission() != null) {
                SocketGuildChannel channel = (SocketGuildChannel)rawMessage.Channel;
                if (channel == null) return;
                SocketGuildUser user = channel.Guild.GetUser(rawMessage.Author.Id);
                if (user == null) return;

                if (!(user.GuildPermissions.ToList().Contains(command.GetPermission()))) {
                    await message.Channel.SendMessageAsync($"You must have the {command.GetPermission()} permission to execute this command.").ContinueWith(async msg => {
                        await Task.Delay(3 * 1000);
                        await msg.Result.DeleteAsync();
                    });
                    return;
                }
            }


            await Task.Run(() => {
                Console.WriteLine($"Command \"{String.Join(" ", args)}\" executed by {rawMessage.Author.Username}#{rawMessage.Author.Discriminator} in #{rawMessage.Channel.Name}.");

                command.Execute(rawMessage.Author, (SocketUserMessage)rawMessage, args);
            });
        }
    }
}
