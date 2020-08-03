namespace SharpBot.Database.Models {
    public class GuildSettingsModel {

        public char CommandPrefix { get; set; }
        public ulong ModerationLogChannel { get; set; }
        
        public GuildSettingsModel() : this('!', 0) { }

        private GuildSettingsModel(char prefix, ulong modLog) {
            CommandPrefix = prefix;
            ModerationLogChannel = modLog;
        }

        public bool ModLogEnabled() {
            return ModerationLogChannel > 0;
        }
    }
}
