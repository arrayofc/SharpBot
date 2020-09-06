using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using SharpBot.Utility;

namespace SharpBot.Command.commands.utility {
    public class DogCommand : Command {

        private readonly ImageWebService _imageService;

        public DogCommand(IServiceProvider services) : base("dog", CommandType.UTILITY) {
            _imageService = services.GetRequiredService<ImageWebService>();
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            var stream = await _imageService.RequestImage(ImageType.Dog);
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                await message.Channel.SendFileAsync(stream, "dog.png");
            } else {
                await MessageUtil.SendError(message.Channel, "Sorry, no dog image was received. (error occurred)");
            }
        }
    }
}
