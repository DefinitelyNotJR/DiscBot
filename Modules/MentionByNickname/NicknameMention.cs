using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Collections.Generic;

namespace SuccBot.Modules.MentionByNickname
{
    public class NicknameMention : ModuleBase<SocketCommandContext>
    {
        [Command("mention", RunMode = RunMode.Async)]
        public async Task MentionAsync([Remainder]string message)
        {
            var user = Context.Message.MentionedUsers.FirstOrDefault();

            if (user == null)
            {
                var users = Context.Guild.Users;
                List<SocketGuildUser> userList = new List<SocketGuildUser>();
                foreach (SocketGuildUser u in users)
                {
                    if (u.Username.ToLower().Contains(message.ToLower()))
                    {
                        userList.Add(u);
                    }
                }
                var maybeTheRightUser = userList.First();

                await ReplyAsync($"You have been mentioned, {maybeTheRightUser.Mention}");
            }
            
            else
            {
                await ReplyAsync($"Ping ping {user.Mention}");
            }
        }
    }
}