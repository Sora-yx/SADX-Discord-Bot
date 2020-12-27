using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeedrunComSharp;
using System.Timers;

namespace SADX_Discord_Bot
{
    class Program
    {
        private static DiscordSocketClient client;
        private CommandService commands;
        public static SpeedrunComClient Src;
        public static SpeedrunComSharp.Game Sadx;
        public static string timeFormat = @"mm\:ss\.ff";
        public static string timeFormatWithHours = @"hh\:mm\:ss\.ff";

        public enum ERun
        {
            newRun,
            EditRun
        }

        static void Main(string[] args) => new Program().RunBotMain().GetAwaiter().GetResult();


        public async Task RunBotMain()
        {
            Src = new SpeedrunComClient(maxCacheElements: 0);
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            commands = new CommandService();

            client.Log += Log;
            client.Ready += () =>
            {
                Sadx = Src.Games.SearchGame(name: "SADX");
                Console.Write("Ready! Gotta go fast!");
                return Task.CompletedTask;
            };

            System.Timers.Timer timer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 3000,
            };

            timer.Elapsed += CheckNewRun_Loop;
            timer.Start();

            await InstallCommandsAsync(); //set command users

            try
            {
                using (var sr = new StreamReader("info.txt"))
                {
                    Console.WriteLine("Reading token information...");
                    string[] lines = File.ReadAllLines("info.txt");
                    await client.LoginAsync(TokenType.Bot, lines[0]);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error, couldn't read the file.");
                Console.WriteLine(e.Message);
                await Task.Delay(3000);
                Environment.Exit(0);
            }

            await client.StartAsync();
            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }
        private async Task HandleCommandAsync(SocketMessage Pmsg)
        {
            var message = (SocketUserMessage)Pmsg; //Convert to Socket user msg

            if (message == null)
                return;

            int argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos)) //if msg user doesn't start with a "!"
                return;

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(context, argPos, null);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason); //send error msg
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());
            return Task.CompletedTask;
        }

        public static IMessageChannel GetRunChannel(ERun currentChannel)
        {
            string stringID = "";

            try
            {
                using (var sr = new StreamReader("info.txt"))
                {
                    Console.WriteLine("Reading channels information...");
                    string[] lines = File.ReadAllLines("info.txt");
                    if (currentChannel == ERun.newRun)
                        stringID = lines[2];

                    if (currentChannel == ERun.EditRun)
                        stringID = lines[3];

                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error, couldn't read the file.");
                Console.WriteLine(e.Message);
            }

            ulong id = Convert.ToUInt64(stringID);
            var chnl = client.GetChannel(id) as IMessageChannel;
            return (chnl);
        }


        private static Task LoopCheck()
        {
            var test = GetRunChannel(ERun.newRun);

            if (test != null)
                test.SendMessageAsync("Test!");
            else
                Console.WriteLine("Error, couldn't get the channel run");

            return Task.CompletedTask;
        }

        private static void CheckNewRun_Loop(object sender, System.Timers.ElapsedEventArgs e)
        {
            LoopCheck();
        }

    }
}
