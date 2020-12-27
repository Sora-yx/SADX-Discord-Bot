using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeedrunComSharp;


namespace SADX_Discord_Bot.Modules
{
    class bot_tasks : ModuleBase<SocketCommandContext>
    {
        public class botExecTask
        {
            public static async Task Test()
            {
                var test = Program.GetRunChannel(Program.ERun.newRun);

                if (test != null)
                    await test.SendMessageAsync("Test! Every 5 sec!");
                else
                    Console.WriteLine("Error, couldn't get the channel run");
            }



        }
    }
}
