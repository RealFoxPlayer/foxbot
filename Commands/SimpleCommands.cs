using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace FoxBot.Core.Commands
{
    public class SimpleCommands : ModuleBase<SocketCommandContext>
    {
        //Help Command
        [Command("help"), Alias("helpme", "mehelp")]
        [Summary("Displays a list of commands")]
        public async Task HelpCommand()
        {
            await ReplyAsync("Community Commands:\n-!help\n-!warmup @user\n1 Easter Egg Command");
        }
        
        //Hewwo Command
        [Command("hewwo")]
        [Summary("Secret Easter Egg Command")]
        public async Task OwO()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            await ReplyAsync(
                $"You fownd an eastew egg, OwO!");
        }

        //Warmup Command
        [Command("warmup")]
        [Summary("It's always nice to be warmed up!")]
        public async Task WarmupCommand(SocketGuildUser user)
        {
            await ReplyAsync($"{Context.Message.Author.Mention} warmed {user.Mention} up!");
        }

        //Strikes Command
        [Command("strikes")]
        [Summary("Get a person's strikes")]
        public async Task StrikesCommand(SocketGuildUser user = null)
        {
            if (user == null) user = Context.Guild.CurrentUser;
            Console.WriteLine("Strikes " + user);
            string strikes = "";

            string[] l = File.ReadAllText(Directory.GetCurrentDirectory() + "\\strikes").Split(char.Parse("\n"));
            foreach(string s in l)
            {
                string[] parts = s.Split(char.Parse("&"));
                string _s;
                if (parts[0] == user.ToString())
                {
                    _s = "[" + parts[2] + "] by " + parts[1] + " info: \"" + parts[3] + "\"";
                    _s = _s.Replace("\n", "");
                    _s = _s.Replace("\r", "");
                    _s = "\n" + _s;
                    strikes += _s;
                }
            }

            if (strikes == "") strikes = "None";

            await ReplyAsync(user.Mention + "'s strikes:" + strikes);
        }
    }
}
