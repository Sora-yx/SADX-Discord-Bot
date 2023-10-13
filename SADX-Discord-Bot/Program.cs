using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeedrunComSharp;
using SADX_Discord_Bot.Modules;
using System.Text.Json;

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
        public static List<string> runList = new List<string>();
        public static List<string> runCEList = new List<string>();
        public static List<string> chanList = new List<string>();
        private bool isConnected;
        private bool isError;

        public enum ELogChannel
        {
            newRunChan,
            editRunChan,
            logBotChan
        }

        static void Main(string[] args) => new Program().RunBotMain().GetAwaiter().GetResult();

        public async Task RunBotMain()
        {
            Src = new SpeedrunComClient(maxCacheElements: 0, accessToken: BotHelper.GetSrcLogin());
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            commands = new CommandService();

            client.Log += Log;
            client.Ready += () =>
            {
                LogToSRC();
                return Task.CompletedTask;
            };

            System.Timers.Timer timer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 180000,
            };

            timer.Elapsed += CheckNewRun_Loop;
            timer.Start();

            await InstallCommandsAsync(); //set command users
            await executecopyJson();
            await LogToDiscord();
            getChanList();
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

        public static IMessageChannel GetRunChannel(ELogChannel currentChannel)
        {
            int enumInt = Convert.ToInt32(currentChannel);

            if (chanList.Count <= 0)
                return null;

            ulong id = Convert.ToUInt64(chanList[enumInt]);
            var chnl = client.GetChannel(id) as IMessageChannel;
            return (chnl);
        }

        public void LogToSRC()
        {
            var curChan = GetRunChannel(ELogChannel.logBotChan);
            var textRdy = "Ready! Gotta go fast on Discord!";

            try
            {
                Sadx = Src.Games.GetGame(BotHelper.getSADXID);
                Console.WriteLine(textRdy);

                if (curChan != null)
                {
                    if (!isConnected && !isError)
                    {
                        curChan.SendMessageAsync(textRdy + "\n" + "Successfully Connected to speedrun.com " + DateTime.Now);
                        isConnected = true;
                        isError = false;
                    }
                    else if (!isConnected && isError)
                    {
                        curChan.SendMessageAsync("We're back! " + DateTime.Now);
                        isConnected = true;
                        isError = false;        
                    }
                }
            }
            catch
            {
                var error = "Error, when trying to access SADX on src, did the API break? I will try again in 5 minutes. ";
                if (curChan != null && !isError)
                {
                    curChan.SendMessageAsync(error + DateTime.Now);
                    isError = true;
                    isConnected = false;
                }

                Console.WriteLine(error);
            }
            return;
        }

        public async Task LogToDiscord()
        {
            try
            {
                using (var sr = new StreamReader("info.txt"))
                {
                    Console.WriteLine("Reading Discord token information...");
                    string[] lines = File.ReadAllLines("info.txt");
                    await client.LoginAsync(TokenType.Bot, lines[0]);
                    sr.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error, couldn't read the file.");
                Console.WriteLine(e.Message);
                await client.StopAsync();
                await Task.Delay(1000);
                Environment.Exit(0);
            }
        }


        private async static void CheckNewRun_Loop(object sender, System.Timers.ElapsedEventArgs e)
        {
            await BotExecTask.checkAndListNewRun();
        }

        private async Task executecopyJson()
        {
            try
            {
                runList = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("runList.json"));
                runCEList = JsonSerializer.Deserialize<List<string>>(File.ReadAllText("runCEList.json"));
            }
            catch
            {
                Console.WriteLine("Couldn't open one of the json file, it will be created later.");
            }
        }

        private void getChanList()
        {
            string txt = "chan.txt";

            try
            {
                using (var sr = new StreamReader(txt))
                {
                    Console.WriteLine("Reading Channels information...");
                    string[] lines = File.ReadAllLines(txt);
                    foreach (var curLine in lines)
                    {
                        chanList.Add(curLine);
                    }
                }
            }
            catch
            {
                Console.WriteLine("No chan.txt found.");
                return;
            }
        }
    }
}
