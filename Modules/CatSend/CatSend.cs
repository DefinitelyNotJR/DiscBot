using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;

namespace SuccBot.Modules.CatSend
{
    public class CatSend : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("cat")]
        public async Task CatSenderAsync()
        {
            string url = "https://cataas.com/cat";

            var stream = await PictureService.GetPictureAsync(url);

            stream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(stream,"cat.png");

        }
    }


}
