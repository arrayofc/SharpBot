using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;

namespace SharpBot.Configuration {
    public class ConfigManager {
        private Config Config { get; set; }

        public ConfigManager() {
            this.Config = null;

            this.LoadConfiguration();
        }

        private void LoadConfiguration() {
            if (!(File.Exists((Environment.CurrentDirectory + "\\" + "config.json")))) {
                using (StreamWriter file = File.CreateText(Environment.CurrentDirectory + "\\" + "config.json")) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, new Config());
                    file.Flush();
                    file.Close();

                    Console.WriteLine("Notice: Configuration file did not exist. One has been created.");
                    Environment.Exit(-1);
                }
                return;
            }

            using (StreamReader reader = new StreamReader(Environment.CurrentDirectory + "\\" + "config.json")) {
                string json = reader.ReadToEnd();
                this.Config = JsonConvert.DeserializeObject<Config>(json);
            }
        }

        public Config getConfig() {
            return this.Config;
        }
    }
}
