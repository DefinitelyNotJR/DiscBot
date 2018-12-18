using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuccBot.Services;

namespace SuccBot.Modules.Music
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot
        public AudioModule(AudioService service)
        {
            _service = service;
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song = "Modules/Music/Songs/1.mp3")
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        // [Command("join", RunMode = RunMode.Async)]
        // public async Task JoinChannel(IVoiceChannel channel = null)
        // {
        //     channel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;

        //     if (channel is null)
        //     {
        //         await ReplyAsync("User must be in a voice channel for bot to work");
        //     }


        //     var audioclient = await channel.ConnectAsync();
        // }
    }
}