using System;
using System.Collections.Generic;
using System.Text;

namespace adventure
{
    class Interpreter
    {
        public Dictionary<TOKEN, Action> Actions = new Dictionary<TOKEN, Action>();
        public TOKEN lastToken = TOKEN.NONE;
        public string line = "";

        public Interpreter()
        {
            Actions.Add(TOKEN.GOTO, GOTO);
            Actions.Add(TOKEN.DECLARE, DECLARE);
            Actions.Add(TOKEN.SET, SET);
            Actions.Add(TOKEN.ADD, ADD);
            Actions.Add(TOKEN.MULTIPLY, MULTIPLY);
            Actions.Add(TOKEN.INPUT, INPUT);
            Actions.Add(TOKEN.EVENT, EVENT);
            Actions.Add(TOKEN.CHOICE, CHOICE);
            Actions.Add(TOKEN.WRITELINE, WRITELINE);
            Actions.Add(TOKEN.NONE, NONE);
        }

        public void Interpret(TOKEN token,string curline)
        {
            line = curline;
            Actions[token]();
        }

        void NONE()
        {
            return;
        }

        public void GOTO()
        {
            string[] pointer = line.Split("::");
            bool foundSection = false;
            for (int l = 0; l < Global.curParser.lines.Length; l++)
            {
                string line = Global.curParser.lines[l];
                if (line.StartsWith(":") && line == ":" + pointer[1])
                {
                    Global.curParser.curLine = l;
                    Global.data.curHeader = pointer[1];
                    foundSection = true;
                }
            }

            if (!foundSection)
            {
                Global.curParser.ParseError($"The defined section ({pointer}) was not found!");
            }
        }

        void DECLARE()
        {
            string var = line.Replace("declare$", "");

            bool exists = false;

            foreach (Variable item in Global.data.vars)
            {
                if (item.name == var)
                {
                    exists = true;
                }
            }

            if (!exists)
            {
                Variable newvar = new Variable();
                newvar.Set(var, "0");
                Global.data.vars.Add(newvar);
            }
                
        }

        void SET()
        {
            string[] setarray = line.Replace("set$", "").Split(";");
            if (setarray.Length != 2)
               Global.curParser.ParseError("Wrong number of arguments", Global.curParser.curLine);

            VarFunctions.ReturnVar(setarray[0]).val = setarray[1];
        }

        void ADD()
        {
            string[] addvar = line.Replace("add$", "").Split(";");
            if (addvar.Length != 2)
                Global.curParser.ParseError("Wrong number of arguments", Global.curParser.curLine);

            int result = int.Parse(VarFunctions.RetriveValues(addvar[0])) + int.Parse(VarFunctions.RetriveValues(addvar[1]));
            VarFunctions.ReturnVar(addvar[0]).val = result.ToString();
        }

        void MULTIPLY()
        {
            string[] multivar = line.Replace("multiply$", "").Split(";");
            if (multivar.Length != 2)
                Global.curParser.ParseError("Wrong number of arguments", Global.curParser.curLine);

            int mresult = int.Parse(VarFunctions.RetriveValues(multivar[0])) * int.Parse(VarFunctions.RetriveValues(multivar[1]));
            VarFunctions.ReturnVar(multivar[0]).val = mresult.ToString();
        }

        void INPUT()
        {
            string[] invar = line.Replace("input$", "").Split(";");
            if (invar.Length > 2)
                Global.curParser.ParseError("Wrong number of arguments", Global.curParser.curLine);

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
        }

        void EVENT()
        {
            string ev = line;
            switch (ev)
            {
                case "--AUTOSAVE--":
                    Adventure.SaveProgress();
                    break;
                case "--PAUSE--":
                    Console.ReadKey(true);
                    break;
                case "--ENDGAME--":
                    Global.stop = true;
                    break;
            }
        }

        void CHOICE()
        {
            int parsingLine;

            List<string> outcomes = new List<string>();
            List<string> choices = new List<string>();

            parsingLine = Global.curParser.curLine;

            while (parsingLine < Global.curParser.lines.Length && Global.curParser.lines[parsingLine].StartsWith("-"))
            {
                string[] choice = Global.curParser.lines[parsingLine].Split(";");
                Console.WriteLine(choice[0]);

                choices.Add(choice[0]);

                if (choice.Length < 2)
                {
                    Global.curParser.ParseError("Choice doesn't contain an outcome.", parsingLine);
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
                        line = $"goto::{outcomes[i]}";
                        GOTO();
                        found = true;
                    }
                }
            } while (!found);
        }

        void WRITELINE()
        {
            foreach (Variable item in Global.data.vars)
            {
                string toreplace = "$" + item.name;
                line = line.Replace(toreplace, item.val);
            }
            Console.WriteLine(line);
        }
    }
}
