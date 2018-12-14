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

namespace SuccBot.Modules
{
    public class Hug : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("hug")]
        public async Task HugAsync(SocketGuildUser user)
        {
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            string backgroundImagePath = "Modules/Hug/images/1.jpg";

            var backgroundImagestream = new FileStream(backgroundImagePath, FileMode.Open);

            var avatarInputStream = await PictureService.GetPictureAsync(url);

            var avatarOutputStream = new MemoryStream();

            var imageOutputStream = new MemoryStream();

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(avatarInputStream))
            {

                using (Image<Rgba32> outputImage = image.Clone(x => x.ConvertToAvatar(new Size(200, 200), 100)))
                {
                    outputImage.SaveAsPng(avatarOutputStream);
                }
            }

           

            // outputStream.Seek(0, SeekOrigin.Begin);
            backgroundImagestream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(backgroundImagestream, "background.png");

            // await Context.Channel.SendFileAsync(outputStream, "croppedAvatar.png");

        }
    }


}