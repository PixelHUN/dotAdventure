using System;
using System.Collections.Generic;
using System.Text;

namespace adventure
{
    class Interpreter
    {
        public TOKEN lastToken = TOKEN.NONE;
        public string line = "";

        public void Interpret(TOKEN token,string curline)
        {
            line = curline;
            switch (token)
            {
                case TOKEN.GOTO:
                    GOTO();
                    break;
                case TOKEN.DECLARE:
                    DECLARE();
                    break;
                case TOKEN.SET:
                    SET();
                    break;
                case TOKEN.ADD:
                    ADD();
                    break;
                case TOKEN.MULTIPLY:
                    MULTIPLY();
                    break;
                case TOKEN.INPUT:
                    INPUT();
                    break;
                case TOKEN.EVENT:
                    EVENT();
                    break;
                case TOKEN.CHOICE:
                    CHOICE();
                    break;
                case TOKEN.WRITELINE:
                    WRITELINE();
                    break;
                case TOKEN.NONE:
                    break;
                default:
                    break;
            }
        }

        void GOTO()
        {
            string[] pointer = line.Split("::");
            bool foundSection = false;
            for (int l = 0; l < Global.curParser.lines.Length; l++)
            {
                string line = Global.curParser.lines[l];
                if (line.StartsWith(":") && line == ":" + pointer[1])
                {
                    Global.curParser.curLine = l;
                    foundSection = true;
                }
            }

            if (!foundSection)
            {
                Global.curParser.ParseError($"The defined section ({pointer}) was not found!");
            }
        }
    }
}
