using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    string var = lines[curLine].Replace("declare$", "");

                    bool exists = false;

                    foreach (Variable item in Global.vars)
                    {
                        if (item.name == var)
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                        Global.vars.Add(new Variable(var, "0"));
                    break;

                case string s
                when s.StartsWith("set$"):
                    token = TOKEN.SET;
                    string[] setarray = lines[curLine].Replace("set$", "").Split(";");
                    if (setarray.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    VarFunctions.ReturnVar(setarray[0]).val = setarray[1];
                    break;

                case string s
                when s.StartsWith("add$"):
                    token = TOKEN.ADD;
                    string[] addvar = lines[curLine].Replace("add$", "").Split(";");
                    if (addvar.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    int result = int.Parse(VarFunctions.RetriveValues(addvar[0])) + int.Parse(VarFunctions.RetriveValues(addvar[1]));
                    VarFunctions.ReturnVar(addvar[0]).val = result.ToString();
                    break;

                case string s
                when s.StartsWith("multiply$"):
                    token = TOKEN.MULTIPLY;
                    string[] multivar = lines[curLine].Replace("multiply$", "").Split(";");
                    if (multivar.Length != 2)
                        ParseError("Wrong number of arguments", curLine);

                    int mresult = int.Parse(VarFunctions.RetriveValues(multivar[0])) * int.Parse(VarFunctions.RetriveValues(multivar[1]));
                    VarFunctions.ReturnVar(multivar[0]).val = mresult.ToString();
                    break;

                case string s
                when s.StartsWith("input$"):
                    token = TOKEN.INPUT;
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

                    VarFunctions.ReturnVar(invar[0]).val = input;
                    break;

                case string s
                when s.StartsWith("--"):
                    token = TOKEN.EVENT;
                    HandleEvent(lines[curLine]);
                    break;

                case string s
                when s.StartsWith("-"):
                    token = TOKEN.CHOICE;
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

                    token = TOKEN.WRITELINE;
                    string line = lines[curLine];
                    foreach (Variable item in Global.vars)
                    {
                        string toreplace = "$" + item.name;
                        line = line.Replace(toreplace, item.val);
                    }
                    Console.WriteLine(line);
                    break;
            }
            ip.Interpret(token,lines[curLine]);
        }

        public void HandleEvent(string ev)
        {
            switch (ev)
            {
                case "--ENDGAME--":
                    Global.stop = true;
                    break;
            }
        }

        public void GotoFunction(string pointer)
        {
            
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
