using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using SuccBot_master.Handlers;

namespace SuccBot.Modules
{
    public class Hug : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("hug", RunMode = RunMode.Async)]
        public async Task HugAsync([Remainder]string message = null)
        {
            Stream stream = null;
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);
            stream = await PictureService.EditedImageAsync(user, "hug", 380, 475);

            await Context.Channel.SendFileAsync(stream, "stream.png", embed:
            await EmbedHandler.CreateBasicEmbed($"{user.Username} has been hugged by a bear", "", "attachment://stream.png", Color.DarkOrange));
        }

        [Command("gorilla", RunMode = RunMode.Async)]
        public async Task GorillaAsync([Remainder]string message = null)
        {
            Stream stream = null;
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);
            stream = await PictureService.EditedImageAsync(user, "gorilla", 240, 450);

            await Context.Channel.SendFileAsync(stream, "stream.png", embed:
            await EmbedHandler.CreateBasicEmbed($"{user.Username} is a gorilla now", "", "attachment://stream.png", Color.DarkOrange));
        }
    }
}

