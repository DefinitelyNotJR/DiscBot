using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Collections.Generic;
using SuccBot.Services;

namespace SuccBot.Modules.MentionByNickname
{
    public class NicknameMention : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }
        [Command("mention", RunMode = RunMode.Async)]
        public async Task MentionAsync([Remainder]string message)
        {
            var user = await PictureService.GetUser((SocketCommandContext)Context, message);
            await ReplyAsync($"You have been mentioned, {user.Mention}");
        }
    }
}