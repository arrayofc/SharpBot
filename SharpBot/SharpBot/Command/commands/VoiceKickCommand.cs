using Discord;
using Discord.Rest;
using Discord.WebSocket;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBot.Command.commands {
    public class VoiceKickCommand : Command {
        public VoiceKickCommand(IServiceProvider _services) : base("voicekick") {
        }
        public override async void Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await message.Channel.SendMessageAsync($"Invalid commad usage. Correct usage is `{Sharp.GetCommandPrefix()}voicekick <@User> [reason]`.");
                return;
            }

            SocketGuildUser target = UserUtil.GetMemberFrom(message: message, args[1]);
            if (target == null) {
                await message.Channel.SendMessageAsync($"Could not find a member with the input `{args[1]}`.");
                return;
            }

            SocketGuildUser punisher = target.Guild.GetUser(user.Id);
            if (punisher == null) return;

            if (target.Guild.Owner.Equals(target) || target.Hierarchy >= punisher.Hierarchy) {
                await message.Channel.SendMessageAsync($"This members has a higher hierarchy than you, therefore you can't kick them.");
                return;
            }

            if (target.VoiceChannel == null) {
                await message.Channel.SendMessageAsync($"This member is not currently connected to any voice channel.");
                return;
            }

            string reason = "";
            if (args.Length > 1) {
                List<string> argsList = new List<string>(args);

                for (int i = 0; i <= 1; i++) { argsList.RemoveAt(0); }

                reason = String.Join(" ", argsList.ToArray());
            }

            reason = reason == "" ? "Not Specified" : reason;

            SocketVoiceChannel socketVoice = target.VoiceChannel;

            RestVoiceChannel voiceChannel = await target.Guild.CreateVoiceChannelAsync("disconnect-member");

            await target.ModifyAsync(modify => modify.ChannelId = voiceChannel.Id).ContinueWith(x => {
                voiceChannel.DeleteAsync().ContinueWith(async task => {
                    await target.SendMessageAsync($"📢 You have been kicked from the voice channel {socketVoice.Name}. Reason: `{reason}`").ContinueWith(async task => {
                        await message.Channel.SendMessageAsync($"📢 {target.Mention} has been kicked from the voice channel {socketVoice.Name}. Reason: `{reason}`");

                        Punishment punishment = new Punishment(PunishmentType.VOICE_KICK, target, punisher, reason, 0);
                    });
                });
            });
        }
    }
}
