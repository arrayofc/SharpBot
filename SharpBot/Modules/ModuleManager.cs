using System.Collections.Generic;
using MongoDB.Bson;
using SharpBot.Database.Models;
using SharpBot.Modules.automod;
using SharpBot.Modules.welcomer;
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace SharpBot.Modules {
    public class ModuleManager {

        public static BsonDocument SerializeModules(GuildModel guild) {
            var modules = new BsonDocument();

            foreach (var module in Module.All) {
                
                switch (module.ModuleType) {
                    case Module.Type.Welcomer: {
                        if (!guild.Modules.ContainsKey(Module.WELCOMER)) {
                            guild.Modules.Add(Module.WELCOMER, WelcomerModuleSettings.NewDefault());
                        }
                    
                        modules.Add("welcomer", ((WelcomerModuleSettings) guild.Modules[module]).ToDocument());
                        break;
                    }
                    case Module.Type.AutoMod: {
                        if (!guild.Modules.ContainsKey(Module.AUTO_MOD)) {
                            guild.Modules.Add(Module.AUTO_MOD, WelcomerModuleSettings.NewDefault());
                        }
                    
                        modules.Add("auto-mod", ((AutoModModuleSettings) guild.Modules[module]).ToDocument());
                        break;
                    }
                }
            }

            return modules;
        }

        public static Dictionary<Module, ModuleSettings> DeserializeModules(BsonDocument document) {
            var modules = new Dictionary<Module, ModuleSettings>();
            
            foreach (var module in Module.All) {
                
                switch (module.ModuleType) {
                    case Module.Type.Welcomer:
                        modules.Add(Module.WELCOMER, !document.ContainsValue("welcomer")
                                ? WelcomerModuleSettings.NewDefault()
                                : WelcomerModuleSettings.FromDocument(document.GetValue("welcomer").ToBsonDocument()));
                        break;
                    
                    case Module.Type.AutoMod:
                        modules.Add(Module.AUTO_MOD, !document.ContainsValue("auto-mod")
                                ? AutoModModuleSettings.NewDefault()
                                : AutoModModuleSettings.FromDocument(document.GetValue("auto-mod").ToBsonDocument()));
                        break;
                }
            }

            return modules;
        }

        public static Dictionary<Module, ModuleSettings> NewDefault() {
            var modules = new Dictionary<Module, ModuleSettings>();

            foreach (var module in Module.All) {
                switch (module.ModuleType) {
                    case Module.Type.Welcomer:
                        modules.Add(Module.WELCOMER, WelcomerModuleSettings.NewDefault());
                        break;

                    case Module.Type.AutoMod:
                        modules.Add(Module.AUTO_MOD, AutoModModuleSettings.NewDefault());
                        break;
                }
            }

            return modules;
        }
    }
}