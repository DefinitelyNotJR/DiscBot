using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;

namespace SuccBot.Modules.AudioModule
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public AudioService AudioService { get; set; }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinAsync()
        {
            await ReplyAsync("", false, await AudioService.JoinAsync((SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id));
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayAsync([Remainder]string query)
        {
            await ReplyAsync("", false, await AudioService.PlayAsync((SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id, query));
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveAsync()
        {
            await ReplyAsync("", false, await AudioService.LeaveAsync(Context.Guild.Id));
        }

        [Command("skip", RunMode = RunMode.Async)]
        public async Task SkipAsync()
        {
            await AudioService.SkipAsync(Context.Guild.Id);
        }

        [Command("stop", RunMode = RunMode.Async)]
        public async Task StopAsync()
        {
            await AudioService.StopAsync(Context.Guild.Id);
        }

        [Command("volume", RunMode = RunMode.Async)]
        public async Task VolumeAsync(int volume)
        {
            await AudioService.VolumeAsync(Context.Guild.Id, volume);
        }

        [Command("np", RunMode = RunMode.Async)]
        public async Task NowPlayingAsync()
        {
            await ReplyAsync("", false, await AudioService.NowPlayingAsync(Context.Guild.Id));
        }
    }
}