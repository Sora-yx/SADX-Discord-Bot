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
    public class Bot_commands : ModuleBase<SocketCommandContext>
    {

        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("pong! Hi, I'm here.");
        }


        [Command("history")]
        public async Task getWRHistory()
        {
            await ReplyAsync("https://docs.google.com/spreadsheets/d/1r3NCGlerKyvKZc6aPV-b4eCJ6JiF1PljDndm4JnOVto/edit?usp=sharing");
        }

        [Command("check")]
        public async Task checkRun()
        {
            var src = Program.Src;
            var conUser = Context.User;

            if (conUser is SocketGuildUser user)
            {
                // Check if the user has the required role
                if (!user.Roles.Any(r => r.Name == "Moderator" && !user.Roles.Any(r => r.Name == "Verifier")))
                {
                    await ReplyAsync("You don't have the permission for this action.");
                    return;
                }

                if (!BotHelper.isConnectionAllowed())
                {
                    await ReplyAsync("Error, couldn't log to SRC. Are you sure the token is valid?");
                    return;
                }

                await ReplyAsync("Dm'ed you the runs awaiting verification. (If any.)");

                string gameID = Program.Sadx.ID;
                IEnumerable<Run> runsList = src.Runs.GetRuns(gameId: gameID, status: RunStatusType.New, embeds: new RunEmbeds(embedPlayers: true)); //RunEmbeds True = no rate limit to get player name.

                foreach (Run curRun in runsList)
                {
                    string catName = curRun.Category.Name;
                    string ILCharaName = "";
                    string bgID = "";
                    string resultDay = BotHelper.getSubmittedDay(curRun);

                    if (curRun.Level != null)
                    {
                        ILCharaName = " (" + catName + ")";
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
                        .WithDescription("Time: " + runTime + "\n" + runLink + "\n" + "Submitted " + resultDay)
                        .WithColor(new Color(33, 176, 252));
                    var emb = builder.Build();
                    await Context.User.SendMessageAsync(null, false, emb);
                }
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("quit")]
        public async Task exitBot()
        {
            DiscordSocketClient task = new DiscordSocketClient();
            var curChan = Program.GetRunChannel(Program.ELogChannel.logBotChan);
            if (curChan != null)
                await curChan.SendMessageAsync(":wave: See ya! \n" + "Disconnected... " + DateTime.Now);
            await task.StopAsync();
            await Task.Delay(500);
            Environment.Exit(0);
        }
    }
}
