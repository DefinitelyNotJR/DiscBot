using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SuccBot.Modules.AvaCrop
{
    public class AvaCrop : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("resize", RunMode = RunMode.Async)]
        public async Task AvaResizeAsync(SocketGuildUser user)
        {
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            var stream = await PictureService.GetPictureAsync(url);

            var outputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(stream))
            {
                image.Mutate(ctx => ctx.Resize(image.Width / 2, image.Height / 2));
                image.SaveAsPng(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(outputStream, "cropped.png");

        }
    }


}

