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
            switch(lines[curLine])
            {
                case string s
                when s.StartsWith("goto::"):
                    string[] got = lines[curLine].Split("::");
                    GotoFunction(got[1]);
                    break;

                case string s
                when s.StartsWith("declare$"):
                    string var = lines[curLine].Replace("declare$", "");

                    bool exists = false;

                    foreach (Variable item in vars)
                    {
                        if (item.name == var)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                        vars.Add(new Variable(var, "0"));
                    break;

                case string s
                when s.StartsWith("set$"):
                    string[] setarray = lines[curLine].Replace("set$", "").Split(";");
                    if (setarray.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    ReturnVar(setarray[0]).val = setarray[1];
                    break;

                case string s
                when s.StartsWith("add$"):
                    string[] addvar = lines[curLine].Replace("add$", "").Split(";");
                    if (addvar.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    int result = int.Parse(RetriveValues(addvar[0])) + int.Parse(RetriveValues(addvar[1]));
                    ReturnVar(addvar[0]).val = result.ToString();
                    break;

                case string s
                when s.StartsWith("multiply$"):
                    string[] multivar = lines[curLine].Replace("multiply$", "").Split(";");
                    if (multivar.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    int mresult = int.Parse(RetriveValues(multivar[0])) * int.Parse(RetriveValues(multivar[1]));
                    ReturnVar(multivar[0]).val = mresult.ToString();
                    break;

                case string s
                when s.StartsWith("input$"):
                    string[] invar = lines[curLine].Replace("input$", "").Split(";");
                    if (invar.Length > 2)
                        ParseError("Wrong number of arguments", curLine);

                    string input = "";

                    if (invar.Length == 2 && invar[1] == "int")
                    {
                        do
                        {
                            Console.Write("> ");
                            input = Console.ReadLine();
                        } while (!int.TryParse(input, out _));
                    }
                    else
                    {
                        Console.Write("> ");
                        input = Console.ReadLine();
                    }

                    ReturnVar(invar[0]).val = input;
                    break;

                case string s
                when s.StartsWith("--"):
                    HandleEvent(lines[curLine]);
                    break;

                case string s
                when s.StartsWith("-"):
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
                        string cinput = Console.ReadLine();

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
                                == cinput.Replace("-", "")
                                .ToLower()
                                .Replace("!", "")
                                .Replace(".", "")
                                .Replace("?", "")
                                .Replace("'", "")
                                .Replace(",", ""))
                                choicepass = true;

                            int usercount = 0;

                            bool parsesuccess = int.TryParse(cinput, out usercount);

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
                    break;

                default:
                    if (lines[curLine].StartsWith(":"))
                        break;
                    string line = lines[curLine];
                    foreach (Variable item in vars)
                    {
                        string toreplace = "$" + item.name;
                        line = line.Replace(toreplace, item.val);
                    }
                    Console.WriteLine(line);
                    break;
            }
        }

        static Variable ReturnVar(string input)
        {
            Variable output = null;
            bool success = false;
            foreach (Variable item in vars)
            {
                if (item.name == input)
                {
                    output = item;
                    success = true;
                }
            }

            if (!success)
                ParseError("Variable not found.", curLine);
            return output;
        }

        static string RetriveValues(string input)
        {
            string output = "null";
            bool success = false;

            //Logitech

            // If num
            if(int.TryParse(input, out _))
            {
                output = input;
                success = true;
            }
            else
            {
                foreach (Variable item in vars)
                {
                    if(item.name == input)
                    {
                        output = item.val;
                        success = true;
                    }
                }
            }

            if (!success)
                ParseError($"Value {input} is not found.",curLine);
            return output;
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

            //Console.WriteLine();
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
