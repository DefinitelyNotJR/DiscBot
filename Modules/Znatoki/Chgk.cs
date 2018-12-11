using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace SuccBot.Modules
{
    public class Chgk : ModuleBase<SocketCommandContext>
    {
        string[] chgkMemes = new string[]
                    {
                "modules/znatoki/images/img1.PNG",
                "modules/znatoki/images/img2.PNG",
                "modules/znatoki/images/img3.PNG",
                "modules/znatoki/images/img4.PNG",
                "modules/znatoki/images/img5.PNG",
                "modules/znatoki/images/img6.PNG",
                "modules/znatoki/images/img7.PNG",
                "modules/znatoki/images/img8.PNG",
                "modules/znatoki/images/img9.PNG"
                                    };

        [Command("chgk")]

        public async Task ChgkAsyng()
        {
            Random random = new Random();
            string znatokToPost = chgkMemes[random.Next(chgkMemes.Length)];
            await Context.Channel.SendFileAsync(znatokToPost);
        }
    }
}