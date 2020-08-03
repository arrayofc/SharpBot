using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Cache;
using SharpBot.Command;
using SharpBot.Configuration;
using SharpBot.Database;
using SharpBot.Punishments;
using SharpBot.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SharpBot {
    public class Sharp {
        private static Sharp _instance;

        public DiscordSocketClient Client { get; private set; }

        public ConfigManager ConfigManager { get; private set; }
        
        public Mongo Mongo { get; private set; }

        public GuildCache GuildCache { get; set; } 

        /// <summary>
        /// Sets up the bot and all of it necessary classes.
        /// </summary>
        public async Task MainAsync() {
            _instance = this;

            ConfigManager = new ConfigManager();

            if (ConfigManager.GetConfig() == null) {
                Console.WriteLine("Warning: Configuration was null, shutting down...");
                Environment.Exit(-1);
                return;
            }

            Mongo = new Mongo(this);
            GuildCache = new GuildCache();

            await using var services = ConfigureServices();

            // ReSharper disable once ObjectCreationAsStatement
            new CommandHandlingService(services);

            Client = services.GetRequiredService<DiscordSocketClient>();
            Client.Log += LogAsync;

            if (ConfigManager.GetConfig().BotToken.Equals("")) {
                Console.WriteLine("Warning: No bot token was found, shutting down...");
                Environment.Exit(-1);
                return;
            }

            await Client.LoginAsync(TokenType.Bot, ConfigManager.GetConfig().BotToken);
            await Client.SetStatusAsync(UserStatus.Online);
            await Client.SetActivityAsync(new Game("@Sharp", ActivityType.Listening));
            await Client.StartAsync();
            
            Client.Ready += ReadyAsync;

            await Task.Delay(-1);
        }

        /// <summary>
        /// Listener for when the client receives a log.
        /// </summary>
        /// <param name="log">The <c>LogMessage</c> object.</param>
        /// <returns>The task when finished.</returns>
        private static Task LogAsync(LogMessage log) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(log.ToString());
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// The ready event listener.
        /// </summary>
        private Task ReadyAsync() {
            Console.WriteLine($"{Client.CurrentUser} is now connected.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the instance of the main class.
        /// </summary>
        /// <returns>The <c>Sharp</c> instance.</returns>
        public static Sharp GetInstance() {
            return _instance;
        }

        /// <summary>
        /// Get the command prefix this bot is meant to listen for.
        /// </summary>
        /// <returns>The command prefix, '!' as default.</returns>
        public static char GetCommandPrefix(SocketUserMessage message) {
            return message.Channel is IPrivateChannel ? GetInstance().ConfigManager.GetConfig().BotPrefix :
                GuildCache.GetGuild(((SocketGuildChannel) message.Channel).Guild.Id).Settings.CommandPrefix;
        }

        /// <summary>
        /// Configures the service provider and adds the dependencies.
        /// </summary>
        /// <returns>The configured <c>ServiceProvider</c>.</returns>
        private static ServiceProvider ConfigureServices() => new ServiceCollection()
            // Discord
            .AddSingleton<DiscordSocketClient>()
            
            // Commands
            .AddSingleton<CommandManager>()
            .AddSingleton<CommandHandlingService>()
            
            // Web & Http
            .AddSingleton<HttpClient>()
            .AddSingleton<ImageWebService>()
            
            // Punishments
            .AddSingleton<PunishmentManager>()
            
            .BuildServiceProvider();
    }
}
