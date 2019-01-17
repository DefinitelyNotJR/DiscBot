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
    public class Ava : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("ava")]
        public async Task AvaAsync([Remainder]string message = null)
        {
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);
            await Context.Channel.SendMessageAsync("", false, await EmbedHandler.CreateBasicEmbed($"{user.Username}'s avatar", "", user.GetAvatarUrl(ImageFormat.Auto, 512), Color.DarkOrange));
        }

        [Command("crop", RunMode = RunMode.Async)]
        public async Task AvacropAsync([Remainder]string message = null)
        {
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);

            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            var inputStream = await PictureService.GetPictureAsync(url);

            var outputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(inputStream))
            {

                using (Image<Rgba32> outputImage = image.Clone(x => x.ConvertToAvatar(new Size(200, 200), 100)))
                {
                    outputImage.SaveAsPng(outputStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(outputStream, "stream.png", embed:
            await EmbedHandler.CreateBasicEmbed($"{user.Username}'s cropped avatar", "", "attachment://stream.png", Color.DarkOrange));
        }

        [Command("resize", RunMode = RunMode.Async)]
        public async Task AvaResizeAsync([Remainder]string message = null)
        {
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);

            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            var inputStream = await PictureService.GetPictureAsync(url);

            var outputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(inputStream))
            {

                image.Mutate(ctx => ctx.Resize(image.Width / 2, image.Height / 2));
                image.SaveAsPng(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(outputStream, "stream.png", embed:
            await EmbedHandler.CreateBasicEmbed($"{user.Username}'s resized avatar", "", "attachment://stream.png", Color.Green));
        }

        [Command("cat")]
        public async Task CatSenderAsync()
        {
            string url = "https://cataas.com/cat";

            var stream = await PictureService.GetPictureAsync(url);

            stream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(stream, "cat.png", embed:
            await EmbedHandler.CreateBasicEmbed($"A cute cat!", "", "attachment://stream.png", Color.Green));
        }
    }
}