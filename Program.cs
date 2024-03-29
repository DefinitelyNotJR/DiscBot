﻿using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using SuccBot.Services;
using System.Net.Http;
using Victoria;
using SuccBot_master.Handlers;
using SuccBot_master.Services;
using LiteDB;
using System.Linq;

namespace SuccBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();


        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private Lavalink _lavalink;
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            var global = new Global().Initialize();
            _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<HttpClient>()
            .AddSingleton<Lavalink>()
            .AddSingleton<AudioService>()
            .AddSingleton<PictureService>()
            .AddSingleton(new LiteDatabase("CommandsData.db"))
            .BuildServiceProvider();

            _lavalink = _services.GetRequiredService<Lavalink>();

            await HookEvents();

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Global.Config.DiscordToken);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task OnReadyAsync()
        {
            try
            {
                var node = await _lavalink.AddNodeAsync(_client, new Configuration
                {
                    Severity = LogSeverity.Info
                });
                node.TrackFinished += _services.GetService<AudioService>().OnFinished;
                await _client.SetGameAsync(Global.Config.GameStatus);
            }
            catch (Exception ex)
            {
                await LoggingService.LogInformationAsync(ex.Source, ex.Message);
            }
        }

        private async Task LogAsync(LogMessage logMessage)
        {
            await LoggingService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        public async Task HookEvents()
        {
            _client.Log += LogAsync;
            _services.GetRequiredService<CommandService>().Log += LogAsync;
            _lavalink.Log += LogAsync;
            _client.Ready += OnReadyAsync;
            _client.UserJoined += OnUserJoined;
            _client.UserUpdated += OnUserUpdated;
        }

        private async Task OnUserUpdated(SocketUser arg1, SocketUser arg2)
        {

            var guild = _client.GetGuild(136520569297436672);
            var logsChannel = _client.GetChannel(537254254209925141) as SocketTextChannel;
            if (!(arg1.Username == arg2.Username))
                await logsChannel.SendMessageAsync($"The user previously known as **{arg1.Username}** changed his username to **{arg2.Username}**  (user Id: {arg1.Id}");
        }

        private async Task OnUserJoined(SocketGuildUser guildUser)
        {
            var guilds = guildUser.Guild;
            var channels = guilds.Channels;
            var channelList = channels.ToList();
            var channel = (channelList.Find(x => x.Name == "logs")) as SocketTextChannel;
            await channel.SendMessageAsync($"A user has joined: **{guildUser.Username}** (user Id: **{guildUser.Id}**)");
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix(Global.Config.BotPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
