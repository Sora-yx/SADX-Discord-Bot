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
using System.Text.Json;


namespace SADX_Discord_Bot.Modules
{

    public class botExecTask 
    {
        public async Task checkNewRun()
        {
            var curChan = Program.GetRunChannel(Program.ELogChannel.newRunChan);

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

                if (isRunListed(curRun.ID))
                    return;

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
            var json = JsonSerializer.Serialize(Program.runList);
            File.WriteAllText("runList.json", json + Environment.NewLine); //Update the json file with the new updated list.
            await curChan.SendMessageAsync("Check done, everything is under control.");
        }

        public static async Task ExecuteCheckRun()
        {
            botExecTask task = new botExecTask();
            //await task.cleanRunList();
            task.checkNewRun();
        }

        public bool isRunListed(string currentRun)
        {
            List<string> run = Program.runList;
            bool result = run.Contains(currentRun);

            if (result)
                return true;

            Program.runList.Add(currentRun);
            return false;
        }


        public async Task cleanRunList()
        {
            string txt = "runList.txt";
            var src = Program.Src;
            string gameID = Program.Sadx.ID;
            IEnumerable<Run> runsList = src.Runs.GetRuns(gameId: gameID, status: RunStatusType.Verified | RunStatusType.Rejected, embeds: new RunEmbeds(embedPlayers: true)); //RunEmbeds True = no rate limit to get player name.

            foreach (Run curRun in runsList)
            {
                try
                {
                    using (var sr = new StreamReader("runList.txt"))
                    {
                        Console.WriteLine("Reading Run List information...");
                        string[] lines = File.ReadAllLines(txt);
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i] == curRun.ID)
                            {
                                lines[i].Remove(i);
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("No runList.txt found.");
                    return;
                }
            }
        }

        public async Task copyJsonToList(List<string> runList)
        {
            string json = "runList.json";

            try
            {
                using (var sr = new StreamReader(json))
                {
                    Console.WriteLine("Reading Run List information...");
                    string[] lines = File.ReadAllLines(json);
                    foreach (var curLine in lines)
                    {
                        Program.runList.Add(curLine);
                    }
                }
            }
            catch
            {
                Console.WriteLine("No runList.json found.");
                return;
            }
        }
    }

}
