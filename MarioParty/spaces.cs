using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class Space
    {
        
        public int type { get; set; }
        public int mapID { get; set; }          // For map routing and special spaces

        private static Dictionary<int, string> goodNames = new Dictionary<int, string>()
        {
            {1, "Blue Space"},
            {2, "Red Space"},
            {3, "Happening (?) Space"},
            {4, "Chance (!) Space"},
            {5, "Bowser Space"},
            {6, "Star Space"},      // Map will define which spaces are able to become star spaces
            {7, "Bowser Star Space"},
            {8, "Mirage Star Space"},
            {9, "Toad Shop"},
            {10, "Bowser Jr. Shop"},
            {11, "Bank"},
            {12, "Boo Space"},
            {13, "Game Guy Space"},
            {14, "Solo Minigame Space"},
            {15, "Battle Minigame Space"},
            {16, "Warp Space"},
            {17, "Start Space"},
            {18, "Map-Defined"},
        };

        private static Dictionary<int, bool> landBools = new Dictionary<int, bool>()
        {
            {1, true},
            {2, true},
            {3, true},
            {4, true},
            {5, true},
            {6, false},
            {7, false},
            {8, false},
            {9, false},
            {10, false},
            {11, true},
            {12, false},
            {13, true},
            {14, true},
            {15, true},
            {16, true},
            {17, false},
            {18, true}, // Unless otherwise overwritten by the maps's scripting, set "Map-Defined" spaces to be landable.
        };

        public void Initialize()
        {
            if (type == 0)
                return;
            else
            {
                string n;
                goodNames.TryGetValue(type, out n);
                name = n;

                bool l;
                landBools.TryGetValue(type, out l);
                landable = l;
            }
        }

        public string name { get; set; }

        public bool landable { get; set; } = true;
        public int[] next { get; set; }          //Enumerates mapIDs of spaces to which one can move
        public int[] rnext { get; set; }         //Same as above but when one is travelling in reverse
        public int hiddenItem { get; set; }

        private string sprite;       //Path to sprite resource

        public void PlayerLand(Player p, Board b, TurnEngine t) // When a player's "space" param matches this space's mapID, execute the correct script.
        { 
            switch (this.type)
            {
                case 01:
                    _BlueSpace(p, b, t);
                    return;
                case 02:
                    _RedSpace(p, b, t);
                    return;
                case 03:
                    _HappeningSpace(p, b, t);
                    return;
                case 04:
                    _ChanceSpace(p, t);
                    return;
                case 06:
                    _ToadStarSpace(p, b);
                    return;
                case 11:
                    _BankSpace(p, b);
                    return;
                case 12:
                    _BooSpace(p, t);
                    return;
                case 17:
                    _StartSpace(p, t);
                    return;
            }
        }

        private void _BlueSpace(Player p, Board b, TurnEngine t)
        {
            if (p.moveSpaces > 0)
            {
                Record.WriteLine("{0} passed {1}.",p.charName,this.name);
                return;
            }
            else
            {
                int changeAmount;

                // Adjust this later for last 5 turns +/-6 coins
                if ((999 - p.coins) > (b.coinSpaceValue-1))
                {
                    changeAmount = b.coinSpaceValue;
                }
                else
                {
                    changeAmount = 999 - p.coins;
                }

                p.ChangeAssets(changeAmount);
                Record.WriteLine("{0} landed on {2}!\n{0} got {1} coins!\n--{0}: {3} coins", p.charName, changeAmount,this.name, p.coins);

                if (this.hiddenItem > 0)
                {
                    // Give player the hidden item here
                }

                p.alignment = 1;  // Blue team
                t.BlueTeam.Add(p);
            }
        }

        private void _RedSpace(Player p, Board b, TurnEngine t)
        {
            if (p.moveSpaces > 0)
            {
                Record.WriteLine("{0} passed {1}.", p.charName, this.name);
                return;
            }
            else
            {
                int changeAmount;

                if (p.coins > (b.coinSpaceValue-1))
                {
                    changeAmount = b.coinSpaceValue;
                }
                else
                {
                    changeAmount = p.coins;
                }

                p.ChangeAssets(-changeAmount);
                Record.WriteLine("{0} landed on {2}!\n{0} lost {1} coins!\n--{0}: {3} coins", p.charName, changeAmount,this.name, p.coins);

                if (this.hiddenItem > 0)
                {
                    // Give player the hidden item here
                }

                p.alignment = 2;  // Red team
                t.RedTeam.Add(p);
            }
        }

        private void _HappeningSpace(Player p, Board b, TurnEngine t)
        {
            // Board Controlled
            if (p.moveSpaces > 0)
            {
                Record.WriteLine("{0} passed {1}.", p.charName, this.name);
                return;
            }
            else
            {

                p.alignment = 3;  // Green...unaffiliated
                t.GreenTeam.Add(p);
                p.happeningSpaces += 1;
                Record.WriteLine("{0} landed on {1}! But nothing happened.", p.charName,this.name);
            }
        }

        private void _ChanceSpace(Player p, TurnEngine t)
        {
            if (p.moveSpaces > 0)
            {
                Record.WriteLine("{0} passed {1}.", p.charName, this.name);
                return;
            }
            else
            {
                t.GreenTeam.Add(p);
                Record.WriteLine("{0} landed on {1}!", p.charName, this.name);
                Player a = new Player();
                Player b = new Player();
                int _event;
                bool rolled = false;

                a = t.order[GlobalRNG.Next(0, 3)];

                while (rolled == false)
                {
                    int r = GlobalRNG.Next(0, 3);
                    
                    if (t.order[r] != a)
                    {
                        rolled = true;
                        b = t.order[r];
                    }
                }

                _event = GlobalRNG.Next(1, 5);
                int amt;

                switch (_event) {
                    case 01: // Give 10 coins
                        Record.WriteLine("{0} => Give (10) Coins => {1}", a.charName, b.charName);
                        amt = 10;
                        if (amt > a.coins)
                            amt = a.coins;
                        if (amt > 0)
                        { 
                            a.ChangeAssets(-amt);
                            b.ChangeAssets(amt);
                            Record.WriteLine("{0} gave {1} {2} coins!\n--{0}: {3} coins\n--{1}: {4} coins", a.charName, b.charName, amt, a.coins, b.coins);
                        } else
                        {
                            Record.WriteLine("{0} has no coins to give to {1}!", a.charName, b.charName);
                        }
                        return;
                    case 02: // Give 20 coins
                        Record.WriteLine("{0} => Give (20) Coins => {1}", a.charName, b.charName);
                        amt = 20;
                        if (amt > a.coins)
                            amt = a.coins;
                        if (amt > 0)
                        {
                            a.ChangeAssets(-amt);
                            b.ChangeAssets(amt);
                            Record.WriteLine("{0} gave {1} {2} coins!\n--{0}: {3} coins\n--{1}: {4} coins", a.charName, b.charName, amt, a.coins, b.coins);
                        }
                        else
                        {
                            Record.WriteLine("{0} has no coins to give to {1}!", a.charName, b.charName);
                        }
                        return;
                    case 03: // Give star
                        Record.WriteLine("{0} => Give Star => {1}", a.charName, b.charName);
                        amt = 1;
                        if (amt > a.stars)
                            amt = a.stars;
                        if (amt > 0)
                        {
                            a.ChangeAssets(0,-1);
                            b.ChangeAssets(0,1);
                            Record.WriteLine("{0} gave {1} a star!\n--{0}: {2} stars\n--{1}: {3} stars", a.charName, b.charName, a.stars, b.stars);
                        }
                        else
                        {
                            Record.WriteLine("{0} has no stars to give to {1}!", a.charName, b.charName);
                        }
                        return;
                    case 04: // Trade coins
                        Record.WriteLine("{0} => Trade Coins With => {1}", a.charName, b.charName);
                        int aCoins = a.coins;
                        int bCoins = b.coins;
                        a.ChangeAssets(bCoins - aCoins);
                        b.ChangeAssets(aCoins - bCoins);
                        Record.WriteLine("{0} traded coins with {1}!\n--{0}: {2} coins\n--{1}: {3} coins", a.charName, b.charName, a.coins, b.coins);
                        return;
                    case 05: // Trade stars
                        Record.WriteLine("{0} => Trade Stars With => {1}", a.charName, b.charName);
                        int aStars = a.stars;
                        int bStars = b.stars;
                        a.ChangeAssets(0, bStars - aStars);
                        b.ChangeAssets(0, aStars - bStars);
                        Record.WriteLine("{0} traded stars with {1}!\n--{0}: {2} stars\n--{1}: {3} stars", a.charName, b.charName, a.stars, b.stars);
                        return;
                }
            }
        }

        private void _StartSpace(Player p, TurnEngine t)
        {
            if (t.currentTurn > 1)
            {
                // +10 coins
                p.ChangeAssets(10);
                Record.WriteLine("{0} passed the Start Space!\nKoopa Troopa gave {0} 10 coins!\n--{0}: {1} coins", p.charName, p.coins);
            }
        }

        private void _ToadStarSpace(Player p, Board b)
        {
            Record.WriteLine("{0} reached a Star Space!", p.charName);
            if (p.coins >= 20)
            {
                p.ChangeAssets(-20, 1);

                Record.WriteLine("{0} traded 20 coins for a star!", p.charName);

                // Move star
                int curStarSpace = b.starSpaces.IndexOf(this.mapID);
                curStarSpace++;
                if (curStarSpace > (b.starSpaces.Count() - 1))
                    curStarSpace = 0;
                b.spaces[b.starSpaces[curStarSpace]].type = 6;
                b.spaces[b.starSpaces[curStarSpace]].Initialize();
                Record.WriteLine("Star moved to mapID {0}", b.spaces[b.starSpaces[curStarSpace]].mapID);
                this.type = 1;
                this.Initialize();

            } else
            {
                Record.WriteLine("{0} couldn't afford a star!", p.charName);
            }
        }

        private void _BankSpace(Player p, Board b)
        {
            if (p.moveSpaces > 0)
            {
                Record.WriteLine("{0} passed {1}.", p.charName, this.name);
                int amt = 5;
                if (amt > p.coins)
                    amt = p.coins;
                p.ChangeAssets(-amt);
                b.bankPot += amt;
                Record.WriteLine("{0} put {1} coins in the bank!\n--{0}: {2} coins",p.charName, amt, p.coins);
                return;
            }
            else
            {
                Record.WriteLine("{0} landed on {1}.", p.charName, this.name);
                if (b.bankPot > 0)
                {
                    p.ChangeAssets(b.bankPot);
                    Record.WriteLine("{0} got {1} coins from the bank!\n--{0}: {2} coins",p.charName, b.bankPot, p.coins);
                    b.bankPot = 0;
                }
                else
                {
                    Record.WriteLine("No money in the bank!");
                }
            }

        }

        private void _BooSpace(Player p, TurnEngine t)
        {
            bool starSteal = false;
            bool coinSteal = false;
            //List<Player> starCandidates = new List<Player>();
            //List<Player> coinCandidates = new List<Player>();
            //List<int[]> starCandidates = new List<int[]>();
            //List<int[]> coinCandidates = new List<int[]>();
            Player sVictim = p; // Will be overwritten
            Player cVictim = p; // Will be overwritten
            int maxS = 0;
            int maxC = 0;

            Record.WriteLine("{0} arrived at Boo!", p.charName);

            // Check everyone's stars and coins
            foreach (Player _p in t.order)
            {
                if ((_p.stars > maxS) && (_p.playerID != p.playerID))
                {
                    starSteal = true;
                    maxS = _p.stars;
                    sVictim = _p;
                    //starCandidates.Add(_p);
                }

                if ((_p.coins > maxC) && (_p.playerID != p.playerID))
                {
                    coinSteal = true;
                    maxC = _p.coins;
                    cVictim = _p;
                    //coinCandidates.Add(_p);
                }
            }

            // Try to steal star
            if (p.coins >= 50)
            {
                p.ChangeAssets(-50, 1);
                sVictim.ChangeAssets(0, -1);

                Record.WriteLine("{0} paid Boo to steal {1}'s star!\nBoo was successful in stealing {1}'s star!\n--{0}: {2} stars\n--{1}: {3} stars", p.charName, sVictim.charName, p.stars,sVictim.stars);
            }

            // Failing that, go for coins
            else if (p.coins >= 5)
            {
                // Full random 1-21 coin steal
                p.ChangeAssets(-5);

                int _r = GlobalRNG.Next(1, 21);

                if (_r > cVictim.coins)
                    _r = cVictim.coins;

                p.ChangeAssets(_r);
                cVictim.ChangeAssets(-_r);

                Record.WriteLine("{0} paid Boo to steal coins from {1}!\nBoo stole {2} coins from {1}!\n--{0}: {3} coins\n--{1}: {4} coins", p.charName, cVictim.charName, _r, p.coins, cVictim.coins);
            }

        }
    }
}
