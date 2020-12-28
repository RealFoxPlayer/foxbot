using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace FoxBot.Core.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {   
        //Permanent Ban
        [Command("ban")]
        [Alias("permban", "banuser", "permbanuser")]
        [Summary("Permanently Ban Someone (Admin Command)")]
        public async Task PermbanUser(SocketGuildUser userName, string reason, int deleteSince = 0)
        {
            if (UserIsAdmin((SocketGuildUser)Context.User))
            {
                await ReplyAsync($"{userName} has been banned for {reason}. \n All their messages from {deleteSince} days ago will be deleted.");
                var DMChannel = await userName.GetOrCreateDMChannelAsync();
                await DMChannel.SendMessageAsync($"You have been banned from Fox Player Discord for {reason}.");
                await userName.Guild.AddBanAsync(userName, deleteSince, $"Banned for {reason}.");
                Console.WriteLine($"{DateTime.Now}] {Context.Message.Author.Username} permbanned {userName}");
            }
            else
            {
                await ReplyAsync($"Hold on, {Context.User.Username}! You can't just permban someone without permission!");
            }
        }

        //Strike User
        [Command("strike"), Alias("strikeuser")]
        [Summary("Sends a warning to an user in DM")]
        private async Task WarnUser(SocketGuildUser user, string info)
        {
            if(UserIsAdmin((SocketGuildUser)Context.User))
            {
                if(!(info.Contains("\n") || info.Contains("\r") || info.Contains("&")))
                {
                    await ReplyAsync($"{user.Username} has been has been striked: '{info}'");
                    string s = File.ReadAllText(Directory.GetCurrentDirectory() + "\\strikes");
                    StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\strikes");
                    sw.WriteLine(s);
                    sw.WriteLine(user + "&" + Context.User + "&" + DateTime.UtcNow + "&" + info);
                    sw.Close();
                }
                else
                {
                    await ReplyAsync("Error, the info can't contain multiple lines and/or the character &");
                }
            }
            else
            {
                await ReplyAsync($"Hold on, {Context.User}! You can't just strike someone without permission!");
            }
        }


        //UserIsAdminBool
        private bool UserIsAdmin(SocketGuildUser user)
        {
            string targetRoleName = "moderators";
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }
    }
}
