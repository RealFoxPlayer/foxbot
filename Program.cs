using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace FoxBot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        public static SocketGuild guild;
        public static SocketTextChannel video_notifications;
        public static SocketTextChannel notices;
        public static SocketTextChannel gamesuggestions;
        public static SocketTextChannel chat;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Console.Title = "FoxBot (Fox Player Discord)";
            Console.WriteLine($"{DateTime.Now} FoxBot Is Launched");

            if (!File.Exists(Directory.GetCurrentDirectory() + "\\strikes")) File.Create(Directory.GetCurrentDirectory() + "\\strikes");

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            client.MessageReceived += client_MessageReceived;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            client.Ready += client_Ready;
            client.Log += client_Log;

            await client.LoginAsync(TokenType.Bot, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            await client.StartAsync();

            ConsoleInput();
            await Task.Delay(-1);
        }

        private async Task ConsoleInput()
        {
            var input = string.Empty;
            var first = true;
            while (true)
            {
                if (first)
                {
                    first = false;
                    Console.Title = "FoxBot (Press enter to set up)";
                    Console.ReadLine();
                    Console.Title = "FoxBot (Setting up)";
                    ConfigureStuff();
                }

                Console.WriteLine("Enter command:");
                input = Console.ReadLine();

                if (input.Trim().ToLower() == "help")
                    ConsoleHelp();

                if (input.Trim().ToLower() == "sendmessage")
                    ConsoleSendMessage();

                if (input.Trim().ToLower() == "notify")
                    ConsoleNotify();
            }
        }

        private void ConfigureStuff()
        {
            Console.WriteLine("Select a server:");
            guild = GetSelectedGuild(client.Guilds);
            Console.WriteLine("\nSelect the video_notifications channel:");
            video_notifications = GetSelectedTextChannel(guild.TextChannels);
            Console.WriteLine("\nSelect the notices channel:");
            notices = GetSelectedTextChannel(guild.TextChannels);
            Console.WriteLine("\nSelect the chat channel:");
            chat = GetSelectedTextChannel(guild.TextChannels);
            Console.WriteLine("\nSelect the gamesuggestions channel:");
            gamesuggestions = GetSelectedTextChannel(guild.TextChannels);
            Console.WriteLine("Configuration Complete!\n\n");
            Console.Title = $"FoxBot ({guild.Name})";
        }

        private void ConsoleHelp()
        {
            Console.WriteLine("\nConsole Commands:\n-help (Command List)\n-notify (Add Notification)\n-sendmessage (Send Message In #chat Through FoxBot)\n\nChat Commands\n-!hewwo (Easter Egg)\n-!ban (Ban-Commands List)\n-!permban @username#id string[reason] int[deleteallmessagessincedays] (Permanently Ban Someone)\n-!tempban @username#id string[reason] int[sentencelenghtindays] (Temporarily Ban Someone For A Specified Amount Of Days)\n");
        }

        private void ConsoleSendMessage()
        {
            Console.WriteLine("Send message in #chat:");
            var message = Console.ReadLine();

            if(message.Trim() != null && message.Trim() != string.Empty)
            {
                chat.SendMessageAsync($"{message}");
            }
        }

        private void ConsoleNotify()
        {
            Console.WriteLine("Enter notification message:");
            var message = Console.ReadLine();

            if (message.Trim() != null && message.Trim() != string.Empty)
            {
                video_notifications.SendMessageAsync($"{message}");
            }
        }

        private SocketTextChannel GetSelectedTextChannel(IEnumerable<SocketTextChannel> channels)
        {
            var textChannels = channels.ToList();
            var maxIndex = textChannels.Count - 1;
            for (var i = 0; i <= maxIndex; i++)
            {
                Console.WriteLine($"{i} - {textChannels[i].Name}");
            }

            var selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                var success = int.TryParse(Console.ReadLine().Trim(), out selectedIndex);
                if (!success)
                {
                    Console.WriteLine("That was an invalid index, try again.");
                    selectedIndex = -1;
                }
                else if (selectedIndex < 0 || selectedIndex > maxIndex) Console.WriteLine("That was an invalid index, try again.");
            }

            return textChannels[selectedIndex];
        }

        private SocketGuild GetSelectedGuild(IEnumerable<SocketGuild> guilds)
        {
            var socketGuilds = guilds.ToList();
            var maxIndex = socketGuilds.Count - 1;
            for(var i = 0; i <= maxIndex; i++)
            {
                Console.WriteLine($"{i} - {socketGuilds[i].Name}");
            }

            var selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                var success = int.TryParse(Console.ReadLine().Trim(), out selectedIndex);
                if (!success)
                {
                    Console.WriteLine("That was an invalid index, try again.");
                    selectedIndex = -1;
                }
                else if (selectedIndex < 0 || selectedIndex > maxIndex) Console.WriteLine("That was an invalid index, try again.");
            }

            return socketGuilds[selectedIndex];
        }

        private async Task client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source} ] {Message.Message}");
        }

        private async Task client_Ready()
        {
            await client.SetGameAsync("Video Games", "https://www.youtube.com/c/foxplayer?sub_confirmation=1", StreamType.NotStreaming);
        }

        private async Task client_MessageReceived(SocketMessage MessageParam)
        {
            var message = MessageParam as SocketUserMessage;
            var context = new SocketCommandContext(client, message);

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;

            var result = await commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} at commands] Something went wrong with executing a command. Text: \"{context.Message.Content}\" | Error: \"{result.ErrorReason}\"");
        }
    }
}
