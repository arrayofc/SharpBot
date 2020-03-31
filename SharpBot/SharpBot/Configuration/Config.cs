using System;
using System.Collections.Generic;
using System.Text;

namespace SharpBot.Configuration {
    public class Config {

        public string BotToken { get; set; }
        public char BotPrefix { get; set; }

        public Config() {
            this.BotToken = "";
            this.BotPrefix = '!';
        }
    }
}
