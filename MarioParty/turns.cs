using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class TurnEngine
    {

        // 
        public int currentTurn { get; set; } = 0;

        public int totalTurns { get; set; } // To be declared at instantiation

        public Player[] order { get; set; } = new Player[4];

        public int curPlayerTurn { get; set; } = 0;

        public List<Player> BlueTeam { get; set; } = new List<Player>();
        public List<Player> RedTeam { get; set; } = new List<Player>();
        public List<Player> GreenTeam { get; set; } = new List<Player>();

        public Board thisBoard { get; set; }

        //public Player[] playerRanks { get; set; } = new Player[4];

        private Dictionary<int, string> rankNames = new Dictionary<int, string>()
        {
            {1, "1st" },
            {2, "2nd" },
            {3, "3rd" },
            {4, "4th" }
        };

        public int HitDiceBlock(int num = 1, int dbType = 0)
        {
            Record.WriteLine("\nROLL:");
            //Random roll = new Random();
            int rollBuffer = 0;
            int totalRoll = 0;
            int min = 1;
            int max = 10;
            if (dbType == 1)
            {
                min = 1;
                max = 5;
            }
            if (dbType == 2)
            {
                min = 6;
                max = 10;
            }
            
            while (num > 0)
            {
                rollBuffer = GlobalRNG.Next(min, max);
                //Record.Write("{0} ", rollBuffer);
                totalRoll += rollBuffer;
                num--;
            }
            //Record.WriteLine("Total roll: {0}\n", totalRoll);
            return totalRoll;
        }

        public void firstTurn(Player a, Player b, Player c, Player d, Board map)
        {
            Record.WriteLine("Beginning game...\n");

            // Set board
            thisBoard = map;

            // Determine turn order, no ties allowed!
            int[] rollResults = new int[4];
            
            //Random roll = new Random();
            int attempt = GlobalRNG.Next(0, 10);

            Dictionary<int, Player> playerDict = new Dictionary<int, Player>()
            {
                {0, a},
                {1, b},
                {2, c},
                {3, d}
            };

            // Populate array with distinct rolls   --- looks like I wrote this before making the DistinctRolls function and didn't update it
            for (int r=0; r<4; r++)
            {
                while (rollResults.Contains(attempt))
                    attempt = GlobalRNG.Next(0, 10);
                rollResults[r] = attempt;
                Player curPlayer;
                playerDict.TryGetValue(r, out curPlayer);
                Record.WriteLine("{0} rolled {1}!",curPlayer.charName,rollResults[r]);
            }
            
            // Sort rolls
            int[] rankedRolls = new int[4];
            Array.Copy(rollResults, rankedRolls,4);
            Array.Sort<int>(rankedRolls);
            Array.Reverse(rankedRolls);
            Record.WriteLine("");
            
            // Player 1 (A)
            int p1Order = Array.IndexOf(rankedRolls, rollResults[0])+1;
            //Record.WriteLine("P1 roll: {0}, P1 turn order: {1}",rollResults[0],p1Order);
            a.turnOrder = p1Order;
            order[p1Order - 1] = a;

            // Player 2 (B)
            int p2Order = Array.IndexOf(rankedRolls, rollResults[1]) + 1;
            //Record.WriteLine("P2 roll: {0}, P2 turn order: {1}", rollResults[1], p2Order);
            b.turnOrder = p2Order;
            order[p2Order - 1] = b;

            // Player 3 (C)
            int p3Order = Array.IndexOf(rankedRolls, rollResults[2]) + 1;
            //Record.WriteLine("P3 roll: {0}, P3 turn order: {1}", rollResults[2], p3Order);
            c.turnOrder = p3Order;
            order[p3Order - 1] = c;

            // Player 4 (D)
            int p4Order = Array.IndexOf(rankedRolls, rollResults[3]) + 1;
            //Record.WriteLine("P4 roll: {0}, P4 turn order: {1}", rollResults[3], p4Order);
            d.turnOrder = p4Order;
            order[p4Order - 1] = d;

            Record.WriteLine("Turn Order:");
            foreach (Player p in order)
            {
                Record.WriteLine("{0}: {1}",p.turnOrder,p.charName);
            }


            // 10 coins
            Record.WriteLine("\nKoopa Troopa gave everyone 10 coins!");
            order[0].ChangeAssets(10);
            Record.WriteLine("---{0}'s coins: {1}",order[0].charName,order[0].coins);
            order[1].ChangeAssets(10);
            Record.WriteLine("---{0}'s coins: {1}", order[1].charName, order[1].coins);
            order[2].ChangeAssets(10);
            Record.WriteLine("---{0}'s coins: {1}", order[2].charName, order[2].coins);
            order[3].ChangeAssets(10);
            Record.WriteLine("---{0}'s coins: {1}", order[3].charName, order[3].coins);

            rankPlayers();
            nextTurn();
        }

        public void nextTurn()
        {
            if (currentTurn == totalTurns)
            {
                // Last turn...go to results screen
                return;
            }

            if (currentTurn == (totalTurns - 5))
            {
                lastFive();
            }

            currentTurn++;
            BlueTeam.Clear();
            RedTeam.Clear();
            GreenTeam.Clear();

            Record.WriteLine("\nTURN {0}", currentTurn);

            foreach (Player p in order)
            {
                // Player's turn
                Record.WriteLine("\n{0}'s turn!", p.charName);
                p.alignment = 0; // Reset

                // Use item

                // Roll
                int roll = HitDiceBlock();
                Record.WriteLine("---{0} rolled a {1}", p.charName, roll);
                p.moveSpaces = roll;

                while (p.moveSpaces > 0)
                {
                    // Movement

                    foreach (Space s in thisBoard.spaces)
                    {
                        
                            if (s.mapID == p.space)  // Match space to Player location
                            {
                                if (s.landable == true)
                                {
                                    p.moveSpaces--;
                                }

                                s.PlayerLand(p, thisBoard, this); // Land
                                //Record.WriteLine("---{0} passed {1}.", p.charName, s.name);

                            // Advance player to next space
                            if (p.locomotion == 1)
                                {
                                    if (s.next.Length == 1) // No forking
                                    {
                                        p.space = s.next[0];
                                        //p.space += 1;
                                        //Record.WriteLine("-----Updated {0}'s position.", p.charName);
                                    break;
                                    }
                                    else                    // Forking    -- Not yet implemented...generated boards are a big ol' circle.
                                    {

                                    }
                                }

                                if (p.locomotion == -1)
                                {

                                }
                            }
                        
                        
                    }

                    if (p.moveSpaces > 0)
                        Record.WriteLine("---Moves left: {0}", p.moveSpaces);

                }
            }

            // Mini-Game

            rankPlayers();

            Minigame m = new Minigame();
            m.determineType(this);
            int[] coinChange = m.randomResults(this); // Gets change in coins for each player in an int[] (in turn order)

            Record.WriteLine("\nRESULTS:");

            for (int i=0;i<4;i++)
            {
                order[i].ChangeAssets(coinChange[i]);
                order[i].minigameCoins += coinChange[i];
                Record.WriteLine("--{0}: {1} coins",order[i].charName, coinChange[i]);
            }

        }

        public void lastFive()
        {
            thisBoard.coinSpaceValue = 6;
            Record.WriteLine("\nLast 5 turns! Coin spaces now give/take 6 coins!\n");
        }

        public void rankPlayers()
        {
            
            int[] scores = new int[4];

            for (int i=0; i<4; i++)
            {
                scores[i] = order[i].stars * 1000 + order[i].coins;
            }

            int[] rankedscores = new int[4];
            Array.Copy(scores, rankedscores, 4);
            Array.Sort<int>(rankedscores);
            Array.Reverse(rankedscores);

            // First Place
            int curRank = 1;
            int curScore = rankedscores[0];

            for (int i=0;i<4;i++)
            {
                if (scores[i] == curScore )
                    order[i].rank = curRank;
            }

            // Second Place

            if (rankedscores[1] != curScore)
            {
                curScore = rankedscores[1];
                curRank++;

                for (int i = 0; i < 4; i++)
                {
                    if (scores[i] == curScore)
                        order[i].rank = curRank;
                }
            }

            // Third Place

            if (rankedscores[2] != curScore)
            {
                curScore = rankedscores[2];
                curRank++;

                for (int i = 0; i < 4; i++)
                {
                    if (scores[i] == curScore)
                        order[i].rank = curRank;
                }
            }

            // Fourth Place

            if (rankedscores[3] != curScore)
            {
                curScore = rankedscores[3];
                curRank++;

                for (int i = 0; i < 4; i++)
                {
                    if (scores[i] == curScore)
                        order[i].rank = curRank;
                }
            }

            Record.WriteLine("\nStandings:\n");
            string[] rankNamesNice = new string[4];

            for (int r = 0; r < 4; r++)
            {
                rankNames.TryGetValue(order[r].rank, out rankNamesNice[r]);
                Record.WriteLine("--{0} - {1}", rankNamesNice[r],order[r].charName);
                Record.WriteLine("--Stars: {0}   Coins: {1}\n",order[r].stars,order[r].coins);
            }
            
        }

        public void bonusStars()
        {
            Record.WriteLine("\nBonus Star Time!");
            coinStar();
            minigameStar();
            happeningStar();

            rankPlayers(); // Final Results

        }

        private void happeningStar()
        {
            Record.WriteLine("\nHappening Star:");
            int topCount = 0;

            // Find top value
            foreach (Player p in order)
            {
                Record.WriteLine("---{0} landed on {1} Happening (?) Spaces", p.charName, p.happeningSpaces);

                if (p.happeningSpaces > topCount)
                    topCount = p.happeningSpaces;
            }

            // Find players with top value
            foreach (Player p in order)
            {
                if (p.happeningSpaces == topCount)
                {
                    p.ChangeAssets(0, 1);
                    Record.WriteLine("{0} won the Happening Star!", p.charName);
                }
            }
        }

        private void coinStar()
        {
            Record.WriteLine("\nCoin Star:");

            int topCount = 0;
            
            // Find top value
            foreach (Player p in order)
            {
                Record.WriteLine("---{0} had at one point {1} coins", p.charName, p.maxCoins);

                if (p.maxCoins > topCount)
                    topCount = p.maxCoins;
            }

            // Find players with top value
            foreach (Player p in order)
            {
                if (p.maxCoins == topCount)
                {
                    p.ChangeAssets(0, 1);
                    Record.WriteLine("{0} won the Coin Star!", p.charName);
                }
            }
        }

        private void minigameStar()
        {
            Record.WriteLine("\nMinigame Star:");

            int topCount = 0;

            // Find top value
            foreach (Player p in order)
            {
                Record.WriteLine("---{0} gained {1} coins from minigames", p.charName, p.minigameCoins);

                if (p.minigameCoins > topCount)
                    topCount = p.minigameCoins;
            }

            // Find players with top value
            foreach (Player p in order)
            {
                if (p.minigameCoins == topCount)
                {
                    p.ChangeAssets(0, 1);
                    Record.WriteLine("{0} won the Mini Game Star!", p.charName);
                }
            }
        }

    }
}
