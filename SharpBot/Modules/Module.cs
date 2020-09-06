using System.Collections.Generic;

namespace SharpBot.Modules {
    public class Module {
        public static readonly List<Module> All = new List<Module>();

        public enum Type { Welcomer, AutoMod }
        
        public static readonly Module WELCOMER = new Module(Type.Welcomer, "Welcomer", "Welcomes new members when joining.");
        public static readonly Module AUTO_MOD = new Module(Type.AutoMod, "Auto Moderation", "Attempts to auto-moderate the server.");

        public string Name { get; }
        public string Description { get; }
        public Type ModuleType { get; }

        private Module(Type type, string name, string description) {
            ModuleType = type;
            Name = name;
            Description = description;
            
            All.Add(this);
        }
    }
}