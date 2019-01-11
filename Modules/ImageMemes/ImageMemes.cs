using System.IO;
using System.Threading.Tasks;
using Discord.Commands;

namespace SuccBot.Modules.ImageMemes
{
    public class ImageMemes : ModuleBase
    {
        [Command("adc")]
        public async Task AdcAsync()
        {
            await Context.Channel.SendFileAsync("Modules/ImageMemes/images/adc.jpg");
        }
    }
}