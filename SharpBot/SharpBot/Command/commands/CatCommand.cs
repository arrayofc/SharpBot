using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SharpBot.Command.commands {
    public class CatCommand : Command {

        private PictureService _pictures;

        public CatCommand(IServiceProvider services) : base("cat") {
            _pictures = services.GetRequiredService<PictureService>();
        }

        public override async void Execute(IUser user, SocketUserMessage message, string[] args) {
            var stream = await _pictures.GetCatPictureAsync();
            if (stream != null) {
                stream.Seek(0, SeekOrigin.Begin);
                await message.Channel.SendFileAsync(stream, "cat.png");
            } else {
                await message.Channel.SendMessageAsync("Sorry, couldn't get a cat picture. (error occurred)");
            }

            return;
        }
    }
}
