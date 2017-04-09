using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioParty
{
    public static class Record
    {
        public static StreamWriter output;

        public static void WriteLine(string s, params object[] o)
        {
            Console.WriteLine(s, o);

            // Write to file here
            if (output != null)
            {
                string parsed = String.Format(s, o);
                output.WriteLine(parsed);
            }
        }

        public static void Write(string s, params object[] o)
        { 
            Console.Write(s, o);

            // Write to file here
            if (output != null)
            {
                string parsed = String.Format(s, o);
                output.Write(parsed);
            }
        }
    }
}
