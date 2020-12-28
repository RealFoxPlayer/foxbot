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
    class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        [Command("notices")]
        public async Task SendMessageInNotices(string message)
        {
            if(UserIsOwner((SocketGuildUser)Context.User))
            {
                await Program.notices.SendMessageAsync(message);
            }
        }

        [Command("notifications")]
        public async Task SendMessageInNotifications(string message)
        {
            if (UserIsOwner((SocketGuildUser)Context.User))
            {
                await Program.video_notifications.SendMessageAsync(message);
            }
        }

        private bool UserIsOwner(SocketGuildUser user)
        {
            string targetRoleName = "owner";
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
