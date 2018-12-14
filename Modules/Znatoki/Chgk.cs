using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace SuccBot.Modules
{
    public class Chgk : ModuleBase<SocketCommandContext>
    {
        [Command("chgk")]

        public async Task ChgkAsyng()
        {
            Random random = new Random();
            var files = Directory.GetFiles("modules/znatoki/images", "*.png");
            string znatokToPost = files[random.Next(files.Length)];
            await Context.Channel.SendFileAsync(znatokToPost);
        }
    }
}