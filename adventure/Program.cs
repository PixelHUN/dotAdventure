using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Media;

namespace adventure
{
    class Program
    {
        static string[] lines = new string[0];
        static int curLine = 0;
        static bool stop;

        public class Variable
        {
            public string name;
            public string val;

            public Variable(string varname, string varval)
            {
                name = varname;
                val = varval;
            }
        }

        static public List<Variable> vars = new List<Variable>();

        static void Main(string[] args)
        {
            ReadFile();
            stop = false;

            do
            {
                ParseLines();
                if (curLine < lines.Length - 1)
                    curLine++;
                else
                    stop = true;
            } while (!stop);
            Console.ReadKey();
        }

        static void ParseLines()
        {
            if (lines[curLine].StartsWith(":"))
            {

            }
            else if (lines[curLine].StartsWith("goto::"))
            {
                string[] got = lines[curLine].Split("::");
                GotoFunction(got[1]);

            }
            else if(lines[curLine].StartsWith("declare$"))
            {
                string var = lines[curLine].Replace("declare$", "");

                bool exists = false;

                foreach(Variable item in vars)
                {
                    if(item.name == var)
                    {
                        exists = true;
                    }
                }

                if (!exists)
                    vars.Add(new Variable(var, "0"));
            }
            else if(lines[curLine].StartsWith("set$"))
            {
                string[] var = lines[curLine].Replace("set$", "").Split(";");
                if (var.Length != 2)
                    ParseError("Wrong number of arguments", curLine);

                bool exists = false;

                foreach (Variable item in vars)
                {
                    if (item.name == var[0])
                    {
                        exists = true;
                        item.val = var[1];
                    }
                }

                if (!exists)
                {
                    //vars.Add(new Variable(var, 0));
                    ParseError($"Variable ({var[0]}) doesn't exist", curLine);
                }
            }
            else if (lines[curLine].StartsWith("add$"))
            {
                string[] var = lines[curLine].Replace("add$", "").Split(";");
                if (var.Length != 2)
                    ParseError("Wrong number of arguments", curLine);

                bool exists = false;

                foreach (Variable item in vars)
                {
                    if (item.name == var[0])
                    {
                        exists = true;
                        int addition;
                        if (!int.TryParse(var[1], out addition))
                        {
                            ParseError("Given value is not an integer (whole number)", curLine);
                        }
                        else
                        {
                            int valHolder;
                            if(int.TryParse(item.val, out valHolder))
                            {
                                valHolder += addition;
                                item.val = valHolder.ToString();
                            }
                            else
                            {
                                ParseError("Variable is not holding an integer (whole number)", curLine);
                            }
                        }
                    }
                }

                if (!exists)
                {
                    //vars.Add(new Variable(var, 0));
                    ParseError($"Variable ({var[0]}) doesn't exist", curLine);
                }
            }
            else if (lines[curLine].StartsWith("multiply$"))
            {
                string[] var = lines[curLine].Replace("multiply$", "").Split(";");
                if (var.Length != 2)
                    ParseError("Wrong number of arguments", curLine);

                bool exists = false;

                foreach (Variable item in vars)
                {
                    if (item.name == var[0])
                    {
                        exists = true;
                        int multiplier;
                        if (!int.TryParse(var[1], out multiplier))
                        {
                            ParseError("Given value is not an integer (whole number)", curLine);
                        }
                        else
                        {
                            int valHolder;
                            if (int.TryParse(item.val, out valHolder))
                            {
                                valHolder *= multiplier;
                                item.val = valHolder.ToString();
                            }
                            else
                            {
                                ParseError("Variable is not holding an integer (whole number)", curLine);
                            }
                        }
                    }
                }

                if (!exists)
                {
                    //vars.Add(new Variable(var, 0));
                    ParseError($"Variable ({var[0]}) doesn't exist", curLine);
                }
            }
            else if (lines[curLine].StartsWith("input$"))
            {
                string[] var = lines[curLine].Replace("input$", "").Split(";");
                if (var.Length > 2)
                    ParseError("Wrong number of arguments", curLine);

                bool exists = false;

                foreach (Variable item in vars)
                {
                    if (item.name == var[0])
                    {
                        exists = true;
                        string input = "";
                        

                        int varHolder;
                        if(var.Length == 2 && var[1] == "int")
                        {
                            do
                            {
                                Console.Write("> ");
                                input = Console.ReadLine();
                            } while (!int.TryParse(input, out varHolder));
                        }
                        else
                        {
                            do
                            {
                                Console.Write("> ");
                                input = Console.ReadLine();
                            } while (input == "");
                        }

                        item.val = input;
                    }
                }

                if (!exists)
                {
                    //vars.Add(new Variable(var, 0));
                    ParseError($"Variable ({var[0]}) doesn't exist", curLine);
                }
            }
            else if (lines[curLine].StartsWith("--"))
            {
                HandleEvent(lines[curLine]);
            }
            else if (lines[curLine].StartsWith("-"))
            {
                int parsingLine;

                List<string> outcomes = new List<string>();
                List<string> choices = new List<string>();

                parsingLine = curLine;

                while (parsingLine < lines.Length && lines[parsingLine].StartsWith("-"))
                {
                    string[] choice = lines[parsingLine].Split(";");
                    Console.WriteLine(choice[0]);

                    choices.Add(choice[0]);

                    if (choice.Length < 2)
                    {
                        ParseError("Choice doesn't contain an outcome.", parsingLine);
                    }
                    else
                        outcomes.Add(choice[1]);

                    parsingLine++;
                }

                bool found = false;

                do
                {
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    for (int i = 0; i < choices.Count; i++)
                    {
                        string ch = choices[i];
                        bool choicepass = false;
                        if (ch.Replace("-", "")
                            .ToLower()
                            .Replace("!", "")
                            .Replace(".", "")
                            .Replace("?", "")
                            .Replace("'", "")
                            .Replace(",", "")
                            == input.Replace("-", "")
                            .ToLower()
                            .Replace("!", "")
                            .Replace(".", "")
                            .Replace("?", "")
                            .Replace("'", "")
                            .Replace(",", ""))
                            choicepass = true;

                        int usercount = 0;

                        bool parsesuccess = int.TryParse(input, out usercount);

                        if (parsesuccess && i + 1 == usercount)
                            choicepass = true;

                        if (choicepass)
                        {
                            Console.Clear();
                            GotoFunction(outcomes[i]);
                            found = true;
                        }
                    }
                } while (!found);
            }
            else
            {
                string line = lines[curLine];
                foreach(Variable item in vars)
                {
                    string toreplace = "$"+item.name;
                    line = line.Replace(toreplace, item.val);
                }
                Console.WriteLine(line);
            }
        }

        static void HandleEvent(string ev)
        {
            switch(ev)
            {
                case "--ENDGAME--":
                    stop = true;
                    break;
            }
        }

        static void ReadFile()
        {
            string gamedir = Directory.GetCurrentDirectory();
            string gamFile = gamedir + "/game.gam";

            lines = File.ReadAllLines(gamFile);
        }

        static void GotoFunction(string pointer)
        {
            bool foundSection = false;
            for (int l = 0; l < lines.Length; l++)
            {
                string line = lines[l];
                if (line.StartsWith(":") && line == ":" + pointer)
                {
                    curLine = l;
                    foundSection = true;
                }
            }

            if(!foundSection)
            {
                ParseError($"The defined section ({pointer}) was not found!");
            }
        }

        static void ParseError(string error, int errorline = -1)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine();
            Console.WriteLine("Parsing Error!");
            Console.WriteLine(error);
            if(errorline != -1)
            {
                Console.WriteLine("At:");
                Console.WriteLine($"Line {errorline+1}");
            }

            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
