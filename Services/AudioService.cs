using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SuccBot_master.DataStructs;
using SuccBot_master.Handlers;
using SuccBot_master.Services;
using Victoria;
using Victoria.Entities;
using Victoria.Entities.Enums;
using System.Linq;
using System.Text;

namespace SuccBot.Services
{
    public class AudioService
    {
        private Lavalink _lavalink;
        private DiscordSocketClient _client;


        public AudioService(Lavalink lavalink, DiscordSocketClient client)
        {
            _lavalink = lavalink;
            _client = client;
        }
        private readonly Lazy<ConcurrentDictionary<ulong, AudioOptions>> _lazyOptions
            = new Lazy<ConcurrentDictionary<ulong, AudioOptions>>();

        private ConcurrentDictionary<ulong, AudioOptions> Options
            => _lazyOptions.Value;

        public async Task<Embed> JoinAsync(SocketGuildUser user, IMessageChannel textChannel, ulong guildId, string query = null)
        {
            if (user.VoiceChannel == null)
                return await EmbedHandler.CreateErrorEmbed("Music, Join command", "Please, join voice channel first.");

            await _lavalink.DefaultNode.ConnectAsync(user.VoiceChannel, textChannel);
            return await EmbedHandler.CreateBasicEmbed("Music", $"Bot is now connected to {user.VoiceChannel.Name} and ready to play music", "", Color.Green);
        }

        public async Task<Embed> PlayAsync(SocketGuildUser user, IMessageChannel textChannel, ulong guildId, string query = null)
        {
            var player = _lavalink.DefaultNode.GetPlayer(guildId);
            var search = await _lavalink.DefaultNode.SearchYouTubeAsync(query);
            var track = search.Tracks.FirstOrDefault();

            if (player.CurrentTrack != null && player.IsPlaying || player.IsPaused)
            {
                player.Queue.Enqueue(track);
                await LoggingService.LogInformationAsync("Music", $"{track.Title} has been added to the music queue.");
                return await EmbedHandler.CreateBasicEmbed("Music", $"{track.Title} has been added to queue.", "", Color.Blue);
            }

            await player.PlayAsync(track);
            await LoggingService.LogInformationAsync("Music", $"Bot is now playing: {track.Title}\n Url: {track.Uri}");
            return await EmbedHandler.CreateBasicEmbed("Music", $"Now Playing: {track.Title}\nUrl: {track.Uri}", "", Color.Blue);

        }

        public async Task SkipAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);
                if (player.Queue.Count == 1)
                {
                    await StopAsync(guildId);
                }
                else
                {
                    await player.SkipAsync();
                }
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        public async Task StopAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);
                await player.StopAsync();
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        public async Task<Embed> QueueAsync(ulong guildId)
        {
            var player = _lavalink.DefaultNode.GetPlayer(guildId);
            var descriptionBuilder = new StringBuilder();
            var trackNum = 1;
            foreach (var track in player.Queue.Items)
            {
                descriptionBuilder.Append($"{trackNum}: {track.Title} ({track.Uri})\n");
                trackNum++;
            }
            return await EmbedHandler.CreateBasicEmbed("Queue", $"{descriptionBuilder.ToString()}","", Color.Green);
        }

        public async Task<Embed> NowPlayingAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);
                return await EmbedHandler.CreateBasicEmbed("Music", $"{player.CurrentTrack.Title} is now playing", "", Color.Blue);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
                return await EmbedHandler.CreateErrorEmbed(ex.Source, ex.Message);
            }
        }

        public async Task VolumeAsync(ulong guildId, int volume)
        {
            try
            {
                if (volume < 0 || volume > 150)
                {
                    return;
                }
                var player = _lavalink.DefaultNode.GetPlayer(guildId);
                await player.SetVolumeAsync(volume);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }
        public async Task<Embed> LeaveAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);

                if (player.IsPlaying)
                    await player.StopAsync();

                var channelName = player.VoiceChannel.Name;
                await _lavalink.DefaultNode.DisconnectAsync(guildId);
                await LoggingService.LogInformationAsync("Music", $"Bot has left {channelName}.");
                return await EmbedHandler.CreateBasicEmbed("Music", $"Disconnected from {channelName} voice channel.", "", Color.Blue);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Leave", ex.ToString());
            }
        }
        public async Task OnFinished(LavaPlayer player, LavaTrack track, TrackReason reason)
        {
            if (reason is TrackReason.LoadFailed || reason is TrackReason.Cleanup)
                return;
            player.Queue.TryDequeue(out LavaTrack nextTrack);

            if (nextTrack is null)
            {
                await LoggingService.LogInformationAsync("Music", "Bot has stopped playback.");
                await player.StopAsync();
            }
            else
            {
                await player.PlayAsync(nextTrack);
                await LoggingService.LogInformationAsync("Music", $"Bot Now Playing: {nextTrack.Title} - {nextTrack.Uri}");
            }
        }
    }
}