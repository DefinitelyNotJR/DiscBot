using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SuccBot_master.Handlers;

namespace SuccBot.Modules.AvaCrop
{
    public class AvaResize : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("resize", RunMode = RunMode.Async)]
        public async Task AvaResizeAsync(SocketGuildUser user)
        {
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            var inputStream = await PictureService.GetPictureAsync(url);

            var outputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(inputStream))
            {

                image.Mutate(ctx => ctx.Resize(image.Width / 2, image.Height / 2));
                image.SaveAsPng(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            // await Context.Channel.SendFileAsync(outputStream, "resizedAvatar.png");
            await Context.Channel.SendFileAsync(outputStream, "stream.png", embed:
            await EmbedHandler.CreateBasicEmbed($"{user.Username}'s resized avatar", "", "attachment://stream.png", Color.Green));
        }

    }


}

