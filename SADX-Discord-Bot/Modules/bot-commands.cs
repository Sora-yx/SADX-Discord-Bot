using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
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

            var gameID = Program.Sadx.ID;
            IEnumerable<Run> runsList = src.Runs.GetRuns(gameId: gameID, status: RunStatusType.New);
            string catName = "";
            string runTime = "";
            string runLink = "";

            foreach (Run curRun in runsList)
            {
                catName = curRun.Category.Name;
                runTime = curRun.Times.PrimaryISO.Value.ToString();
                runLink = curRun.WebLink.ToString();
                await ReplyAsync(catName + runTime + runLink);
            }
            
            //Get Category Extension runs awaiting verification
            var gameCEID = Bot_Core.botHelper.getCEID;
            IEnumerable<Run> SADXCEruns = src.Runs.GetRuns(gameId: gameCEID, status: RunStatusType.New);

            foreach (Run curRun in SADXCEruns)
            {
                catName = curRun.Category.Name;
                runTime = curRun.Times.PrimaryISO.Value.ToString();
                runLink = curRun.WebLink.ToString();
                await ReplyAsync(catName + runTime + runLink);
            }

            await ReplyAsync("Check done, everything is under control.");
        }
        
        [Command("reject")]
        public async Task rejectRun(string reason)
        {
            var src = Program.Src;

            src.AccessToken = Bot_Core.botHelper.GetSrcLogin();

            if (!Program.Src.IsAccessTokenValid) { 
                await ReplyAsync("Error, I couldn't log to SRC.");
                return;
            }

            var id = "zgv6dlnz";
            src.Runs.ChangeStatus(id, RunStatusType.Rejected, reason);
            await ReplyAsync("the Run https://www.speedrun.com/sadxrando/run/zgv6dlnz was successfully rejected reason: " + reason);
        }

    }
}
