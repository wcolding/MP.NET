using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class Item
    {
        public int type { get; set; }

        public string name { get; set; }

        private Dictionary<int, string> itemTypes = new Dictionary<int, string>()
        {
            {1, "Mushroom" },
            {2, "Golden Mushroom" },
            {3, "Poison Mushroom" },
            {4, "Reverse Mushroom" },
            {5, "Cellular Shopper" },
            {6, "Dueling Glove" },
            {7, "Boo Repellant" },
            {8, "Magic Lamp" },
            {9, "Warp Block" },
            {10, "Bowser Phone" },
            {11, "Bowser Suit" },
            {12, "Lucky Lamp" },
            {13, "Plunder Chest" },
            {14, "Boo Bell" },
            {15, "Skeleton Key" },
            {16, "Item Bag" },
            {17, "Wacky Watch" },
            {18, "Barter Box" },
            {19, "Koopa Kard" },
            {20, "Lucky Charm" },
            {21, "Hidden 20 Coins" },
            {22, "Hidden Star" }
        };



    }
}
