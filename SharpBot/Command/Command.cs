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
        
        public string Description { get; set; }
        
        public CommandType Type { get; set; }

        public Command(string invoke, CommandType type) {
            Invoke = invoke;
            Aliases = new List<string>();
            GuildOnly = false;
            Permission = null;
            Description = "";
            Type = type;

            CommandManager.Commands.Add(this);
        }

        /// <summary>
        /// Makes this command executable in guilds only.
        /// </summary>
        public Command SetGuildOnly() {
            GuildOnly = true;
            return this;
        }

        /// <summary>
        /// Set the permission for this command. May be null.
        /// </summary>
        public Command SetPermission(GuildPermission permission) {
            Permission = permission;
            return this;
        }

        /// <summary>
        /// Set the aliases for this command.
        /// </summary>
        public Command PutAliases(List<string> aliases) {
            Aliases = aliases;
            return this;
        }
        
        /// <summary>
        /// Set the description of this command.
        /// </summary>
        public Command SetDescription(string description) {
            Description = description;
            return this;
        }

        /// <summary>
        /// Get the invoke for this command.
        /// </summary>
        public string GetInvoke() {
            return Invoke;
        }

        /// <summary>
        /// See if this command is executable in guilds only.
        /// </summary>
        public bool IsGuildOnly() {
            return GuildOnly;
        }

        /// <summary>
        /// Get the permission for this command.
        /// </summary>
        /// <returns>Null if none.</returns>
        public GuildPermission? GetPermission() {
            return Permission;
        }

        /// <summary>
        /// Get the aliases for this command.
        /// </summary>
        public List<string> GetAliases() {
            return Aliases;
        }
        
        /// <summary>
        /// Get the description of this command.
        /// </summary>
        public string GetDescription() {
            return Description;
        }

        /// <summary>
        /// Get the command type.
        /// </summary>
        public CommandType GetType() {
            return Type;
        }

        public abstract Task Execute(IUser user, SocketUserMessage message, string[] args);
    }
}
