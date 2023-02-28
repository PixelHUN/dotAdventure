using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Media;

namespace adventure
{
    class Global
    {
        static public List<Variable> vars = new List<Variable>();
        static public Parser curParser = new Parser();
        static public bool stop = false;
    }

    class Adventure
    {
        static void Main(string[] args)
        {
            ReadFile();
            Global.stop = false;

            do
            {
                Global.curParser.ParseLines();
                if (Global.curParser.curLine < Global.curParser.lines.Length - 1)
                    Global.curParser.curLine++;
                else
                    Global.stop = true;
            } while (!Global.stop);
            Console.ReadKey();
        }

        static void ReadFile()
        {
            string gamedir = Directory.GetCurrentDirectory();
            string gamFile = gamedir + "/game.gam";

            Global.curParser.lines = File.ReadAllLines(gamFile);
        }
    }
}
