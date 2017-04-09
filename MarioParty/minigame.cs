using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class Minigame
    {
        public int type { get; set; } // Declared at end of every turn

        public List<Player> BlueTeam { get; set; } = new List<Player>();
        public List<Player> RedTeam { get; set; } = new List<Player>();

        private Dictionary<int, string> gameTypes = new Dictionary<int, string>
        {
            { 1, "4p Game" },
            { 2, "2v2 Game" },
            { 3, "1v3 Game" }
        };

        private string directoryFile = @"PATH_TO_MINIGAMES_DIRECTORY";

        private List<string> gameList = new List<string>();

        public int[] orderedPayout = new int[4];

        public void determineType(TurnEngine t)
        {
            Record.WriteLine("Minigame time!");

            foreach (Player p in t.BlueTeam)
            {
                BlueTeam.Add(p);
            }

            foreach (Player p in t.RedTeam)
            {
                RedTeam.Add(p);
            }

            foreach (Player p in t.GreenTeam)
            {
                if (GlobalRNG.Next(0, 1) == 0)
                {
                    BlueTeam.Add(p);
                }
                else
                {
                    RedTeam.Add(p);
                }
            }

            int b = BlueTeam.Count;
            int r = RedTeam.Count;

            Record.WriteLine("Blue Team: {0}\nRed Team {1}\n", b, r);

            if (Math.Abs(b - r) == 4)
            {
                // 4v4
                Record.WriteLine("4-Player Minigame started!");
                type = 1;
            }

            if (r == b)
            {
                // 2v2
                Record.WriteLine("2v2 Minigame started!");
                type = 2;
                Record.WriteLine("TEAMS:");
                foreach (Player p in BlueTeam)
                {
                    Record.WriteLine("Blue: {0}", p.charName);
                }
                Record.WriteLine("vs");
                foreach (Player p in RedTeam)
                {
                    Record.WriteLine("Red: {0}", p.charName);
                }
            }

            if (Math.Abs(b - r) == 2)
            {
                // 3v1
                Record.WriteLine("3v1 Minigame started!");
                type = 3;

                // Reorder teams so the lone player is on Red team
                if (r > 1)
                {
                    List<Player> tempTeam = new List<Player>(RedTeam);

                    RedTeam.Clear();
                    RedTeam = new List<Player>(BlueTeam);
                    BlueTeam.Clear();
                    BlueTeam = new List<Player>(tempTeam);
                }

                Record.WriteLine("TEAMS:");
                foreach (Player p in BlueTeam)
                {
                    Record.WriteLine("Blue: {0}", p.charName);
                }
                Record.WriteLine("vs");
                foreach (Player p in RedTeam)
                {
                    Record.WriteLine("Red: {0}", p.charName);
                }
            }

        }

        public int[] randomResults(TurnEngine t, bool loserPenalty=false)
        {
            int[] coinSpread = { 0, 0, 0, 0 };
            List<Player> winners = new List<Player>();
            int rngRoll = 0;

            switch (type)
            {
                case 1:
                    rngRoll = GlobalRNG.Next(0, 3);
                    winners.Add(t.order[rngRoll]);
                    
                    break;

                case 2:
                    rngRoll = GlobalRNG.Next(1, 2);
                    if (rngRoll == 1)
                    {
                        winners = BlueTeam;
                    }
                    else
                    {
                        winners = RedTeam;
                    }
                    break;

                case 3:
                    rngRoll = GlobalRNG.Next(1, 2);
                    if (rngRoll == 1)
                    {
                        winners = BlueTeam;
                    }
                    else
                    {
                        winners = RedTeam;
                    }
                    break;
            }

            for (int i = 0; i < 4; i++)
            {
                if (winners.Contains(t.order[i]))
                {
                    coinSpread[i] = 10;
                }
                else
                {
                    if (loserPenalty == true)
                        coinSpread[i] = -5;
                }
            }

            Record.WriteLine("\nWinner(s):");
            foreach (Player p in winners)
                Record.WriteLine(p.charName);

            return coinSpread;
        }

        private void chooseGame()
        {
            if ((type < 1) || (type > 3))
                return;

            XmlReader reader = XmlReader.Create(directoryFile);
            for (int i = 0; i < type; i++)
                reader.ReadToFollowing("type");

            reader.Read(); // Advance for the sake of the while loop

            List<minigameOption> options = new List<minigameOption>(); // List (not array) for dynamic size based on XML content
            string tempString;
            bool tempBool;
            string tempStringB;

            while (reader.Name != "type") // Go through every game of that type and record it
            {
                reader.ReadToFollowing("game");
                reader.ReadToFollowing("name");
                tempString = reader.ReadElementContentAsString();
                reader.ReadToFollowing("multiwin");
                //tempBool = reader.ReadElementContentAsBoolean();
                tempStringB = reader.ReadElementContentAsString();

                Console.WriteLine("{0}:{1}",tempString,tempStringB);
                Console.ReadLine();
                options.Add(new minigameOption { name = tempString});
            }

            foreach (minigameOption m in options)
            {
                Record.WriteLine("{0} : {1}", m.name,m.multiwin);
            }

            /*string test = reader.GetAttribute("name");
            Console.WriteLine(": "+test); */
            Console.ReadLine();
            

        }

        private class minigameOption
        {
            public string name { get; set; }
            public bool multiwin { get; set; }
        }
    }
}

   

