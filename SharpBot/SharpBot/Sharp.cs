using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpBot {
    class Sharp {

        public DiscordSocketClient _client { get; private set; }

        public async Task MainAsync() {
            using var services = ConfigureServices();

            this._client = services.GetRequiredService<DiscordSocketClient>();

            this._client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            //await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.LoginAsync(TokenType.Bot, "NjkzODk4Njg2NDQyMjQyMDgw.XoDyGA.VwRWijkG7zeGXB-6qg9hxd4MCu8");
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetActivityAsync(new Game("@Sharp", ActivityType.Listening));
            await _client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

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

        private ServiceProvider ConfigureServices() => new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
    }
}
