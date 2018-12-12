using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SuccBot.Modules.Music
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;

            if (channel is null)
            {
                await ReplyAsync("User must be in a voice channel for bot to work");
            }
            else
            {
                await channel.ConnectAsync();
            }

            var audioclient = await channel.ConnectAsync();
        }
    }
}