using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SuccBot_master.DataStructs;
using SuccBot_master.Handlers;
using SuccBot_master.Services;
using Victoria;

namespace SuccBot.Services
{
    public class AudioService
    {
        private Lavalink _lavalink;

        private DiscordSocketClient _client;
        public LavaNode node { get; set; }
        public LavaPlayer player { get; set; }

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
            if (node == null)
            {
                var node = await _lavalink.AddNodeAsync(_client, new Configuration
                {
                    Severity = LogSeverity.Info
                });
            }
            await _lavalink.DefaultNode.ConnectAsync(user.VoiceChannel, textChannel);
            return await EmbedHandler.CreateBasicEmbed("Music", $"Bot is now connected to {user.VoiceChannel.Name} and ready to play music", "", Color.Green);
        }

        public async Task<Embed> LeaveAsync(ulong guildId)
        {
            try
            {
                var player = _lavalink.DefaultNode.GetPlayer(guildId);

                if (player.IsPlaying)
                    await player.StopAsync();

                //Leave the voice channel.
                var channelName = player.VoiceChannel.Name;
                await _lavalink.DefaultNode.DisconnectAsync(guildId);
                await LoggingService.LogInformationAsync("Music", $"Bot has left {channelName}.");
                return await EmbedHandler.CreateBasicEmbed("Music", $"I've left {channelName}. Thank you for playing moosik.", "", Color.Blue);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.CreateErrorEmbed("Music, Leave", ex.ToString());
            }
        }
    }
}