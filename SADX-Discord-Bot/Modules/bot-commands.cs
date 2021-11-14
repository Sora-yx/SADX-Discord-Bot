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

        [Command("help")]
        public async Task help()
        {
            await ReplyAsync("If you are looking for a World Record of a category, you need to type !wr and the abbreviation of the category just after: " +
                "\n**!wr knux** will tell you the WR for Knuckles's Story." +
                "\n**!wr ec s** will tell you the WR for Sonic's Emerald Coast." +
                "\nIf you want the WR history, type: **!history**" +
                "\n**Staff Command:** type **!check** in a channel to get the runs awaiting verification.");
        }


        [Command("history")]
        public async Task getWRHistory()
        {
            await ReplyAsync("https://docs.google.com/spreadsheets/d/1r3NCGlerKyvKZc6aPV-b4eCJ6JiF1PljDndm4JnOVto/edit?usp=sharing");
        }

        [Command("wr")]
        public async Task getWR(string category)
        {
            var src = Program.Src;

            Dictionary<string, SADXCharacter> sadxcharaList = SADXEnums.charactersID;

            foreach (var key in sadxcharaList.Keys)
            {
                if (key == category)
                {
                    Leaderboard LB = src.Leaderboards.GetLeaderboardForFullGameCategory(gameId: Program.Sadx.ID, categoryId: sadxcharaList[key].CharID, 5);
                    string catName = sadxcharaList[key].CharName;
                    string bgID = sadxcharaList[key].BgID += ".jpg";
                    string runLink = LB.Records[0].WebLink.ToString();
                    string bgURL = "https://i.imgur.com/";

                    string runTime = LB.Records[0].Times.PrimaryISO.Value.ToString(Program.timeFormat);

                    if (LB.Records[0].Times.PrimaryISO.Value.Hours != 0)
                        runTime = LB.Records[0].Times.PrimaryISO.Value.ToString(Program.timeFormatWithHours);

                    var builder = new EmbedBuilder()
                        .WithTitle(catName)
                       .WithThumbnailUrl(bgURL + bgID)
                       .WithDescription("The World Record is " + runTime + " by " + LB.Records[0].Player.Name + "\n" + runLink)
                       .WithColor(new Color(33, 176, 252));
                    var emb = builder.Build();
                    await ReplyAsync(null, false, emb);
                    return;
                }
            }
        }

        [Command("wr")]
        public async Task getILWR(string category, string character)
        {
            Dictionary<string, SADXLevel> sadxlvlList = SADXEnums.levelsID;

            Dictionary<string, string> charaILID = SADXEnums.charaILID;

            foreach (var key2 in sadxlvlList.Keys)
            {
                if (key2 == category)
                {
                    string lvl = sadxlvlList[key2].levelID;

                    Leaderboard LB2 = Program.Src.Leaderboards.GetLeaderboardForLevel(gameId: Program.Sadx.ID, levelId: lvl, charaILID[character], 5);

                    if (LB2.Records[0] != null)
                    {
                        string catName2 = sadxlvlList[key2].CatName;
                        string bgID2 = sadxlvlList[key2].BgID += ".jpg";
                        string runLink2 = LB2.Records[0].WebLink.ToString();
                        string bgURL2 = "https://i.imgur.com/";

                        string runTime2 = LB2.Records[0].Times.PrimaryISO.Value.ToString(Program.timeFormat);

                        if (LB2.Records[0].Times.PrimaryISO.Value.Hours != 0)
                            runTime2 = LB2.Records[0].Times.PrimaryISO.Value.ToString(Program.timeFormatWithHours);

                        var builder = new EmbedBuilder()
                            .WithTitle(catName2 + " (" + LB2.Records[0].Category.Name + ")")
                           .WithThumbnailUrl(bgURL2 + bgID2)
                           .WithDescription("The World Record is " + runTime2 + " by " + LB2.Records[0].Player.Name + "\n" + runLink2)
                           .WithColor(new Color(33, 176, 252));
                        var emb = builder.Build();
                        await ReplyAsync(null, false, emb);
                    }
                }
            }
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
                    await ReplyAsync("Error, couldn't log to SRC. Are you sure the token is valid? Perhaps the site is down or laggy.");
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
            await ReplyAsync(":wave: See ya! \n");
            DiscordSocketClient task = new DiscordSocketClient();
            var curChan = Program.GetRunChannel(Program.ELogChannel.logBotChan);
            if (curChan != null)
                await curChan.SendMessageAsync("Disconnected... " + DateTime.Now);
            await task.StopAsync();
            await Task.Delay(500);
            Environment.Exit(0);
        }

        [Command("count")]
        public async Task CountDown(string count)
        {

            var conUser = Context.User;

            if (conUser is SocketGuildUser user)
            {
                // Check if the user has the required role (mod, verifier and tournament organizer)
                if (!user.Roles.Any(r => r.Id == 772829556716470302) && !user.Roles.Any(r => r.Id == 772830066677121044) && !user.Roles.Any(r => r.Id == 896190676277010433))
                {
                    await ReplyAsync("You don't have the permission for this action.");
                    return;
                }

                int numericValue;
                int numericCopy;

                bool isNumber = int.TryParse(count, out numericValue);


                if (count == "" || !isNumber || numericValue > 20)
                {
                    await ReplyAsync("Please enter a valid number, max allowed is 20. (ie: !count 10)");
                    return;
                }

                numericCopy = numericValue;

                do
                {
                    if (numericCopy != numericValue)
                        await Task.Delay(1000);

                    await ReplyAsync(numericValue.ToString());
                    numericValue--;

                } while (numericValue > -1);
            }



            return;
        }
    }
}
