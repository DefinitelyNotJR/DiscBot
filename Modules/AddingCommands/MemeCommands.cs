using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using LiteDB;
using System.Linq;
using System;
using SuccBot_master.Handlers;

namespace SuccBot.Modules.AddingCommands
{
    public class CommandDb
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string CommandName { get; set; }
        public string CommandBody { get; set; }
    }

    public class MemeCommands : ModuleBase
    {
        private LiteDatabase _database;
        public MemeCommands(LiteDatabase database)
        {
            _database = database;
        }

        [Command("!", RunMode = RunMode.Async)]
        public async Task AddCommandAsync(string commandName, [Remainder]string commandBody)
        {
            try
            {
                var db = _database.GetCollection<CommandDb>("commands");
                var results = db.Find(x => (x.GuildId == Context.Guild.Id) && (x.CommandName.Contains(commandName)));
                if (!(results == null))
                {
                    await ReplyAsync("", false, await EmbedHandler.CreateErrorEmbed("AddCommand", "A command with this name already exists."));
                }
                else
                {
                    db.Insert(new CommandDb
                    {
                        GuildId = Context.Guild.Id,
                        CommandName = commandName,
                        CommandBody = commandBody
                    });

                    await ReplyAsync($"A command *{commandName.ToUpper()}* has been added.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("!!", RunMode = RunMode.Async)]
        public async Task SayCommandAsync(string commandName)
        {
            try
            {
                string commandMessage = "";

                var db = _database.GetCollection<CommandDb>("commands");

                var results = db.Find(x => (x.GuildId == Context.Guild.Id) && (x.CommandName.Contains(commandName)));

                var result = results.FirstOrDefault();

                commandMessage = result.CommandBody;

                await ReplyAsync(commandMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("delcommand", RunMode = RunMode.Async)]
        public async Task DeleteCommandAsync(string commandName)
        {
            try
            {
                var db = _database.GetCollection<CommandDb>("commands");

                var results = db.Find(x => (x.GuildId == Context.Guild.Id) && (x.CommandName.Contains(commandName)));

                var result = results.FirstOrDefault();

                db.Delete(result.Id);

                await ReplyAsync($"The command {commandName} has been deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
