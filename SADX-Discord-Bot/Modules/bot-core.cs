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
    class Bot_Core
    {
        public class botHelper
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

            public static string getCharacterPicture(string catName)
            {

                    switch (catName)
                    {
                    case "Sonic's Story":
                        return "https://i.imgur.com/JNzU3kg.jpg";
                    case "Tails's Story":
                        return "https://i.imgur.com/1UUd2MI.jpg";
                    case "Knuckles's Story":
                        return "https://i.imgur.com/Ei2zWl2.jpg";             
                    case "Amy's Story":
                        return "https://i.imgur.com/h0hMckY.jpg";
                    case "Big's Story":
                        return "https://i.imgur.com/hsic1CA.jpg";
                    case "Gamma's Story":
                        return "https://i.imgur.com/HCounU0.jpg";
                    case "Super Sonic's Story":
                        return "https://i.imgur.com/zugq3NE.jpg";
                    case "130 Emblems":
                        return "https://i.imgur.com/2IPPCec.jpg";
                    case "All Stories":
                        return "https://i.imgur.com/qt9qXJo.jpeg";
                    }

                return "https://i.imgur.com/qt9qXJo.jpeg";
            }
        }

    }
}
