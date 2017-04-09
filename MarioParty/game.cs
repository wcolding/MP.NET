using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public static class Game
    {
        private static Player[] _players;

        public static int randomSeed;

        public static string style;

        private static Board _gameBoard;

        private static TurnEngine _turnEngine;

        private static FileStream fs { get; set; }
        private static StreamWriter sw { get; set; }

        public static void Start(string board = "random", string players = "random", int boardLength = 30, int turns = 20, bool OutputToFile = false, int RandomSeed = 0, string style = "marioparty3")
        {
            string timeStamp = DateTime.Now.ToString();

            if (OutputToFile)
            {
                fs = new FileStream(GetFileName(timeStamp), FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                Record.output = sw;
            }

            GlobalRNG.Initialize(RandomSeed);

            if (board == "random")
            {
                _gameBoard = new Board();
                _gameBoard.GenerateBoard(boardLength);
            }

            if (players == "random")
            {
                int[] chars = GlobalRNG.DistinctValues(1, 8, 4);
                _players = new Player[4];
                for (int p = 0; p < 4; p++)
                {
                    int[] invSize = new int[0];
                    if (style == "marioparty2")
                    {
                        invSize = new int[1];
                    }
                    if (style == "marioparty3")
                    {
                        invSize = new int[3];
                    }
                    _players[p] = new Player { playerID = (p + 1), character = chars[p], items = invSize };
                    _players[p].Initialize();                    
                        
                        //.playerID = p + 1;
                    //_players[p].character = chars[p];
                }
            }

            _turnEngine = new TurnEngine { totalTurns = turns };

            _turnEngine.firstTurn(_players[0], _players[1], _players[2], _players[3], _gameBoard);

            while (_turnEngine.currentTurn < _turnEngine.totalTurns)
            {
                _turnEngine.nextTurn();
            }

            _turnEngine.bonusStars();

            if (OutputToFile)
                try { sw.Close(); }
                catch { }

        }

        private static string GetFileName(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Mario Party ");

            foreach (char c in s)
            {
                char t = c;
                /*if (t==' ')
                    t = '_' ;*/
                if (t == '/')
                    t = '-';
                if (t == ':')
                    t = ';';
                sb.Append(t);
            }

            sb.Append(".txt");
            return sb.ToString();
        }
    }
}
