using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace SharpBot.Configuration {
    public class ConfigManager {
        private Config Config { get; set; }

        public ConfigManager() {
            Config = null;

            LoadConfiguration();
        }

        /// <summary>
        /// Load the local bot configuration. If no configuration is found, it will attempt to create
        /// a configuration file with default values and exit the process.
        /// </summary>
        private void LoadConfiguration() {
            // No configuration file was found, let's create one and write the default JSON config to it.
            if (!File.Exists(Environment.CurrentDirectory + "\\" + "config.json")) {
                using var file = File.CreateText(Environment.CurrentDirectory + "\\" + "config.json");
                
                var serializer = new JsonSerializer();
                serializer.Serialize(file, new Config());
                    
                file.Flush();
                file.Close();

                Console.WriteLine("Notice: Configuration file did not exist. One has been created.");
                Environment.Exit(-1);
                return;
            }

            using var reader = new StreamReader(Environment.CurrentDirectory + "\\" + "config.json");
            
            var json = reader.ReadToEnd();
            Config = JsonConvert.DeserializeObject<Config>(json);
        }

        /// <summary>
        /// Returns the loaded configuration file.
        /// </summary>
        /// <returns>Loaded <c>Config</c> class.</returns>
        public Config GetConfig() {
            return Config;
        }
    }
}
