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

namespace SuccBot.Modules.AvaCrop
{
    
   public class Avacrop : ModuleBase<SocketCommandContext>
    {
        
        public PictureService PictureService { get; set; }

        [Command("crop", RunMode = RunMode.Async)]
        public async Task AvacropAsync(SocketGuildUser user)
        {
            string url = user.GetAvatarUrl(ImageFormat.Auto, 512).ToString();

            var inputStream = await PictureService.GetPictureAsync(url);

            var outputStream = new MemoryStream();



            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(inputStream))
            {

                using (Image<Rgba32> outputImage = image.Clone(x => x.ConvertToAvatar(new Size(200,200),100)))
                {
                    outputImage.SaveAsPng(outputStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            await Context.Channel.SendFileAsync(outputStream, "croppedAvatar.png");

        }

    }


}

