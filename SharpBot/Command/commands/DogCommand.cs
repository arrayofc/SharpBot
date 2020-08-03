using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SharpBot.Command.commands {
    public class DogCommand : Command {

        private readonly ImageWebService _imageService;

        public DogCommand(IServiceProvider services) : base("dog") {
            _imageService = services.GetRequiredService<ImageWebService>();
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var stream = await _imageService.RequestImage(ImageType.Dog);
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                await message.Channel.SendFileAsync(stream, "dog.png");
            } else {
                await message.Channel.SendMessageAsync("Sorry, no dog cat image was received. (error occurred)");
            }
        }
    }
}
