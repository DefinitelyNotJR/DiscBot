using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuccBot_master.DataStructs;

namespace SuccBot_master.Handlers
{
    public class Global
    {
        public static string ConfigPath { get; set; } = "config.json";

        public static BotConfig Config { get; set; }

        public async Task Initialize()
        {
            var json = string.Empty;

            if (!File.Exists(ConfigPath))
            {
                json = JsonConvert.SerializeObject(GenerateNewConfig(), Formatting.Indented);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                Console.WriteLine("New config file has been created. Restart your bot");
                await Task.Delay(-1);
            }

            json = File.ReadAllText(ConfigPath, new UTF8Encoding(false));
            Config = JsonConvert.DeserializeObject<BotConfig>(json);
        }
        private static BotConfig GenerateNewConfig() => new BotConfig
        {
            DiscordToken = "",
            BotPrefix = "!",
            GameStatus = "with my butt"
        };

    }



}