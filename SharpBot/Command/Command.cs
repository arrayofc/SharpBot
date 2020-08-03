using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace SharpBot.Command {
    public abstract class Command {

        public string Invoke { get; }
        public bool GuildOnly { get; set; }
        public GuildPermission? Permission { get; set; }

        public List<string> Aliases { get; set; }

        public Command(string invoke) {
            Invoke = invoke;
            Aliases = new List<string>();
            GuildOnly = false;
            Permission = null;

            CommandManager.Commands.Add(this);
        }

        public Command SetGuildOnly() {
            GuildOnly = true;
            return this;
        }

        public Command SetPermission(GuildPermission permission) {
            Permission = permission;
            return this;
        }

        public Command PutAliases(List<string> aliases) {
            Aliases = aliases;
            return this;
        }

        public string GetInvoke() {
            return Invoke;
        }

        public bool IsGuildOnly() {
            return GuildOnly;
        }

        public GuildPermission? GetPermission() {
            return Permission;
        }

        public List<string> GetAliases() {
            return Aliases;
        }

        public abstract Task Execute(IUser user, SocketUserMessage message, string[] args);
    }
}
