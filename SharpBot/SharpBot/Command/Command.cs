using System;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace SharpBot.Command {
    public abstract class Command {

        public string Invoke { get; }
        public bool GuildOnly { get; set; }
        public Nullable<GuildPermission> Permission { get; set; }

        public List<string> Aliases { get; set; }

        public Command(String invoke) {
            this.Invoke = invoke;
            this.Aliases = new List<string>();
            this.GuildOnly = false;
            this.Permission = null;

            CommandManager.Commands.Add(this);
        }

        public Command SetGuildOnly() {
            this.GuildOnly = true;
            return this;
        }

        public Command SetPermission(GuildPermission permission) {
            this.Permission = permission;
            return this;
        }

        public Command PutAliases(List<String> aliases) {
            this.Aliases = aliases;
            return this;
        }

        public string GetInvoke() {
            return this.Invoke;
        }

        public bool IsGuildOnly() {
            return this.GuildOnly;
        }

        public Nullable<GuildPermission> GetPermission() {
            return this.Permission;
        }

        public List<String> GetAliases() {
            return this.Aliases;
        }

        public abstract void Execute(IUser user, SocketUserMessage message, string[] args);
    }
}
