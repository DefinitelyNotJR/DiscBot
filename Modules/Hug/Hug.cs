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

        [Command("hug", RunMode = RunMode.Async)]
        public async Task HugAsync(SocketGuildUser user)
        {
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            string backgroundImagePath = "Modules/Hug/images/1.jpg";

            var backgroundImageStream = new FileStream(backgroundImagePath, FileMode.Open);

            backgroundImageStream.Seek(0, SeekOrigin.Begin);

            var avatarInputStream = await PictureService.GetPictureAsync(url);

            avatarInputStream.Seek(0, SeekOrigin.Begin);

            var avatarOutputStream = new MemoryStream();

            var outputStream = new MemoryStream();
            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(avatarInputStream))
            {

                using (Image<Rgba32> outputImage = image.Clone(x => x.ConvertToAvatar(new Size(200, 200), 100)))
                {
                    outputImage.SaveAsPng(avatarOutputStream);
                    avatarOutputStream.Seek(0, SeekOrigin.Begin);
                }
            }

            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(backgroundImageStream))
            {
                image.Mutate(x => x.ApplyScalingWaterMark(avatarOutputStream, 380, 475));
                image.SaveAsPng(outputStream);
            }
            try
            {
                outputStream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(outputStream, "hugged.png");
            }
            finally
            {
                if (outputStream != null)
                {
                    outputStream.Dispose();
                    avatarInputStream.Dispose();
                    avatarOutputStream.Dispose();
                    backgroundImageStream.Dispose();
                }
            }


        }
    }
}
