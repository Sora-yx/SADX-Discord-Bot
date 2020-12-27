using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedrunComSharp;


namespace SADX_Discord_Bot.Modules
{

    public class botExecTask : ModuleBase<SocketCommandContext>
    {
        public async Task checkNewRun()
        {
            DiscordSocketClient _client = new DiscordSocketClient();

            var curChan = Program.GetRunChannel(Program.ERun.newRun);

            if (curChan == null)
            {
                Console.WriteLine("Error, I couldn't get the channel. Please check the text file.");
                return;
            }

            var src = Program.Src;

            if (!Bot_Core.botHelper.isConnectionAllowed())
            {
                await curChan.SendMessageAsync("Error, I couldn't log to SRC. Are you sure the token is valid? Please check the the text file.");
                return;
            }

            string gameID = Program.Sadx.ID;
            IEnumerable<Run> runsList = src.Runs.GetRuns(gameId: gameID, status: RunStatusType.New, embeds: new RunEmbeds(embedPlayers: true)); //RunEmbeds True = no rate limit to get player name.

            foreach (Run curRun in runsList)
            {
                string catName = curRun.Category.Name;
                string ILCharaName = "";
                string bgID = "";

                if (curRun.Level != null)
                {
                    string curChara = catName;
                    ILCharaName = " (" + curChara + ")";
                    catName = curRun.Level.Name;
                }

                string runTime = curRun.Times.PrimaryISO.Value.ToString(Program.timeFormat);

                if (curRun.Times.PrimaryISO.Value.Hours != 0)
                    runTime = curRun.Times.PrimaryISO.Value.ToString(Program.timeFormatWithHours);

                string runLink = curRun.WebLink.ToString();
                string bgURL = "https://i.imgur.com/";

                string ext = ".jpg";

                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(bgURL + bgID + ext)
                    .WithTitle(catName + ILCharaName + " run by " + curRun.Player.Name)
                    .WithDescription("Time: " + runTime + "\n" + runLink)
                    .WithColor(new Color(33, 176, 252));
                var emb = builder.Build();
                await curChan.SendMessageAsync(null, false, emb);
            }

                await curChan.SendMessageAsync("Check done, everything is under control."); 
        }

        public static void ExecuteCheckRun()
        {
            botExecTask task = new botExecTask();
            task.checkNewRun();
        }
    }

}
