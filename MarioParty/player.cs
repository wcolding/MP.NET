using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class Player
    {
        public int playerID { get; set; }
        public int turnOrder { get; set; }
        public int character { get; set; }
        public string charName { get; set; }
        public string username { get; set; }
        public int stars { get; set; } = 0;
        public int coins { get; set; } = 0;
        public int[] items { get; set; } // = { 0, 0, 0 };
        public int alignment { get; set; }              //{Blue/red/green} to decide minigame type and teams
        public int locomotion { get; set; } = 1;        //1 for forward, -1 for backward
        public int poisonedTurns { get; set; }
        public int rank { get; set; }
        public int space { get; set; } = 0;
        public int moveSpaces { get; set; }

        // For bonus star calculation:
        public int maxCoins { get; set; } = 0;
        public int minigameCoins { get; set; } = 0;
        public int happeningSpaces { get; set; } = 0;

        private static Dictionary<int, string> charNames = new Dictionary<int, string>()
        {
            {1, "Mario"},
            {2, "Luigi"},
            {3, "Peach"},
            {4, "Yoshi"},
            {5, "DK"},
            {6, "Wario"},
            {7, "Waluigi"},
            {8, "Daisy"}
        };

        public void Initialize()
        {
            if (character == 0)
                return;
            else
            {
                string n;
                charNames.TryGetValue(character, out n);
                charName = n;
            }
        }

        /* At first, I simply directly modified the values of coins and stars,
         * but as it became necessary to track coin values (such as with bonus
         * stars), it made sense to funnel star and coin changes through this
         * function. It's particularly interesting on the Chance Time space. */
        public void ChangeAssets(int _coins = 0, int _stars = 0)
        {
            this.coins += _coins;
            this.stars += _stars;

            if (coins > maxCoins)
                maxCoins = coins;


        }

    }
}
