using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace SuccBot.Services
{
    public class UserSearch
    {
        public async Task<SocketGuildUser> GetUser(SocketCommandContext context, string message)
        {
            var user = context.Message.MentionedUsers.FirstOrDefault();

            if (user == null)
            {
                var users = context.Guild.Users.ToList();
                List<SocketGuildUser> userList = new List<SocketGuildUser>();
                foreach (SocketGuildUser u in users)
                {
                    if (u.Username.ToLower().Contains(message.ToLower()) || ((u.Nickname ?? "").ToLower().Contains(message.ToLower())))

                    {
                        userList.Add(u);
                    }
                }

                var maybeTheRightUser = userList.First();
                return maybeTheRightUser;
            }
            else
            {
                return (SocketGuildUser)user;
            }
        }
    }
}