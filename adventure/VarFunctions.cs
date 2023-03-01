using System;
using System.Collections.Generic;
using System.Text;

namespace adventure
{
    class VarFunctions
    {
        public static Variable ReturnVar(string input)
        {
            Variable output = null;
            bool success = false;
            foreach (Variable item in Global.data.vars)
            {
                if (item.name == input)
                {
                    output = item;
                    success = true;
                }
            }

            if (!success)
                Global.curParser.ParseError("Variable not found.", Global.curParser.curLine);
            return output;
        }

        public static string RetriveValues(string input)
        {
            string output = "null";
            bool success = false;

            //Logitech

            // If num
            if (int.TryParse(input, out _))
            {
                output = input;
                success = true;
            }
            else
            {
                foreach (Variable item in Global.data.vars)
                {
                    if (item.name == input)
                    {
                        output = item.val;
                        success = true;
                    }
                }
            }

            if (!success)
                Global.curParser.ParseError($"Value {input} is not found.", Global.curParser.curLine);
            return output;
        }
    }
}
