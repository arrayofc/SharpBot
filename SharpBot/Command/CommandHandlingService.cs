using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using SharpBot.Cache;

namespace SharpBot.Command {
    internal class CommandHandlingService {

        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly CommandManager _commandManager;

        public CommandHandlingService(IServiceProvider services) {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commandManager = services.GetRequiredService<CommandManager>();
            _services = services;
            
            _discord.MessageReceived += MessageReceivedAsync;
        }
        
        /// <summary>
        /// This task grabs a message from the fired <c>MessageReceived</c> event,
        /// parses is into a command and executes it.
        /// </summary>
        /// <param name="rawMessage">The raw socket message from the event.</param>
        private async Task MessageReceivedAsync(SocketMessage rawMessage) {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            var prefix = rawMessage.Channel is IPrivateChannel ? '!' : GuildCache.GetGuild(((SocketGuildChannel) rawMessage.Channel).Guild.Id).Settings.CommandPrefix;
            
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos) && !message.ToString().StartsWith(prefix)) return;

            var args = _commandManager.ParseCommand(message);

            var command = CommandManager.GetCommand(args[0].ToLower());

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

            var permission = command.GetPermission();

            if (permission != null) {
                var channel = (SocketGuildChannel)rawMessage.Channel;
                var user = channel?.Guild.GetUser(rawMessage.Author.Id);
                if (user == null) return;
                
                if (!user.GuildPermissions.ToList().Contains(permission.GetValueOrDefault())) {
                    await message.Channel.SendMessageAsync($"You must have the `{command.GetPermission()}` permission to execute this command.").ContinueWith(async msg => {
                        await Task.Delay(5 * 1000);
                        await msg.Result.DeleteAsync();
                    });
                    return;
                }
            }

            await Task.Run(() => {
                Console.WriteLine($"Command \"{string.Join(" ", args)}\" executed by {rawMessage.Author.Username}#{rawMessage.Author.Discriminator} ({rawMessage.Author.Id}) in #{rawMessage.Channel.Name} ({rawMessage.Channel.Id}).");
                command.Execute(rawMessage.Author, message, args);
            });
        }
    }
}
