using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Command.commands {
    public class VoiceKickCommand : Command {

        private IServiceProvider _services;

        public VoiceKickCommand(IServiceProvider _services) : base("voicekick") {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await message.Channel.SendMessageAsync($"Invalid command usage. Correct usage is `{Sharp.GetCommandPrefix(message)}voicekick <@User> [reason]`.");
                return;
            }

            var target = UserUtil.GetMemberFrom(message: message, args[1]);
            if (target == null) {
                await message.Channel.SendMessageAsync($"Could not find a member with the input `{args[1]}`.");
                return;
            }

            var punisher = target.Guild.GetUser(user.Id);
            if (punisher == null) return;

            if (target.Guild.Owner.Equals(target) || target.Hierarchy >= punisher.Hierarchy) {
                await message.Channel.SendMessageAsync($"This members has a higher hierarchy than you, therefore you can't kick them.");
                return;
            }

            if (target.VoiceChannel == null) {
                await message.Channel.SendMessageAsync($"This member is not currently connected to any voice channel.");
                return;
            }

            var reason = "";
            var argsList = new List<string>(args);
            if (args.Length > 1) {
                for (var i = 0; i <= 1; i++) { argsList.RemoveAt(0); }

                reason = string.Join(" ", argsList.ToArray());
            }

            reason = reason == "" ? "Not Specified" : reason;

            var socketVoice = target.VoiceChannel;

            var voiceChannel = await target.Guild.CreateVoiceChannelAsync("disconnect-member");

            await target.ModifyAsync(modify => modify.ChannelId = voiceChannel.Id).ContinueWith(x => {
                voiceChannel.DeleteAsync().ContinueWith(async voiceChannelDeleted => {
                    await target.SendMessageAsync($"📢 You have been kicked from the voice channel {socketVoice.Name}. Reason: `{reason}`").ContinueWith(async messageTask => {
                        await message.Channel.SendMessageAsync($"📢 {target.Mention} has been kicked from the voice channel {socketVoice.Name}. Reason: `{reason}`");

                        var punishment = new Punishment(PunishmentType.Voice_Kick, target, punisher, reason, 0);
                        _services.GetRequiredService<PunishmentManager>().InsertPunishment(punishment);
                    });
                });
            });
        }
    }
}
