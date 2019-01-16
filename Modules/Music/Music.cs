using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;
using Victoria;
using Victoria.Entities;

namespace SuccBot.Modules.Music
{
    public class AudioModule : ModuleBase<ICommandContext>
    {

        public DiscordSocketClient DiscordSocketClient { get; set; }

        private Lavalink _lavalink;
        public AudioModule(Lavalink lavalink)
        {
            _lavalink = lavalink;
        }

        [Command("hehe", RunMode = RunMode.Async)]

        public async Task JoinAsync()
        {
            // await ReplyAsync("", false, await AudioService.JoinOrPlayAsync((SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id));
            var user = (SocketGuildUser)Context.User;
            var node = await _lavalink.AddNodeAsync(DiscordSocketClient, new Configuration
            {
                Severity = LogSeverity.Info
            });
            await node.ConnectAsync(user.VoiceChannel);
        }

        [Command("heheh", RunMode = RunMode.Async)]

        public async Task PlayAsync([Remainder] string song)
        {
            LavaTrack track;
            var search = await _lavalink.DefaultNode.SearchYouTubeAsync(song);
            track = search.Tracks.FirstOrDefault();
            var player = _lavalink.DefaultNode.GetPlayer(Context.Guild.Id);
            await player.PlayAsync(track);
        }
        // => await ReplyAsync("", false, await AudioService.ConnectAndPlayAsync(channel);

        // (SocketGuildUser)Context.User, Context.Channel, Context.Guild.Id)
    }
}