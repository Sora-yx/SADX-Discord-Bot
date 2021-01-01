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

    public static class BotExecTask
    {
        public static async Task checkNewRun()
        {
            var curChan = Program.GetRunChannel(Program.ELogChannel.newRunChan);

            if (curChan == null)
            {
                Console.WriteLine("Error, I couldn't get the channel to send the runs. Please check the text channel file.");
                return;
            }

            if (!BotHelper.isConnectionAllowed())
            {
                curChan = Program.GetRunChannel(Program.ELogChannel.logBotChan);
                if (curChan != null)
                    await curChan.SendMessageAsync("Error, I couldn't log to SRC to check the runs. The site might be down, also make sure the Token is valid.");
                Console.WriteLine("Error, couldn't log to SRC to check the runs.");
                return;
            }

            //List Runs
            string gameID = Program.Sadx.ID;
            await ListNewRuns(gameID, curChan);
            //Category Extension
            gameID = BotHelper.getCEID;
            await ListNewRuns(gameID, curChan);
        }

        public static async Task ListNewRuns(string gameID, IMessageChannel curChan)
        {
            List<string> newRunList = new List<string>();

            IEnumerable<Run> runsList = Program.Src.Runs.GetRuns(gameId: gameID, status: RunStatusType.New, embeds: new RunEmbeds(embedPlayers: true)); //RunEmbeds True = no rate limit to get player name.
            bool listEdited = false;

            foreach (Run curRun in runsList)
            {
                string catName = curRun.Category.Name;
                string ILCharaName = "";
                string bgID = ""; 
                bgID += ".jpg";
                string resultDay = BotHelper.getSubmittedDay(curRun);
                newRunList.Add(curRun.ID);

                if (curRun.Level != null)
                {
                    string curChara = catName;
                    ILCharaName = " (" + curChara + ")";
                    catName = curRun.Level.Name;
                }

                if (isRunListed(curRun.ID, gameID))
                {
                    continue;
                }
                else
                {
                    listEdited = true;
                    string runTime = curRun.Times.PrimaryISO.Value.ToString(Program.timeFormat);

                    if (curRun.Times.PrimaryISO.Value.Hours != 0)
                        runTime = curRun.Times.PrimaryISO.Value.ToString(Program.timeFormatWithHours);

                    string runLink = curRun.WebLink.ToString();
                    string bgURL = "https://i.imgur.com/";


                    var builder = new EmbedBuilder()
                        .WithThumbnailUrl(bgURL + bgID)
                        .WithTitle(catName + ILCharaName + " run by " + curRun.Player.Name)
                        .WithDescription("Time: " + runTime + "\n" + runLink + "\n" + "Submitted " + resultDay)
                        .WithColor(new Color(33, 176, 252));
                    var emb = builder.Build();
                    await curChan.SendMessageAsync(null, false, emb);
                }
            }

            if (listEdited)   //Update the json file with the new updated list.
            {
                if (gameID == BotHelper.getCEID)
                {
                    Program.runCEList = newRunList;
                    var jsonCE = JsonSerializer.Serialize(Program.runCEList);
                    File.WriteAllText("runCEList.json", jsonCE + Environment.NewLine);
                    Console.WriteLine("Updated Category Extension run list and json file.");
                }
                else
                {
                    Program.runList = newRunList;
                    var json = JsonSerializer.Serialize(Program.runList);
                    File.WriteAllText("runList.json", json + Environment.NewLine);
                    Console.WriteLine("Updated run list and json file.");
                }
            }
        }

        public static bool isRunListed(string currentRun, string gameID)
        {
            List<string> run = Program.runList;

            if (gameID == BotHelper.getCEID)
                run = Program.runCEList;

            bool result = run.Contains(currentRun);

            if (result)
                return true;

            return false;
        }

        public static void listWRCategory(string cat)
        {
            Leaderboard LB = Program.Src.Leaderboards.GetLeaderboardForFullGameCategory(gameId: Program.Sadx.ID, categoryId: cat);
            if (LB.Category != null)
                Console.WriteLine(LB.Category.Name.ToString());
        }
    }

}
