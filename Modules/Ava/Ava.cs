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

        public async Task AvaAsync(SocketGuildUser user = null)
        {
            if (user == null)
            {
                user = (SocketGuildUser)Context.User;
            }
            // var builder = new EmbedBuilder()
            // {
            //     ImageUrl = user.GetAvatarUrl(ImageFormat.Auto, 256)
            // };
            // await Context.Channel.SendMessageAsync("", false, builder.Build());

            await Context.Channel.SendMessageAsync("", false, await EmbedHandler.CreateBasicEmbed($"{user.Username}'s avatar", "", user.GetAvatarUrl(ImageFormat.Auto, 512), Color.DarkOrange));
        }
    }
}