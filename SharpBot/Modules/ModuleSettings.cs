using System.ComponentModel;

namespace SharpBot.Modules {
    public class ModuleSettings {
        public Module Module { get; }
        public bool Enabled { get; }

        public ModuleSettings(Module module, bool enabled) {
            Module = module;
            Enabled = enabled;
        }
    }
}