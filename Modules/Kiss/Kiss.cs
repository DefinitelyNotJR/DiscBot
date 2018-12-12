using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SuccBot.Modules
{
    public class Kiss : ModuleBase<SocketCommandContext>
    {
        [Command("kiss")]
        public async Task KissAsync()
        {
            
            await ReplyAsync($":* {Context.User.Mention}");
        }
    }

    
}