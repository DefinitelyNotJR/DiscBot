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
        [Command("mention")]
        public async Task MentionAsync([Remainder]string nickname)
        {
            var users = Context.Guild.Users;
            List<IUser> usersList = new List<IUser>();
            foreach (IUser user in users)
            {
                if (user.Username.ToLower().Contains(nickname.ToLower())
                || user.Mention.ToLower().Contains(nickname.ToLower()))
                {
                    usersList.Add(user);
                }
            }
            var maybeTheRightUser = usersList.First();

            await ReplyAsync($"You have been mentioned, {maybeTheRightUser.Mention}");
        }

        // public async Task MentionAsync([Remainder]string arg)
        // {
        //     var users = Context.Guild.Users;
        //     List<SocketGuildUser> userList = new List<SocketGuildUser>();
        //     foreach (SocketGuildUser user in users)
        //     {
        //         if (user.Nickname.ToLower().Contains(arg.ToLower())
        //         || user.Username.ToLower().Contains(arg.ToLower())
        //         || user.Mention.ToLower().Contains(arg.ToLower()))
        //         {
        //             userList.Add(user);
        //         }
        //     }

        //     var userNeeded = userList.FirstOrDefault();

        //     await ReplyAsync($"You have been mentioned, {userNeeded.Mention}");
        // }
    }
}