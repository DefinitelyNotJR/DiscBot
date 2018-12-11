using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SuccBot.Modules
{


    public class Ava : ModuleBase<SocketCommandContext>
    {
        [Command("ava")]

        public async Task AvaAsync(SocketGuildUser user)
        {
            var builder = new EmbedBuilder()
            {
                ImageUrl = user.GetAvatarUrl(ImageFormat.Auto, 256)
            };
            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}