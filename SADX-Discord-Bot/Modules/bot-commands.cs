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

            src.AccessToken = Bot_Core.botHelper.GetSrcLogin();

            if (!Program.Src.IsAccessTokenValid) { 
                await ReplyAsync("Error, I couldn't log to SRC.");
                return;
            }

            /*var id = "y8rg89dm";
            Program.Src.Runs.ChangeStatus(id, RunStatusType.Rejected, reason);
            await ReplyAsync("the Run was successfully rejected reason: " + reason);*/
        }

    }
}
