using System;
using System.IO;

namespace adventure
{
    enum TOKEN
    {
        GOTO,
        DECLARE,
        SET,
        ADD,
        MULTIPLY,
        INPUT,
        EVENT,
        CHOICE,
        WRITELINE,
        NONE
    }
    class Parser
    {
        public string[] lines = new string[0];
        public int curLine = 0;
        public TOKEN token = TOKEN.NONE;
        public Interpreter ip = new Interpreter();

        public Parser(string filename)
        {
            string gamedir = Directory.GetCurrentDirectory();
            string gamFile = gamedir + "/" + filename;

            lines = File.ReadAllLines(gamFile);
        }

        public void JumpToHeader()
        {
            bool foundSection = false;
            for (int l = 0; l < lines.Length; l++)
            {
                string line = Global.curParser.lines[l];
                if (line.StartsWith(":") && line == ":" + Global.data.curHeader)
                {
                    Global.curParser.curLine = l;
                    foundSection = true;
                }
            }

            if (!foundSection)
            {
                ParseError($"The defined section ({Global.data.curHeader}) was not found!");
            }
        }

        public void ParseLines()
        {
            switch (lines[curLine])
            {
                case string s
                when s.StartsWith("goto::"):
                    token = TOKEN.GOTO;

                    break;

                case string s
                when s.StartsWith("declare$"):
                    token = TOKEN.DECLARE;

                    break;

                case string s
                when s.StartsWith("set$"):
                    token = TOKEN.SET;

                    break;

                case string s
                when s.StartsWith("add$"):
                    token = TOKEN.ADD;

                    break;

                case string s
                when s.StartsWith("multiply$"):
                    token = TOKEN.MULTIPLY;

                    break;

                case string s
                when s.StartsWith("input$"):
                    token = TOKEN.INPUT;

                    break;

                case string s
                when s.StartsWith("--"):
                    token = TOKEN.EVENT;
                    break;

                case string s
                when s.StartsWith("-"):
                    token = TOKEN.CHOICE;

                    break;

                default:
                    if (lines[curLine].StartsWith(":"))
                    {
                        Global.data.curHeader = lines[curLine].Replace(":", "");
                        break;
                    }
                    token = TOKEN.WRITELINE;

                    break;
            }
            ip.Interpret(token, lines[curLine]);
        }

        public void ParseError(string error, int errorline = -1)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            //Console.WriteLine();
            Console.WriteLine("Parsing Error!");
            Console.WriteLine(error);
            if (errorline != -1)
            {
                Console.WriteLine("At:");
                Console.WriteLine($"Line {errorline + 1}");
            }

            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
