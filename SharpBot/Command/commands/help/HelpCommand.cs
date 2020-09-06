using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnsClient.Protocol;
using SharpBot.Cache;
using SharpBot.Utility;

namespace SharpBot.Command.commands.help {
    public class HelpCommand : Command {

        private readonly IServiceProvider _services;

        public HelpCommand(IServiceProvider services) : base("help", CommandType.GENERAL) {
            _services = services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var prefix = message.Channel is IPrivateChannel ? Sharp.GetInstance().ConfigManager.GetConfig().BotPrefix :
                char.Parse(GuildCache.GetGuild(((SocketGuildChannel) message.Channel).Guild.Id).Settings.CommandPrefix);

            if (args.Length == 1) {

                var embed = new EmbedBuilder();

                embed.WithTitle("Commands Help");
                embed.WithDescription("For further information, you may use \n" +
                                      $"`{prefix}help [command-type|command].` \n " +
                                      " \n ");
                
                var current = 1;
                
                foreach (var type in Enum.GetValues(typeof(CommandType)).Cast<CommandType>()) {
                    var commands = CommandManager.GetCommands().Where(command => command.GetType() == type)
                        .Select(command => "`" + prefix + command.Invoke + "`").ToList();
                   
                    if (current == 2) {
                        embed.AddField(builder => builder.WithName("\u200b").WithValue("\u200b").WithIsInline(true));
                        current++;
                    }
                    
                    current++;
                    embed.AddField(builder => builder.WithName(MessageUtil.MakeSentence(type + " Commands")).WithValue(
                        commands.Any() ? string.Join(", ", commands) : "No commands here yet!").WithIsInline(current != 3));

                    if (current == 3) {
                        current = 1;
                    }
                }

                await message.Channel.SendMessageAsync(null, false, embed.Build());
                
            } else if (Enum.GetValues(typeof(CommandType)).Cast<CommandType>().Select(type => type.ToString().ToLower())
                .Any(s => args[1].Equals(s))) {

                var commandType = Enum.GetValues(typeof(CommandType)).Cast<CommandType>()
                    .FirstOrDefault(s => args[1].ToLower().Equals(s.ToString().ToLower()));

                var commands = CommandManager.GetCommands().Where(command => command.GetType() == commandType)
                    .Select(command => "`" + prefix + command.Invoke + "`").ToList();

                var embed = new EmbedBuilder();

                embed.WithTitle(MessageUtil.MakeSentence(commandType + " Commands  🡒  Help"));
                embed.WithDescription("For further information, you may use \n" +
                                      $"`{prefix}help [command-type|command].` \n " +
                                      " \n ");
                embed.AddField(builder => builder.WithName(MessageUtil.MakeSentence(commandType + " Commands")).WithValue(
                    commands.Any() ? string.Join(", ", commands) : "No commands here yet!").WithIsInline(true));

                await message.Channel.SendMessageAsync(null, false, embed.Build());
                
            } else if (CommandManager.GetCommands().Any(command => command.Invoke.ToLower().Equals(args[1].ToLower()))) {
                var command = CommandManager.GetCommands()
                    .FirstOrDefault(cmd => cmd.Invoke.ToLower().Equals(args[1].ToLower()));

                if (command == null) {
                    await MessageUtil.SendError(message.Channel, "Could not find a command with invoke `" + args[1] + "`.");
                    return;
                }
                
                await SendCommandInfo(message, command);
            }
        }

        private async Task SendCommandInfo(SocketMessage channel, Command command) {
            var embed = new EmbedBuilder();
            var builder = new StringBuilder();

            builder.Append($"**Command Invoke:** {command.Invoke}")
                .Append("\n \n ");

            if (command.Aliases.Any()) {
                builder.Append("**Aliases:** " + string.Join(", ", command.Aliases));
                builder.Append("\n \n ");
            }
            
            builder.Append("**Guild Only:** " + (command.IsGuildOnly() ? "Yes" : "No"));
            builder.Append("\n \n ");
            builder.Append("**Description:** \n");
            builder.Append(command.GetDescription());
            builder.Append("\n \n ");
            builder.Append("**Type:** " + MessageUtil.MakeSentence(command.GetType() + " Command"));
            builder.Append("\n \n ");
            if (command.Permission.HasValue) {
                builder.Append("**Required Permission:** " + command.GetPermission().Value);
                builder.Append("\n \n ");
            }

            embed.WithTitle(command.GetInvoke() + "  🡒  Help");
            embed.WithDescription(builder.ToString());

            await channel.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
