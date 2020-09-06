using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SharpBot.Punishments;
using SharpBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpBot.Command.commands.moderation {
    public class VoiceKickCommand : Command {

        private IServiceProvider _services;

        public VoiceKickCommand(IServiceProvider _services) : base("voicekick", CommandType.MODERATION) {
            this._services = _services;
        }

        public override async Task Execute(IUser user, SocketUserMessage message, string[] args) {
            if (args.Length <= 1) {
                await MessageUtil.InvalidCommandUsage(message.Channel, this, "<@User> [reason]");
                return;
            }

            var target = UserUtil.GetMemberFrom(message: message, args[1]);
            if (target == null) {
                await MessageUtil.SendWarning(message.Channel, $"Could not find a member with the input `{args[1]}`");
                return;
            }

            var punisher = target.Guild.GetUser(user.Id);
            if (punisher == null) return;

            if (target.Guild.Owner.Equals(target) || target.Hierarchy >= punisher.Hierarchy) {
                await MessageUtil.SendError(message.Channel, "This member has a higher hierarchy than you, therefore you can't voice-kick them.");
                return;
            }

            if (target.VoiceChannel == null) {
                await MessageUtil.SendError(message.Channel, "This member is not currently connected to any voice channel.");
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

            if (!Sharp.CheckPermissions(target.Guild, GuildPermission.MoveMembers, GuildPermission.ManageChannels)) {
                await MessageUtil.MissingPermissions(message.Channel, GuildPermission.MoveMembers, GuildPermission.ManageChannels);
                return;
            }

            var voiceChannel = await target.Guild.CreateVoiceChannelAsync("disconnect-member");

            await target.ModifyAsync(modify => modify.ChannelId = voiceChannel.Id).ContinueWith(x => {
                voiceChannel.DeleteAsync().ContinueWith(async voiceChannelDeleted => {
                    await target.SendMessageAsync($"📢 You have been kicked from the voice channel {socketVoice.Name}. Reason: `{reason}`").ContinueWith(async messageTask => {
                        await MessageUtil.SendSuccess(message.Channel, $"{target.Mention} has successfully been kicked from the voice channel 🔈 {socketVoice.Name}. Reason: `{reason}.`");

                        var punishment = new Punishment(PunishmentType.Voice_Kick, target, punisher, reason, -1);
                        _services.GetRequiredService<PunishmentManager>().InsertPunishment(punishment);
                    });
                });
            });
        }
    }
}
