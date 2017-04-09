using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public class Board
    {
        public Space[] spaces { get; set; }
        
        public List<int> starSpaces { get; set; }   // Will catalog indicies of "spaces" on which a star space can be placed

        public int coinSpaceValue { get; set; } = 3;

        public int bankPot { get; set; } = 0;
        
        public void Initialize(int numSpaces)
        {
            spaces = new Space[numSpaces];
        }

        public void GenerateBoard(int size)
        {
            Record.WriteLine("Randomly generating board...\n----------------------------\n");
            spaces = new Space[size + 1]; // +1 to compensate for start space

            // Start Space
            spaces[0] = new Space { type = 17, mapID = 0, next = new int[1] { 1 }, rnext = new int[1] { size + 1 } };
            spaces[0].Initialize();
            Record.WriteLine("Created {0} at mapID {1} with next space at mapID {2}.", spaces[0].name, 0,spaces[0].next[0]);

            // Populate Spaces
            int randType;

            for (int s = 1; s < size + 1; s++)
            {
                randType = GlobalRNG.Next(1, 100);
                Record.WriteLine("---{0}", randType);

                if ((0 < randType) && (randType < 41))
                {
                    randType = 1; // Blue
                }
                if ((40 < randType) && (randType < 81))
                {
                    randType = 2; // Red
                }
                if ((80 < randType) && (randType < 101))
                {
                    randType = 3; // Event (?)
                }

                spaces[s] = new Space { type = randType, mapID = s, next = new int[1] { s + 1 }, rnext = new int[1] { s - 1 } };
                spaces[s].Initialize();
                Record.WriteLine("Created {0} at mapID {1} with next space at mapID {2}.", spaces[s].name, s, spaces[s].next[0]);
            }

            // Set last space "next" param to point at the start space
            spaces[spaces.Length-1].next[0] = 0;
            Record.WriteLine("\nReached end of spaces!\nLast space updated to point to mapID {0} as next space.\n", spaces[spaces.Length - 1].next[0]);

            // Designate some blue spaces as potential star spaces

            starSpaces = new List<int>();

            int maxStarSpaces = 6;
            int starTimeout = 12;
            //int curStarSpaces = 0;

            while ((starSpaces.Count() < maxStarSpaces) && (starTimeout>0))
            {
                starTimeout -= 1;
                for (int f = 0; f < spaces.Length; f++)
                {
                    if ((starSpaces.Count() < maxStarSpaces) && (spaces[f].type == 1))
                    {
                        randType = GlobalRNG.Next(0, 3);
                        if ((randType == 1) && !starSpaces.Contains(f))
                        {
                            starSpaces.Add(f);
                            //curStarSpaces++;
                            Record.WriteLine("Space at mapID {0} was made a potential star space!", f);
                        }
                    }
                }
            }

            randType = GlobalRNG.Next(0, starSpaces.Count-1);
            Record.WriteLine("---{0}---{1}",randType,starSpaces[randType]);
            spaces[starSpaces[randType]].type = 6;
            spaces[starSpaces[randType]].Initialize();
            Record.WriteLine("Space at mapID {0} was chosen for the star space!\n\n",starSpaces[randType]);

            bool spaceFound = false;
            
            while (spaceFound == false)
            {
                int a = GlobalRNG.Next(1, spaces.Length-1);
                //Record.WriteLine("{0}",a);
                if (spaces[a].type == 2)
                {
                    spaceFound = true;
                    spaces[a].type = 12;
                    spaces[a].Initialize();
                    Record.WriteLine("Space at mapID {0} was chosen for Boo's space!\n\n", spaces[a].mapID);
                }
            }

            spaceFound = false;

            while (spaceFound == false)
            {
                int a = GlobalRNG.Next(1, spaces.Length - 1);
                //Record.WriteLine("{0}",a);
                if (spaces[a].type == 2)
                {
                    spaceFound = true;
                    spaces[a].type = 11;
                    spaces[a].Initialize();
                    Record.WriteLine("Space at mapID {0} was chosen for a Bank!\n\n", spaces[a].mapID);
                }
            }

            spaceFound = false;

            while (spaceFound == false)
            {
                int a = GlobalRNG.Next(1, spaces.Length - 1);
                //Record.WriteLine("{0}",a);
                if ((spaces[a].type != 11) && (spaces[a].type != 12) && (!starSpaces.Contains(spaces[a].mapID)))
                {
                    spaceFound = true;
                    spaces[a].type = 4;
                    spaces[a].Initialize();
                    Record.WriteLine("Space at mapID {0} was chosen for a Chance Time space!\n\n", spaces[a].mapID);
                }
            }

        }
    }
}
