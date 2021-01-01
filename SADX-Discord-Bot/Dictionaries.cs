using System;
using System.Collections.Generic;
using System.Text;

namespace SADX_Discord_Bot
{
    public class SADXCharacter
    {
        public string CharID { get; set; }
        public string CharName { get; set; }
        public string BgID { get; set; }

        public SADXCharacter(string charID, string charName, string bgID)
        {
            this.CharID = charID;
            this.CharName = charName;
            this.BgID = bgID;
        }
    }
    public class SADXLevel
    {
        public string CatID { get; set; }
        public string CatName { get; set; }
        public string BgID { get; set; }

        public SADXLevel(string catID, string catName, string bgID)
        {
            this.CatID = catID;
            this.CatName = catName;
            this.BgID = bgID;
        }
    }

    public static class SADXEnums
    {
        public static Dictionary<string, SADXLevel> levelsID = new Dictionary<string, SADXLevel>()
        {
            //Action Stages //TODO: Change BG ID with the original Level Picture
            {"EC", new SADXLevel("592zmmg9", "Emerald Coast", "qt9qXJo")},
            {"WV", new SADXLevel("29v3g61w", "Windy Valley", "qt9qXJo")},
            {"CAS", new SADXLevel("xd4jp10d", "Casinopolis", "qt9qXJo")},
            {"IC", new SADXLevel("xd0gp10w", "Ice Cap", "qt9qXJo")},
            {"TP", new SADXLevel("rw6421n9", "Twinkle Park", "qt9qXJo")},
            {"SH", new SADXLevel("n937k17d", "Speed Highway", "qt9qXJo")},
            {"RM", new SADXLevel("z986p37d", "Red Mountain", "qt9qXJo")},
            {"SD", new SADXLevel("rdn037nw", "Sky Deck", "qt9qXJo")},
            {"LW", new SADXLevel("ldyyqxjd", "Lost World", "qt9qXJo")},
            {"FE", new SADXLevel("gdrern89", "Final Egg", "qt9qXJo")},
            {"HS", new SADXLevel("nwll05pw", "Hot Shelter", "qt9qXJo")},

            //Sub Game
            {"SHL", new SADXLevel("y9mj06l9", "Sand Hill", "qt9qXJo")},
            {"TC", new SADXLevel("5wkj0x5d", "Twinkle Circuit", "qt9qXJo")},

            //Chaos Bosses
            {"C0", new SADXLevel("ywe10yqd", "Chaos 0", "qt9qXJo")},
            {"C2", new SADXLevel("4956p22d", "Chaos 2", "qt9qXJo")},
            {"C4", new SADXLevel("r9g26lqd", "Chaos 4", "qt9qXJo")},
            {"C6", new SADXLevel("o9xn6g6w", "Chaos 6", "qt9qXJo")},
            {"PC", new SADXLevel("owo08jvw", "Perfect Chaos", "qt9qXJo")},

            //Eggman Bosses
            {"EH", new SADXLevel("69z3756d", "Egg Hornet", "qt9qXJo")},
            {"EV", new SADXLevel("xd17pmqd", "Egg Viper", "qt9qXJo")},
            {"EW", new SADXLevel("ewpjn3lw", "Egg Walker", "qt9qXJo")},
           
            //E100 Bosses
            {"E101", new SADXLevel("rdqjy8m9", "E101 Beta", "qt9qXJo")},
            {"MK2", new SADXLevel("kwj50gr9", "Beta MK.II", "qt9qXJo")},
            {"Zero", new SADXLevel("5d7vp15d", "Zero", "qt9qXJo")},
        };

        public static Dictionary<string, SADXCharacter> charactersID = new Dictionary<string, SADXCharacter>()
        {
            {"sonic", new SADXCharacter("mndx5rkq", "Sonic's Story", "JNzU3kg")},
            {"tails", new SADXCharacter("xw20x52n", "Tails's Story", "1UUd2MI")},
            {"knux", new SADXCharacter("xwdmvodq", "Knuckles's Story", "Ei2zWl2")},
            {"amy", new SADXCharacter("lvdoxvdp", "Amy's Story", "h0hMckY")},
            {"big", new SADXCharacter("37dg0pk4", "Big's Story", "hsic1CA")},
            {"e102", new SADXCharacter("7wkp0w2r", "Gamma's Story", "HCounU0")},
            {"ss", new SADXCharacter("4xk94vd0", "Super Sonic's Story", "zugq3NE")},
            {"all", new SADXCharacter("pmkel6d6", "All Stories", "qt9qXJo")},
            {"130", new SADXCharacter("w5dw6l2g", "130 Emblems", "2IPPCec")},
        };
    }
}
