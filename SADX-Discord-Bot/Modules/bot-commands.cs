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
    public class bot_commands : ModuleBase<SocketCommandContext>
    {
  
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("pong! I'm here.");
        }

        [Command("cat")]
        public async Task getCat()
        {
            var game = Program.Src.Games.SearchGame(name: "sadx");

            foreach (var category in game.FullGameCategories)
            {
                await Context.Channel.SendMessageAsync(category.Name);
            }
        }

        [Command("history")]
        public async Task getWRHistory()
        {
            await ReplyAsync("https://docs.google.com/spreadsheets/d/1r3NCGlerKyvKZc6aPV-b4eCJ6JiF1PljDndm4JnOVto/edit?usp=sharing");
        }

        [Command("notif")]
        public async Task getNotif()
        {
            var srcRefresh = new SpeedrunComClient();
            srcRefresh.AccessToken = Bot_Core.botHelper.GetSrcLogin();

            if (!srcRefresh.IsAccessTokenValid)
                return;

            var allNotif = srcRefresh.Notifications.GetNotifications();

            foreach (Notification notif in allNotif)
            {
                if (notif.Type == NotificationType.Run && notif.Status == NotificationStatus.Unread)
                {
                    string newRun = Bot_Core.botHelper.StripHTML(notif.Text);
                    await ReplyAsync(newRun);
                    await ReplyAsync(notif.Run.Category.Name);
                    await ReplyAsync(notif.Run.Date.Value.Date.ToString());
                    await ReplyAsync(notif.Run.Player.Name);
                    await ReplyAsync(notif.Run.WebLink.AbsoluteUri);
                }
            }
        }


        [Command("check")]
        public async Task checkRun()
        {
            var src = Program.Src;
           
            if (!Bot_Core.botHelper.isConnectionAllowed())
            {
                await ReplyAsync("Error, couldn't log to SRC. Are you sure the token is valid?");
                return;
            }

            string gameID = Program.Sadx.ID;
            IEnumerable<Run> runsList = src.Runs.GetRuns(gameId: gameID, status: RunStatusType.New);
           
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
                    //bgID = value.BgID;
                }

                    /*if (curRun.LevelID != null)
                    {
                        catName = curRun.LevelID;

                        Dictionary<string, SADXLevel>.ValueCollection sadxlevelList = SADXEnums.levelsID.Values;

                        foreach (var value in sadxlevelList)
                        {
                            if (value.CatID == catName)
                            {
                                string curChara = curRun.Category.Name;
                                ILCharaName = " (" + curChara + ")";
                                catName = value.CatName;
                                bgID = value.BgID;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, SADXCharacter>.ValueCollection sadxcharaList = SADXEnums.charactersID.Values;

                        foreach (var value in sadxcharaList)
                        {
                            if (value.CharID == catName)
                            {
                                catName = value.CharName;
                                bgID = value.BgID;
                                break;
                            }
                        }
                    }*/

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
                     await Context.Channel.SendMessageAsync(null, false, emb);
            }
          
            await ReplyAsync("Check done, everything is under control.");
        }
        
        [Command("reject")]
        public async Task rejectRun(string reason)
        {
            var src = Program.Src;

            src.AccessToken = Bot_Core.botHelper.GetSrcLogin();

            if (!Program.Src.IsAccessTokenValid) {
                await ReplyAsync("Error, couldn't log to SRC. Are you sure the token is valid?");
                return;
            }

            var id = "zgv6dlnz";
            src.Runs.ChangeStatus(id, RunStatusType.Rejected, reason);
            await ReplyAsync("the Run https://www.speedrun.com/sadxrando/run/zgv6dlnz was successfully rejected reason: " + reason);
        }

    }
}
