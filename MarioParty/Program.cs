using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarioParty
{
    public class TestingGround

    {
        static void Main()
        {
            string timeStamp = DateTime.Now.ToString();
            
            // Uncommenting the field below will enable writing the game output to a file.

            /*
            FileStream fs = new FileStream(GetFileName(timeStamp), FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            Record.output = sw; */

            Console.WriteLine("Seed (Leave blank for none)");
            Console.Write("> ");
            try
            {
                int userSeed = Convert.ToInt32(Console.ReadLine());
                Game.Start("random", "random", 30, 20, false, userSeed);
            }
            catch
            {
                Game.Start("random", "random", 30, 20, false);
            }

            if (Record.output != null)
                Record.output.Close();

            Console.WriteLine("\nFinished! Press enter to exit.");
            Console.ReadLine();

        }

        static string GetFileName(string s)
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
