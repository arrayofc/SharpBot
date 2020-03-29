using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;

namespace SharpBot.Modules {
    public class PublicModule : ModuleBase<SocketCommandContext> {

        private readonly PictureService _pictures;
        private readonly IServiceProvider _services;

        public PublicModule(IServiceProvider services) {
            _pictures = services.GetRequiredService<PictureService>();
            _services = services;
        }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync() =>
            ReplyAsync("Pong!");

        [Command("cat")]
        [Alias("kitten")]
        public async Task CatAsync() {
            var stream = await _pictures.GetCatPictureAsync();
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "cat.png");
            } else {
                await Context.Channel.SendMessageAsync("Sorry, couldn't get a cat picture. (error occurred)");
            }
            return;
        }

        [Command("dog")]
        [Alias("doggo")]
        public async Task DogAsync() {
            var stream = await _pictures.GetRandomDogPicture();
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "dog.png");
            } else {
                await Context.Channel.SendMessageAsync("Sorry, couldn't get a dog picture. (error occurred)");
            }
        }
    }
}
