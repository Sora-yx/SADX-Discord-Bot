using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedrunComSharp;
using System.Text.RegularExpressions;
using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using System.Net;

namespace SADX_Discord_Bot.Modules
{
    public class BotHelper
    {
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string GetSrcLogin()
        {
            try
            {
                using (var sr = new StreamReader("info.txt"))
                {
                    string[] lines = File.ReadAllLines("info.txt");
                    string tok2 = lines[1];
                    return tok2;
                }
            }
            catch
            {
                Console.WriteLine("Error, couldn't get API Token");
                return null;
            }
        }

        public static bool isConnectionAllowed()
        {
            Program.Src.AccessToken = GetSrcLogin();

            if (!Program.Src.IsAccessTokenValid)
            {
                Console.WriteLine("Error, couldn't log to SRC. Make sure the bot can read the SRC Token.");
                return false;
            }

            return true;
        }

        public static string getCEID
        {
            get { return "268r391p"; }
        }
    }


}
