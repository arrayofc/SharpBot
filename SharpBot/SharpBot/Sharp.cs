using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Command;
using SharpBot.Configuration;
using SharpBot.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpBot {
    class Sharp {

        public static Sharp instance = null;

        public DiscordSocketClient _client { get; private set; }

        public ConfigManager _configManager { get; set; }

        public async Task MainAsync() {
            instance = this;

            _configManager = new ConfigManager();

            if (_configManager.getConfig() == null) {
                Console.WriteLine("Warning: Configuration was null, shutting down...");
                Environment.Exit(-1);
                return;
            }

            using var services = ConfigureServices();

            this._client = services.GetRequiredService<DiscordSocketClient>();
            this._client.Log += LogAsync;

            if (_configManager.getConfig().BotToken.Equals("")) {
                Console.WriteLine("Warning: No bot token was found, shutting down...");
                Environment.Exit(-1);
                return;
            }

            await _client.LoginAsync(TokenType.Bot, _configManager.getConfig().BotToken);
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetActivityAsync(new Game("@Sharp", ActivityType.Listening));
            await _client.StartAsync();

            services.GetRequiredService<CommandHandlingService>().Initialize();

            this._client.Ready += ReadyAsync;

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log) {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync() {
            Console.WriteLine($"{_client.CurrentUser} is now connected.");
            return Task.CompletedTask;
        }

        public static Sharp getInstance() {
            return instance;
        }

        public static char GetCommandPrefix() {
            return getInstance()._configManager.getConfig().BotPrefix;
        }

        private ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandManager>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
    }
}
